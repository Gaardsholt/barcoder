using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace barcoder.Controllers
{
    [Route("image.png")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(string text, BarcodeFormat type, int width = 300, int height = 30, int rotate = 0, string color = "000000")
        {
            var translatedColor = System.Drawing.ColorTranslator.FromHtml($"#{color}");

            //http://localhost:16727/image.png?text=aasdasd&type=2048&width=300&height=300&rotate=0&color=03fcc6

            try
            {
                var barcodeWriter = new BarcodeWriter
                {
                    Format = type,
                    Options = new EncodingOptions
                    {
                        Width = width,
                        Height = height,
                        PureBarcode = true,
                        Margin = 0
                    },
                    Renderer = new BitmapRenderer
                    {
                        Foreground = translatedColor,
                        Background = System.Drawing.Color.White
                    }
                };
                var barCodeBitmap = barcodeWriter.Write(text);

                var convert = (System.Drawing.Image)barCodeBitmap;

                var someBytes = convertToByteArray(convert);

                var rotatedImage = RotateImage(someBytes, rotate, width, height);

                return new FileContentResult(rotatedImage, "image/png");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        private static byte[] convertToByteArray(System.Drawing.Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }


        private static byte[] RotateImage(byte[] imageInBytes, float degree, int width, int height)
        {
            using (var image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(imageInBytes))
            {
                image.Mutate(a => {
                    a.Rotate(degree);
                    a.BackgroundColor(SixLabors.ImageSharp.Color.Red);
                });

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, PngFormat.Instance);
                    return ms.ToArray();
                }
            }
        }

    }
}
