using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace barcoder.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var myTypes = new ApiController();
            return View(myTypes.GetTypes());
        }

    }
}
