using barcoder.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ZXing;

namespace barcoder.Controllers
{
    [Route("Api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        [HttpGet]
        public Dictionary<string, int> GetTypes()
        {
            var allowedBarcodes = MultiFormatWriter.SupportedWriters.ToList();
            var AllBarcodes = new Dictionary<string, int>();

            foreach (int i in Enum.GetValues(typeof(BarcodeFormat)))
            {
                var name = Enum.GetName(typeof(BarcodeFormat), i);

                var isAllowed = allowedBarcodes.Any(a => Enum.GetName(typeof(BarcodeFormat), a) == name);
                if (isAllowed)
                {
                    AllBarcodes.Add(name, i);
                }

            }
            AllBarcodes.Add("HEX", 99999);

            return AllBarcodes;
        }

    }

}
