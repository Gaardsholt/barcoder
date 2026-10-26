using Microsoft.AspNetCore.Mvc;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using System;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using ZXing.Rendering;

namespace barcoder.Controllers
{
    [ApiController]
    public class ImageController : ControllerBase
    {

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

                using var barcode = new BarcodeWriter<SkiaBitmapExportContext>()
                {
                    Format = type,
                    Options = barcodeOptions,
                    Renderer = new MauiBarcodeRenderer()
                }.Write(text);

                if (type == BarcodeFormat.QR_CODE && !string.IsNullOrEmpty(logo))
                    AddLogoToBarcode(barcode, logo);

                using var rotatedImage = RotateImage(barcode, rotate);
                using var ms = new MemoryStream();
                rotatedImage.WriteToStream(ms);

                return File(ms.ToArray(), "image/png");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private static void AddLogoToBarcode(SkiaBitmapExportContext barcode, string url)
        {
            var request = System.Net.WebRequest.Create(url);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var overlay = SkiaImage.FromStream(stream))
            {
                var scale = Math.Min(barcode.Width / 3f / overlay.Width, barcode.Height / 3f / overlay.Height);
                var width = overlay.Width * scale;
                var height = overlay.Height * scale;
                barcode.Canvas.DrawImage(overlay, (barcode.Width - width) / 2, (barcode.Height - height) / 2, width, height);
            }
        }

        private static SkiaBitmapExportContext RotateImage(SkiaBitmapExportContext image, float angle)
        {
            var radians = angle * Math.PI / 180;
            var width = Math.Max(1, (int)Math.Round(Math.Abs(image.Width * Math.Cos(radians)) + Math.Abs(image.Height * Math.Sin(radians))));
            var height = Math.Max(1, (int)Math.Round(Math.Abs(image.Height * Math.Cos(radians)) + Math.Abs(image.Width * Math.Sin(radians))));
            var result = new SkiaBitmapExportContext(width, height, 1f);
            var canvas = result.Canvas;

            canvas.FillColor = Colors.White;
            canvas.FillRectangle(0, 0, width, height);
            canvas.Translate(width / 2f, height / 2f);
            canvas.Rotate(angle);
            canvas.DrawImage(image.Image, -image.Width / 2f, -image.Height / 2f, image.Width, image.Height);
            return result;
        }

        private sealed class MauiBarcodeRenderer : IBarcodeRenderer<SkiaBitmapExportContext>
        {
            public SkiaBitmapExportContext Render(BitMatrix matrix, BarcodeFormat format, string content)
            {
                return Render(matrix, format, content, new EncodingOptions
                {
                    Width = matrix.Width,
                    Height = matrix.Height
                });
            }

            public SkiaBitmapExportContext Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
            {
                var result = new SkiaBitmapExportContext(options.Width, options.Height, 1f);
                var canvas = result.Canvas;
                var scaleX = options.Width / (float)matrix.Width;
                var scaleY = options.Height / (float)matrix.Height;

                canvas.Antialias = false;
                canvas.FillColor = Colors.White;
                canvas.FillRectangle(0, 0, options.Width, options.Height);
                canvas.FillColor = Colors.Black;

                for (var y = 0; y < matrix.Height; y++)
                {
                    var x = 0;
                    while (x < matrix.Width)
                    {
                        if (!matrix[x, y])
                        {
                            x++;
                            continue;
                        }

                        var start = x;
                        while (x < matrix.Width && matrix[x, y])
                            x++;

                        canvas.FillRectangle(start * scaleX, y * scaleY, (x - start) * scaleX, scaleY);
                    }
                }

                return result;
            }
        }

    }
}
