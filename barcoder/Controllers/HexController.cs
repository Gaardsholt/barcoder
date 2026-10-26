using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;

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
                var backgroundColor = Color.FromArgb($"#{color}");
                var borderColor = Color.FromArgb($"#{border}");

                using var context = new SkiaBitmapExportContext(width, height, 1);
                var canvas = context.Canvas;

                canvas.FillColor = backgroundColor;
                canvas.FillRectangle(0, 0, width, height);
                canvas.StrokeColor = borderColor;
                canvas.StrokeSize = 3;
                canvas.DrawRectangle(1, 1, width - 3, height - 3);

                return File(context.Image.AsBytes(ImageFormat.Png), "image/png");
            }
            catch (Exception e)
            {
                return BadRequest($"'{color}' does not match any known color: {e.Message}");
            }
        }

    }
}
