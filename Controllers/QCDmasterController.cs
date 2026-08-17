using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Recon_proto.Models;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
namespace STAWeb.Controllers
{
    [Authorize]
    public class QCDmasterController : Controller
    {
        private IConfiguration _configuration; 
        private readonly UserProvider userinfo;  
        string APIcookieName = "";

        public QCDmasterController(IConfiguration configuration, UserProvider _userinfo)
        {
            _configuration = configuration;
            userinfo = _userinfo;
            //userinfo.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
            //userinfo.UserRole = User.FindFirst(ClaimTypes.Role)?.Value.ToString();
            //APIcookieName = userinfo.UserId + "-" + userinfo.UserRole;
        }
        string urlstring = "";
        public IActionResult QcdMaster(UserProvider _userinfo)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            ViewBag.UserName = User.Identity?.Name;
            ViewBag.UserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            userinfo.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
            userinfo.UserRole = User.FindFirst(ClaimTypes.Role)?.Value.ToString();
            //APIcookieName = userinfo.UserId + "-" + userinfo.UserRole;
            APIcookieName = "APItoken-" + User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString() + "_" + User.FindFirst(ClaimTypes.Role)?.Value.ToString();
            HttpContext.Items["Userinfo"] = APIcookieName;
            return View();
        }

        #region QcdMasterGridRead
        [HttpPost]
        public JsonResult QcdMasterGridRead([FromBody] QcdlistModal context)
        {
            urlstring = _configuration.GetSection("Appsettings")["apiurl"].ToString();
            QcdlistModal objList = new QcdlistModal();
            DataTable result = new DataTable();
            List<QcdMasterModel> objcat_lst = new List<QcdMasterModel>();
            string post_data = "";
            try
            {
                using (var client = new HttpClient())
                {
                    APIcookieName = "APItoken-" + User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString() + "_" + User.FindFirst(ClaimTypes.Role)?.Value.ToString();

                    string Urlcon = "qcdmaster/";
                    client.BaseAddress = new Uri(urlstring + Urlcon);
                    //client.BaseAddress = new Uri("https://localhost:44348/api/qcdmaster/");
                    client.DefaultRequestHeaders.Accept.Clear();
					client.Timeout = Timeout.InfiniteTimeSpan;
					client.DefaultRequestHeaders.Add("user_code", context.in_user_code);
					client.DefaultRequestHeaders.Add("lang_code", _configuration.GetSection("AppSettings")["lang_code"].ToString());
					client.DefaultRequestHeaders.Add("role_code", _configuration.GetSection("AppSettings")["role_code"].ToString());
					client.DefaultRequestHeaders.Add("ipaddress", _configuration.GetSection("AppSettings")["ipaddress"].ToString());
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(context), UTF8Encoding.UTF8, "application/json");

                   // APIcookieName = userinfo.UserId +"-"+ userinfo.UserRole;
                    string token = Request.Cookies[APIcookieName];
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                    var response = client.PostAsync("QcdMasterGridRead", content).Result;

                    ApiTokenRefreshMiddleware.TokenUpdate(HttpContext, response, APIcookieName);
                    var userInfo = HttpContext.Items["Userinfo"] as UserProvider;

                    string userId = userInfo.UserId;
                    string urole = userInfo.UserRole;


                    Stream data = response.Content.ReadAsStreamAsync().Result;
                    StreamReader reader = new StreamReader(data);
                    post_data = reader.ReadToEnd();
                    string d2 = JsonConvert.DeserializeObject<string>(post_data);
                    result = JsonConvert.DeserializeObject<DataTable>(d2);
                    for (int i = 0; i < result.Rows.Count; i++)
                    {
                        QcdMasterModel objcat = new QcdMasterModel();
                        objcat.master_gid = Convert.ToInt32(result.Rows[i]["master_gid"]);
                        objcat.master_syscode = result.Rows[i]["master_syscode"].ToString();
                        objcat.master_code = result.Rows[i]["master_code"].ToString();
                        objcat.master_short_code = result.Rows[i]["master_short_code"].ToString();
                        objcat.master_name = result.Rows[i]["master_name"].ToString();
                        objcat.parent_master_syscode = result.Rows[i]["parent_master_syscode"].ToString();
                        objcat.active_status = result.Rows[i]["active_status"].ToString();
                        objcat.active_status_desc = result.Rows[i]["active_status_desc"].ToString();
                        objcat.master_multiple_name = result.Rows[i]["master_multiple_name"].ToString();
                        objcat.parent_master_syscode_desc = result.Rows[i]["parent_master_syscode_desc"].ToString();
						objcat.depend_flag = result.Rows[i]["depend_flag"].ToString();
						objcat.depend_parent_master_syscode_desc = result.Rows[i]["depend_parent_master_syscode_desc"].ToString();
						objcat.depend_parent_master_syscode = result.Rows[i]["depend_parent_master_syscode"].ToString();
                        objcat.depend_master_syscode = result.Rows[i]["depend_master_syscode"].ToString();
						objcat_lst.Add(objcat);
                    }
                    return Json(objcat_lst);
                }
            }
            catch (Exception ex)
            {
                CommonController objcom = new CommonController(_configuration);
                objcom.errorlog(ex.Message, "QcdMasterGridRead");
                return Json(ex.Message);
            }
        }

        public class QcdlistModal
        {
            public string? in_user_code { get; set; }
            public string? in_master_code { get; set; } 

        }
        #endregion

        #region QcdCrud
        [HttpPost]
        public JsonResult QcdCrud([FromBody] QcdCrudModal context)
        {
            urlstring = _configuration.GetSection("Appsettings")["apiurl"].ToString();
            QcdCrudModal objList = new QcdCrudModal();
            DataTable result = new DataTable();
            string post_data = "";
            try
            {
                using (var client = new HttpClient())
                {
                    string Urlcon = "/api/qcdmaster/";
                    client.BaseAddress = new Uri(urlstring + Urlcon);
                    //client.BaseAddress = new Uri("https://localhost:44348/api/qcdmaster/");
                    client.DefaultRequestHeaders.Accept.Clear();
					client.Timeout = Timeout.InfiniteTimeSpan;
					client.DefaultRequestHeaders.Add("user_code",  context.action_by);
					client.DefaultRequestHeaders.Add("lang_code", _configuration.GetSection("AppSettings")["lang_code"].ToString());
					client.DefaultRequestHeaders.Add("role_code", _configuration.GetSection("AppSettings")["role_code"].ToString());
					client.DefaultRequestHeaders.Add("ipaddress", _configuration.GetSection("AppSettings")["ipaddress"].ToString());
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(context), UTF8Encoding.UTF8, "application/json");
                    try
                    {
                       // APIcookieName = userinfo.UserId + "-" + userinfo.UserRole;
                        string token = Request.Cookies[APIcookieName];
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", token);


                        var response = client.PostAsync("QcdMaster", content).Result;

                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            Response.Cookies.Delete(APIcookieName);
                            return Json(new
                            {
                                success = false,
                                authExpired = true,
                                message = "Session expired. Please login again."
                            });
                        }
                        ApiTokenRefreshMiddleware.TokenUpdate(HttpContext, response, APIcookieName);
                        var userInfo = HttpContext.Items["Userinfo"] as UserProvider;


                        Stream data = response.Content.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(data);
                        post_data = reader.ReadToEnd();
                        string d2 = JsonConvert.DeserializeObject<string>(post_data);
                        // result = JsonConvert.DeserializeObject<DataTable>(d2);
                        return Json(d2);
                    }
					catch (HttpRequestException ex)
					{
						return Json(ex);
					}
				}
            } catch (Exception ex)
            {
                CommonController objcom = new CommonController(_configuration);
                objcom.errorlog(ex.Message, "QcdCrud");
                return Json(ex.Message);
            }
        }

        public class QcdCrudModal
        {
            public string? masterCode { get; set; }
            public string? active_status { get; set; }
            public string? action { get; set; }
			public string? action_by { get; set; }
			public int? masterGid { get; set; }
            public string? mastermutiplename { get; set; }
            public string? masterName { get; set; }
            public string? masterShortCode { get; set; }
            public string? masterSyscode { get; set; }
            public string? parentMasterSyscode { get; set; }
			public string? depend_parent_master_syscode { get; set; }
			public string? depend_master_syscode { get; set; }
			public string? depend_flag { get; set; } 
        }
		#endregion

		#region allQcdmaster
		[HttpPost]
		public JsonResult getallQcdmaster([FromBody] Qcdgridread context)
		{
            //string ss = HttpContext.Items["Userinfo"]?.ToString() ?? "";
			urlstring = _configuration.GetSection("Appsettings")["apiurl"].ToString();
			DataTable result = new DataTable();
			List<QcdMasterModel> objcat_lst = new List<QcdMasterModel>();
			string post_data = "";
			try
			{
				using (var client = new HttpClient())
				{  
                    APIcookieName = "APItoken-" + User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString() + "_" + User.FindFirst(ClaimTypes.Role)?.Value.ToString();
                    string Urlcon = "/api/Qcdmaster/";
					client.BaseAddress = new Uri(urlstring + Urlcon);
					//client.BaseAddress = new Uri("http://localhost:4195/api/Qcdmaster/");
					client.DefaultRequestHeaders.Accept.Clear();
					client.Timeout = Timeout.InfiniteTimeSpan;
					client.DefaultRequestHeaders.Add("user_code", context.in_user_code);
					client.DefaultRequestHeaders.Add("lang_code", _configuration.GetSection("AppSettings")["lang_code"].ToString());
					client.DefaultRequestHeaders.Add("role_code", _configuration.GetSection("AppSettings")["role_code"].ToString());
					client.DefaultRequestHeaders.Add("ipaddress", _configuration.GetSection("AppSettings")["ipaddress"].ToString());
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					HttpContent content = new StringContent(JsonConvert.SerializeObject(context), UTF8Encoding.UTF8, "application/json");
                   // APIcookieName = userinfo.UserId + "-" + userinfo.UserRole;
                    string token = Request.Cookies[APIcookieName];
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                    var response = client.PostAsync("getallqcdmaster", content).Result;

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    { 
                        Response.Cookies.Delete(APIcookieName); 
                        return Json(new
                        {
                            success = false,
                            authExpired = true,
                            message = "Session expired. Please login again."
                        });
                    }
                    //ApiTokenRefreshMiddleware.TokenUpdate(HttpContext, response, APIcookieName);



                    Stream data = response.Content.ReadAsStreamAsync().Result;
					StreamReader reader = new StreamReader(data);
					post_data = reader.ReadToEnd();
					string d2 = JsonConvert.DeserializeObject<string>(post_data);
					result = JsonConvert.DeserializeObject<DataTable>(d2);
					for (int i = 0; i < result.Rows.Count; i++)
					{
						QcdMasterModel objcat = new QcdMasterModel();
						objcat.master_gid = Convert.ToInt32(result.Rows[i]["master_gid"]);
						objcat.master_syscode = result.Rows[i]["master_syscode"].ToString();
						objcat.master_code = result.Rows[i]["master_code"].ToString();
						objcat.master_short_code = result.Rows[i]["master_short_code"].ToString();
						objcat.master_name = result.Rows[i]["master_name"].ToString();
						objcat.parent_master_syscode = result.Rows[i]["parent_master_syscode"].ToString();
						objcat.active_status = result.Rows[i]["active_status"].ToString();
						objcat.active_status_desc = result.Rows[i]["active_status_desc"].ToString();
						objcat.master_multiple_name = result.Rows[i]["master_multiple_name"].ToString();
						objcat.parent_master_syscode_desc = result.Rows[i]["parent_master_syscode_desc"].ToString();
						objcat.depend_flag = result.Rows[i]["depend_flag"].ToString();
                        objcat.system_flag = result.Rows[i]["system_flag"].ToString();
                        objcat.depend_parent_master_syscode_desc = result.Rows[i]["depend_parent_master_syscode_desc"].ToString();
						objcat.depend_parent_master_syscode = result.Rows[i]["depend_parent_master_syscode"].ToString();
						objcat.depend_master_syscode = result.Rows[i]["depend_master_syscode"].ToString();
						objcat_lst.Add(objcat);
					}
					return Json(objcat_lst);
				}
			}
			catch (Exception ex)
			{
				CommonController objcom = new CommonController(_configuration);
				objcom.errorlog(ex.Message, "getallqcdmaster");
				return Json(ex.Message);
			}
		}


		public class Qcdgridread
		{
			public string in_user_code { get; set; }
			public string in_master_code { get; set; } 
        }
		#endregion

	}
}
