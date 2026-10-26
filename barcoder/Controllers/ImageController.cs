using Microsoft.AspNetCore.Mvc;
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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

                var pixelData = new BarcodeWriterPixelData()
                {
                    Format = type,
                    Options = barcodeOptions
                }.Write(text);

                using var bm = Image.LoadPixelData<Bgra32>(pixelData.Pixels, pixelData.Width, pixelData.Height);

                if (type == BarcodeFormat.QR_CODE && !string.IsNullOrEmpty(logo))
                    AddLogoToBarcode(bm, logo);

                if (rotate % 360 != 0)
                {
                    var background = bm[0, 0];
                    bm.Mutate(x => x.Rotate(rotate).BackgroundColor(Color.FromPixel(background)));
                }

                using var ms = new MemoryStream();
                bm.SaveAsPng(ms);

                return File(ms.ToArray(), "image/png");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private static void AddLogoToBarcode(Image<Bgra32> bm, string url)
        {
            using var overlay = DownloadImage(url);
            var ratio = Math.Min((double)(bm.Width / 3) / overlay.Width, (double)(bm.Height / 3) / overlay.Height);
            var newWidth = Math.Max(1, (int)(overlay.Width * ratio));
            var newHeight = Math.Max(1, (int)(overlay.Height * ratio));
            overlay.Mutate(x => x.Resize(newWidth, newHeight));

            var location = new Point((bm.Width - newWidth) / 2, (bm.Height - newHeight) / 2);
            bm.Mutate(x => x.DrawImage(overlay, location, 1f));
        }

        private static Image<Bgra32> DownloadImage(string url)
        {
            var request = System.Net.WebRequest.Create(url);
            using var response = request.GetResponse();
            using var responseStream = response.GetResponseStream();
            return Image.Load<Bgra32>(responseStream);
        }
    }
}
