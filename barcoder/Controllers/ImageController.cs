using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
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


        private byte[] RotateImage(byte[] imageInBytes, float degree, int width, int height)
        {
            using (var image = Image.LoadPixelData<SixLabors.ImageSharp.PixelFormats.Rgba32>(imageInBytes, width, height))
            {
                image.Mutate(a => a.Rotate(degree));

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormats.Png);
                    return ms.ToArray();
                }
            }
        }


    }
}