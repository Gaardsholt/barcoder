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
        public IActionResult Get(string color, string border, int width = 300, int height = 300)
        {
            try
            {
                var backgroundColor = ColorTranslator.FromHtml($"#{color}");
                var borderColor = ColorTranslator.FromHtml($"#{border}");

                var image = new Bitmap(width, height);
                
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.Clear(backgroundColor);
                    g.DrawRectangle(new Pen(borderColor, 3), new Rectangle(1, 1, image.Width - 3, image.Height - 3));
                }

                return File((byte[])(new ImageConverter()).ConvertTo(image, typeof(byte[])), "image/png");
            }
            catch (Exception e)
            {
                return BadRequest($"'{color}' does not match any known color: {e.Message}");
            }
        }

    }
}
