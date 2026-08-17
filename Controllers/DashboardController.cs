using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;


namespace STAWeb.Controllers
{
   // [Authorize]

    
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserProvider userinfo; // Declare it
        public DashboardController(IConfiguration configuration, UserProvider _userinfo)
        {
            _configuration = configuration;
            userinfo = _userinfo;
        }
        public IActionResult Dashboard()
        { 
            //userinfo.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
            //userinfo.UserRole = User.FindFirst(ClaimTypes.Role)?.Value.ToString();
            return View();
        }
    }
}
