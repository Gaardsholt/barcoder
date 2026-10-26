using Microsoft.AspNetCore.Mvc;
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace barcoder.Controllers
{
    [ApiController]
    public class HexController : ControllerBase
    {

        [HttpGet("hex.png")]
        [HttpOptions("hex.png")]
        [HttpHead("hex.png")]
        public IActionResult Get(string color, string border, int width = 300, int height = 300)
        {
            try
            {
                var backgroundColor = Color.ParseHex(color);
                var borderColor = Color.ParseHex(border);

                using var image = new Image<Rgba32>(width, height, backgroundColor.ToPixel<Rgba32>());

                // Border of 3px drawn inside the image bounds
                const int thickness = 3;
                var borderPixel = borderColor.ToPixel<Rgba32>();
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        if (x < thickness || y < thickness || x >= width - thickness || y >= height - thickness)
                            image[x, y] = borderPixel;
                    }
                }

                using var ms = new MemoryStream();
                image.SaveAsPng(ms);
                return File(ms.ToArray(), "image/png");
            }
            catch (Exception e)
            {
                return BadRequest($"'{color}' does not match any known color: {e.Message}");
            }
        }

    }
}
