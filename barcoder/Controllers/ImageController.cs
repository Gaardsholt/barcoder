using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using ZXing;
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
            var image = (Image)bm;
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
            g.DrawImage(overlay, new Point(deltaWidth / 2, deltaHeigth / 2));
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

        private Bitmap RotateImage(Bitmap bm, float angle)
        {
            // Make a Matrix to represent rotation by this angle.
            var rotate_at_origin = new Matrix();
            rotate_at_origin.Rotate(angle);

            // Rotate the image's corners to see how big it will be after rotation.
            PointF[] points =
            {
                new PointF(0, 0),
                new PointF(bm.Width, 0),
                new PointF(bm.Width, bm.Height),
                new PointF(0, bm.Height),
            };
            rotate_at_origin.TransformPoints(points);


            var result = CreateBitMapThatFits(points);

            // Create the real rotation transformation.
            var rotate_at_center = new Matrix();
            rotate_at_center.RotateAt(angle, new PointF(result.Width / 2f, result.Height / 2f));

            // Draw the image onto the new bitmap rotated.
            using (Graphics gr = Graphics.FromImage(result))
            {
                gr.InterpolationMode = InterpolationMode.High;
                gr.Clear(bm.GetPixel(0, 0));

                gr.Transform = rotate_at_center;

                // Draw the image centered on the bitmap.
                int x = (result.Width - bm.Width) / 2;
                int y = (result.Height - bm.Height) / 2;
                gr.DrawImage(bm, x, y);
            }

            return result;
        }

        private static Bitmap CreateBitMapThatFits(PointF[] points)
        {
            var xmin = points[0].X;
            var xmax = xmin;
            var ymin = points[0].Y;
            var ymax = ymin;
            foreach (PointF point in points)
            {
                if (xmin > point.X) xmin = point.X;
                if (xmax < point.X) xmax = point.X;
                if (ymin > point.Y) ymin = point.Y;
                if (ymax < point.Y) ymax = point.Y;
            }

            int wid = (int)Math.Round(xmax - xmin);
            int hgt = (int)Math.Round(ymax - ymin);
            var result = new Bitmap(wid, hgt);

            return result;
        }

    }
}
