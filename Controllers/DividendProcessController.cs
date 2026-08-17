using Microsoft.AspNetCore.Mvc;
using STAWeb.Models;

namespace STAWeb.Controllers
{
    public class DividendProcessController : Controller
    {
        public IActionResult DividendProcess()
        {
            return View();
        }

        public JsonResult GetDividendProcessList()
        {
            var data = new List<DividendProcessModel>
            {
                new DividendProcessModel
                {
                    DividendName = "KOVAI MEDICAL EYE HOSPITAL - INE177F01017",
                    DividendAcNo = "234567456321",
                    DematBenposNSDL = new DateTime(2022, 02, 11),
                    DematBenposCDSL = new DateTime(2022, 03, 11),
                    PhysicalMaster = new DateTime(2022, 03, 11),
                    PanMaster = "Yes",
                    PanMasterUpdate = "Yes"
                },
                new DividendProcessModel
                {
                    DividendName = "YEEMAK PRIVATE LIMITED - INE2O4L01019",
                    DividendAcNo = "987654321098",
                    DematBenposNSDL = new DateTime(2023, 03, 10),
                    DematBenposCDSL = new DateTime(2023, 04, 11),
                    PhysicalMaster = new DateTime(2023, 04, 11),
                    PanMaster = "No",
                    PanMasterUpdate = "Yes"
                },
                 new DividendProcessModel
                {
                   DividendName = "RATHAN DIA JEWELS - INE2OLV01010",
                    DividendAcNo = "987654321870",
                    DematBenposNSDL = new DateTime(2021, 01, 11),
                    DematBenposCDSL = new DateTime(2021, 02, 10),
                    PhysicalMaster = new DateTime(2021, 02, 10),
                    PanMaster = "Yes",
                    PanMasterUpdate = "No"
                }
            };

            return Json(data);   // ✅ JSON ONLY HERE
        }
    }
}
