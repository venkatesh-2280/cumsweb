using Microsoft.AspNetCore.Mvc;
using STAWeb.Models;

namespace STAWeb.Controllers
{
    public class MasterController : Controller
    {
        public IActionResult Master()
        {
            return View();
        }

        public JsonResult GetMasterList()
        {
            var data = new List<MasterModel>
            {
                new MasterModel
                {
                    Code = "QCD_mst_finance",
                    Name = "Finance",
                    DependentValue = ""
                },
                new MasterModel
                {
                     Code = "QCD_mst_hr",
                    Name = "HR",
                    DependentValue = ""
                }
            };

            return Json(data);   // ✅ JSON ONLY HERE
        }

    }
}
