using Microsoft.AspNetCore.Mvc;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using SkiaSharp;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using ZXing;
using ZXing.QrCode.Internal;
using ZXing.Rendering;

namespace barcoder.Controllers
{
    [ApiController]
    public class ImageController : ControllerBase
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [HttpGet("image.png")]
        [HttpOptions("image.png")]
        [HttpHead("image.png")]
        public IActionResult Get(string text, BarcodeFormat type, string logo, int width = 300, int height = 30, int rotate = 0)
        {
            try
            {
                var barcodeOptions = new ZXing.Common.EncodingOptions()
                {
                    Height = height,
                    Width = width,
                    Margin = 0,
                    PureBarcode = true
                };
                barcodeOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);

                var pixelData = new BarcodeWriter<PixelData>()
                {
                    Format = type,
                    Options = barcodeOptions,
                    Renderer = new PixelDataRenderer()
                }.Write(text);

                using var barcode = ToBitmap(pixelData);
                using var overlay = type == BarcodeFormat.QR_CODE && !string.IsNullOrEmpty(logo)
                    ? DownloadImage(logo)
                    : null;

                return File(Render(barcode, overlay, rotate), "image/png");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private static SKBitmap ToBitmap(PixelData data)
        {
            var bitmap = new SKBitmap(new SKImageInfo(data.Width, data.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
            Marshal.Copy(data.Pixels, 0, bitmap.GetPixels(), data.Pixels.Length);
            return bitmap;
        }

        private static SKBitmap DownloadImage(string url)
        {
            using var stream = HttpClient.GetStreamAsync(url).GetAwaiter().GetResult();
            return SKBitmap.Decode(stream) ?? throw new InvalidDataException("The logo is not a valid image.");
        }

        private static byte[] Render(SKBitmap barcode, SKBitmap overlay, float angle)
        {
            var radians = angle * Math.PI / 180;
            var cos = Math.Abs(Math.Cos(radians));
            var sin = Math.Abs(Math.Sin(radians));
            var outWidth = Math.Max(1, (int)Math.Round(barcode.Width * cos + barcode.Height * sin));
            var outHeight = Math.Max(1, (int)Math.Round(barcode.Width * sin + barcode.Height * cos));

            var background = barcode.GetPixel(0, 0);

            using var context = new SkiaBitmapExportContext(outWidth, outHeight, 1);
            var canvas = context.Canvas;

            canvas.FillColor = Color.FromRgba(background.Red, background.Green, background.Blue, background.Alpha);
            canvas.FillRectangle(0, 0, outWidth, outHeight);

            canvas.Translate(outWidth / 2f, outHeight / 2f);
            canvas.Rotate(angle);
            canvas.Translate(-barcode.Width / 2f, -barcode.Height / 2f);

            var barcodeImage = new SkiaImage(barcode);
            canvas.DrawImage(barcodeImage, 0, 0, barcode.Width, barcode.Height);

            if (overlay != null)
            {
                var ratio = Math.Min((double)(barcode.Width / 3) / overlay.Width, (double)(barcode.Height / 3) / overlay.Height);
                var newWidth = (int)(overlay.Width * ratio);
                var newHeight = (int)(overlay.Height * ratio);

                var overlayImage = new SkiaImage(overlay);
                canvas.DrawImage(overlayImage, (barcode.Width - newWidth) / 2, (barcode.Height - newHeight) / 2, newWidth, newHeight);
            }

            return context.Image.AsBytes(ImageFormat.Png);
        }

    }
}
