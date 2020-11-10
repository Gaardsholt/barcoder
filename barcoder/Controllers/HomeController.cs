using barcoder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace barcoder.Controllers
{
    // mobilepay://send?phone=46028&comment=Smart+QR-kodegenerator+%3A-%29&amount=49.00&lock=1

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var myTypes = new ApiController();
            return View(myTypes.GetTypes());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
