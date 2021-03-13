using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.IO;
using ZXing;
using ZXing.QrCode.Internal;
using ZXing.Rendering;

namespace barcoder.Controllers
{
    [Route("image.png")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        
        [HttpGet]
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

                var bm = new BarcodeWriter()
                {
                    Format = type,
                    Options = barcodeOptions,
                    Renderer = new BitmapRenderer()
                }.Write(text);

                if (type == BarcodeFormat.QR_CODE && !string.IsNullOrEmpty(logo))
                    bm = AddLogoToBarcode(bm, logo);

                var rotatedImage = RotateImage(bm, rotate);
                var imageAsByte = Bitmap2Byte(rotatedImage);

                return File(imageAsByte, "image/png");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private static byte[] Bitmap2Byte(Bitmap bm)
        {
            var image = (System.Drawing.Image)bm;
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private static Bitmap AddLogoToBarcode(Bitmap bm, string url)
        {
            var overlay = DownloadImageAsBitmap(url);
            overlay = ScaleImage(overlay, bm.Width / 3, bm.Height / 3);

            var deltaHeigth = bm.Height - overlay.Height;
            var deltaWidth = bm.Width - overlay.Width;

            var g = Graphics.FromImage(bm);
            g.DrawImage(overlay, new System.Drawing.Point(deltaWidth / 2, deltaHeigth / 2));
            return bm;
        }

        private static Bitmap DownloadImageAsBitmap(string url)
        {
            var request = System.Net.WebRequest.Create(url);
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            var overlay = new Bitmap(responseStream);

            return overlay;
        }


        public static Bitmap ScaleImage(Bitmap bmp, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / bmp.Width;
            var ratioY = (double)maxHeight / bmp.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(bmp.Width * ratio);
            var newHeight = (int)(bmp.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(bmp, 0, 0, newWidth, newHeight);

            return newImage;
        }

        private static Bitmap RotateImage(Bitmap bm, float rotate)
        {
            using (Graphics g = Graphics.FromImage(bm))
            {
                // Set the rotation point to the center in the matrix
                g.TranslateTransform(bm.Width / 2, bm.Height / 2);
                // Rotate
                g.RotateTransform(rotate);
                // Restore rotation point in the matrix
                g.TranslateTransform(-bm.Width / 2, -bm.Height / 2);
                // Draw the image on the bitmap
                g.DrawImage(bm, new System.Drawing.Point(0, 0));
            }
            return bm;
        }

    }
}
