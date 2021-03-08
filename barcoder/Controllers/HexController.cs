using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;

namespace barcoder.Controllers
{
    [Route("hex.png")]
    [ApiController]
    public class HexController : ControllerBase
    {

        [HttpGet("")]
        public IActionResult Get(string color, int width = 300, int height = 300)
        {
            try
            {
                var image = new Bitmap(width, height);
                var graph = Graphics.FromImage(image);
                var translatedColor = ColorTranslator.FromHtml($"#{color}");

                graph.Clear(translatedColor);

                return File((byte[])(new ImageConverter()).ConvertTo(image, typeof(byte[])), "image/png");
            }
            catch (Exception e)
            {
                return BadRequest($"'{color}' does not match any known color: {e.Message}");
            }
        }

    }
}
