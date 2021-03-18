using barcoder.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace barcoder.Controllers
{
    // mobilepay://send?phone=46028&comment=Smart+QR-kodegenerator+%3A-%29&amount=49.00&lock=1
    // https://stadel.dk/MobilePay_QR_kode_generator

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var myTypes = new ApiController();
            return View(myTypes.GetTypes());
        }


        [HttpGet("Mobilepay")]
        public IActionResult Mobilepay()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
