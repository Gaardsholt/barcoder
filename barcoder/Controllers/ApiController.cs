using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using barcoder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZXing;

namespace barcoder.Controllers
{
    [Route("Api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        [HttpGet]
        public List<Barcode> GetTypes()
        {
            var allowedBarcodes = MultiFormatWriter.SupportedWriters.ToList();
            var AllBarcodes = new List<Barcode>();

            var values = Enum.GetValues(typeof(BarcodeFormat)).Cast<BarcodeFormat>();
            foreach (int i in Enum.GetValues(typeof(BarcodeFormat)))
            {
                var name = Enum.GetName(typeof(BarcodeFormat), i);

                var isAllowed = allowedBarcodes.Any(a => Enum.GetName(typeof(BarcodeFormat), a) == name);
                if (isAllowed)
                    AllBarcodes.Add(new Barcode { Key = name, Value = i });


            }
            return AllBarcodes;
        }

    }

}
