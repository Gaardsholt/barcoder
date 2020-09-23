using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using ZXing;

namespace barcoder.Controllers
{
    [Route("image.png")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        // http://localhost:21666/image.png?type=128&text=5901234123457&width=300&height=30
        [HttpGet]
        public IActionResult Get(string text, BarcodeFormat type, int width = 300, int height = 30, int rotate = 0)
        {
            try
            {
                var BarcodeData = new BarcodeWriterPixelData
                {
                    Format = type,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = height,
                        Width = width,
                        Margin = 0,
                        PureBarcode = true
                    }
                }.Write(text);

                var rotatedImage = RotateImage(BarcodeData.Pixels, rotate, BarcodeData.Width, BarcodeData.Height);

                return File(rotatedImage, "image/png");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        private byte[] RotateImage(byte[] imageInBytes, float degree, int width, int height)
        {
            using (var image = Image.LoadPixelData<SixLabors.ImageSharp.PixelFormats.Rgba32>(imageInBytes, width, height))
            {
                image.Mutate(a => a.Rotate(degree));

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, PngFormat.Instance);
                    return ms.ToArray();
                }
            }
        }

    }
}
