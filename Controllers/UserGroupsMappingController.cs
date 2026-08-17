using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STAWeb.Models;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;

namespace STAWeb.Controllers
{
    [Authorize]
    public class UserGroupsMappingController : Controller
    {
        string APIcookieName = "";
        public IActionResult UserGroupsMapping()
        {
            return View();
        }

        private IConfiguration _configuration;
        public UserGroupsMappingController(IConfiguration configuration)
        {

            _configuration = configuration;
        }
        string urlstring = "";


        public JsonResult UserGroups()
        {
            urlstring = Convert.ToString(_configuration.GetSection("Appsettings")["apiurl"]) + "/UserGroups";
            DataSet result = new DataSet();
            string post_data = "";
            try
            {
                using (var client = new HttpClient())
                {                   
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    APIcookieName = "APItoken-" +
                        User.FindFirst(ClaimTypes.NameIdentifier)?.Value + "_" +
                        User.FindFirst(ClaimTypes.Role)?.Value;

                    string token = Request.Cookies[APIcookieName];

                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                    var response = client.GetAsync(urlstring).Result;
                    Stream data = response.Content.ReadAsStreamAsync().Result;
                    StreamReader reader = new StreamReader(data);
                    post_data = reader.ReadToEnd();
                    string _data1 = JsonConvert.DeserializeObject<string>(post_data);
                    result = JsonConvert.DeserializeObject<DataSet>(_data1);
                    string _data = JsonConvert.SerializeObject(result.Tables[0]);
                    return Json(new { _data });
                }

            }
            catch (Exception ex)
            {
             
                return Json(ex.Message);
            }

        }

        //[HttpPost]
        //public async Task<IActionResult> CreateUserGroupsNew_old(string role_id, string role_name, string role_code, string app_code, string role_status)
        //{
        //    string user_gid = "0";
        //    int usergroup_id = 0;
        //    if (role_id != "" && role_id != null)
        //    {
        //        usergroup_id = Convert.ToInt32(role_id);
        //    }
        //    else
        //    {
        //        usergroup_id = 0;
        //    }
        //    string result = "";
        //    string msg = "";

        //    //urlstring = Convert.ToString(_configuration.GetSection("Appsettings")["apiurl"]) + "/CreateUserGroupsNew?usergroup_gid=" + 
        //    //    usergroup_gid + "&usergroup_name='" + usergroup_name + "'&usergroup_code='" + usergroup_code + 
        //    //    "'&user_gid=" + user_gid + "&app_code='" + app_code + "'&usrgrpstatus='" + usrgrpstatus + "'";
        //    urlstring = Convert.ToString(_configuration.GetSection("Appsettings")["apiurl"]) + "/CreateUserGroupsNew?" +
        //                "role_id=" + Uri.EscapeDataString(role_id ?? "0").Trim() +
        //                "&usergroup_name=" + Uri.EscapeDataString(usergroup_name?? "").Trim() +
        //                "&usergroup_code=" + Uri.EscapeDataString(usergroup_code ?? "").Trim() +
        //                "&user_gid=" + Uri.EscapeDataString(user_gid).Trim() +
        //                "&app_code=" + Uri.EscapeDataString(app_code ?? "").Trim() +
        //                "&usrgrpstatus=" + Uri.EscapeDataString(usrgrpstatus ?? "").Trim();

        //    DataSet result1 = new DataSet();
        //    string post_data = "";
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {                   
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.Timeout = Timeout.InfiniteTimeSpan;
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            APIcookieName = "APItoken-" +
        //               User.FindFirst(ClaimTypes.NameIdentifier)?.Value + "_" +
        //               User.FindFirst(ClaimTypes.Role)?.Value;

        //            string token = Request.Cookies[APIcookieName];

        //            client.DefaultRequestHeaders.Authorization =
        //                new AuthenticationHeaderValue("Bearer", token);
        //            var response = await client.PostAsync(urlstring, null);
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                return Json(new { result = false, msg = "API Error: " + response.StatusCode });
        //            }
        //            Stream data = response.Content.ReadAsStreamAsync().Result;
        //            StreamReader reader = new StreamReader(data);
        //            post_data = reader.ReadToEnd();
        //            string _data1 = JsonConvert.DeserializeObject<string>(post_data);
        //            result1 = JsonConvert.DeserializeObject<DataSet>(_data1);
        //            string _data = JsonConvert.SerializeObject(result1.Tables[0]); 
        //            return Json(new { result = true, msg = _data });
        //        }

        //    }
        //    catch (Exception ex)
        //    {               
        //        return Json(ex.Message);
        //    }
        //}


        [HttpPost]
        public async Task<IActionResult> CreateUserGroupsNew(
            string role_id,
            string role_name,
            string role_code,
            string app_code,
            string role_status)
        {
            string user_id = "0";

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string apiUrl = _configuration
                        .GetSection("Appsettings")["apiurl"];

                    // Token
                    string cookieName = "APItoken-" +
                        User.FindFirst(ClaimTypes.NameIdentifier)?.Value + "_" +
                        User.FindFirst(ClaimTypes.Role)?.Value;

                    string token = Request.Cookies[cookieName];

                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                    // Send as POST BODY
                    var values = new Dictionary<string, string>
            {
                { "role_id", role_id ?? "0" },
                { "role_name", role_name ?? "" },
                { "role_code", role_code ?? "" },
                { "user_id", user_id },
                { "app_code", app_code ?? "" },
                { "role_status", role_status ?? "" }
            };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync(
                        apiUrl + "/CreateUserGroupsNew",
                        content);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Response.Cookies.Delete(APIcookieName);

                        return Json(new
                        {
                            authExpired = true,
                            message = "Session expired. Please login again."
                        });
                    }

                    var apiResult = await response.Content.ReadAsStringAsync();

                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(apiResult);

                    return Json(new
                    {
                        result = json.msg == 1,
                        msg = json.result.ToString()
                    });

                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = false,
                    msg = ex.Message
                });
            }
        }
        [HttpPost]
        public ActionResult RoleMapping(string role_code,string role_name, string app_code, string app_name, string mode_)
        {
            ViewBag.role_code = role_code;
            ViewBag.role_name = role_name;
            ViewBag.app_code = app_code;
            ViewBag.app_name = app_name;
            ViewBag.mode_flag = mode_;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoleMappingData_(string role_code, string app_code)
        {
            try
            {
                string urlstring = $"{_configuration["Appsettings:apiurl"]}/RoleMapping?role_code={role_code}&app_code={app_code}";
                using (var client = new HttpClient())
                {
                    APIcookieName = "APItoken-" +
                        User.FindFirst(ClaimTypes.NameIdentifier)?.Value + "_" +
                        User.FindFirst(ClaimTypes.Role)?.Value;

                    string token = Request.Cookies[APIcookieName];

                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                    var response = await client.GetAsync(urlstring);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Response.Cookies.Delete(APIcookieName);

                        return Json(new
                        {
                            authExpired = true,
                            message = "Session expired. Please login again."
                        });
                    }

                    var json = await response.Content.ReadAsStringAsync();

                    // Since API already returns JSON array
                    //  var data = JsonConvert.DeserializeObject<object>(json);

                    return Content(json, "application/json");
                }
              
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });
            }
        }



        [HttpGet]

        public async Task<IActionResult> Application_List()
        {
            urlstring = _configuration.GetSection("Appsettings")["apiurl"] + "/Application_List";
            List<ApplicationModel> distinctRoles = new List<ApplicationModel>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    APIcookieName = "APItoken-" +
                        User.FindFirst(ClaimTypes.NameIdentifier)?.Value + "_" +
                        User.FindFirst(ClaimTypes.Role)?.Value;

                    string token = Request.Cookies[APIcookieName];
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync(urlstring);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Response.Cookies.Delete(APIcookieName);
                        return Json(new
                        {
                            authExpired = true,
                            message = "Session expired. Please login again."
                        });
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var post_datas = await response.Content.ReadAsStringAsync();
                        var parsed = JObject.Parse(post_datas);
                        var tables = parsed["table"].ToObject<List<ApplicationModel>>();

                        distinctRoles = tables
                            .GroupBy(r => r.app_code)
                            .Select(g => new ApplicationModel
                            { 
                                app_code = g.First().app_code,
                                app_name = g.First().app_name
                            })
                            .ToList();
                    }
                }

                return Json(distinctRoles); // ✅ JSON, not View
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveRolePermissions([FromBody] List<RolePermissionDto> permissions)
        {
            if (permissions == null || !permissions.Any())
                return Json(new { status = false, message = "No data received" });

            string apiUrl = _configuration.GetSection("Appsettings")["apiurl"] + "/SaveRolePermissions";

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = Timeout.InfiniteTimeSpan;

                    // Attach Bearer token from cookie
                    string apiCookieName = "APItoken-" +
                        User.FindFirst(ClaimTypes.NameIdentifier)?.Value + "_" +
                        User.FindFirst(ClaimTypes.Role)?.Value;

                    string token = Request.Cookies[apiCookieName];
                    if (!string.IsNullOrEmpty(token))
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", token);
                     
                    var jsonContent = new StringContent(
                        JsonConvert.SerializeObject(permissions),
                        Encoding.UTF8,
                        "application/json"
                    );
                     
                    var response = await client.PostAsync(apiUrl, jsonContent);
                     
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Response.Cookies.Delete(apiCookieName);
                        return Json(new
                        {
                            authExpired = true,
                            message = "Session expired. Please login again."
                        });
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var parsed = JObject.Parse(responseData); 
                        // Forward API response to frontend
                        return Json(parsed);
                    }
                    else
                    {
                        return Json(new { status = false, message = "API Error: " + response.StatusCode });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        
    }

    public class RolePermissionDto
    {
        //public int menu_id { get; set; }
        public string menu_code { get; set; }
        public string Add { get; set; }
        public string Modify { get; set; }
        public string Delete { get; set; }
        public string View { get; set; }
        public string Download { get; set; }
        public string Link { get; set; }
        public string Mail { get; set; }
        public string RetReq { get; set; }
        public string Approve { get; set; }
        public string Boachecklist { get; set; }
        public string role_code { get; set; }
        public string app_code { get; set; }
    }
}
