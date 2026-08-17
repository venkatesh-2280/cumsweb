using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 
using STAWeb.Models;
using System.Data; 
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;


namespace STAWeb.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private static IConfiguration _configuration;
        private readonly UserProvider userinfo; // Declare it
        string _data = "";
        string set_Apitoken = "";

        public LoginController(IConfiguration configuration, UserProvider _userinfo)
        {
            _configuration = configuration;
            userinfo = _userinfo;
        }
        string urlstring = "";
     
        public IActionResult Login()
        { 
            ViewBag.AlertMessage = TempData["AlertMessage"];
            //HttpContext.Session.SetString("user_id", "1");
            return View();
        }
       
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult SessionExpired()
        {
            return View();
        }

        public IActionResult GetChngPwdFlag(string empCode)
        {
            urlstring = Convert.ToString(_configuration.GetSection("Appsettings")["apiurl"]) + "/GetChngPwdFlag";
            DataSet result = new DataSet();
            string post_data = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(urlstring);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    LoginModel context = new LoginModel();
                    context.empCode = empCode;
                    //context.Otp = Otp;
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(context), UTF8Encoding.UTF8, "application/json");
                    var response = client.PostAsync(urlstring, content).Result;
                    Stream data = response.Content.ReadAsStreamAsync().Result;
                    StreamReader reader = new StreamReader(data);
                    post_data = reader.ReadToEnd();
                    _data = JsonConvert.DeserializeObject<string>(post_data);
                    result = JsonConvert.DeserializeObject<DataSet>(_data);

                    _data = JsonConvert.SerializeObject(result);
                    if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                    {
                        HttpContext.Session.SetString("user_role", Convert.ToString(result.Tables[0].Rows[0]["user_role"]));
                        HttpContext.Session.SetString("user_id", Convert.ToString(result.Tables[0].Rows[0]["id"]));
                        HttpContext.Session.SetString("user_name", Convert.ToString(result.Tables[0].Rows[0]["name"]));
                        HttpContext.Session.SetString("user_code", Convert.ToString(result.Tables[0].Rows[0]["emp_code"]));
                        HttpContext.Session.SetString("user_email", Convert.ToString(result.Tables[0].Rows[0]["email"]));
                        HttpContext.Session.SetString("mandatory_field_id", Convert.ToString(result.Tables[0].Rows[0]["mandatory_field_id"]));
                    }

                }
                return Json(_data);
            }
            catch (Exception ex)
            {
                CommonController objcom = new CommonController(_configuration);
                objcom.errorlog(ex.Message, "setFieldlist");
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult ChangePassword([FromBody] LoginModel mymodel)
        {
            var apiUrl = _configuration.GetSection("Appsettings")["apiurl"] + "/ChangePassword";

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = Timeout.InfiniteTimeSpan;

                var jsonContent = new StringContent(JsonConvert.SerializeObject(mymodel), Encoding.UTF8, "application/json");
                var response = client.PostAsync(apiUrl, jsonContent).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = $"API call failed with status: {response.StatusCode}" });
                }

                var rawJson = response.Content.ReadAsStringAsync().Result;
                var nestedJson = JsonConvert.DeserializeObject<string>(rawJson);
                var parsedResult = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(nestedJson);

                var message = parsedResult?["Table"]?.FirstOrDefault()?["result"] ?? "Unknown response";
                var isSuccess = message.ToLower().Contains("success");

                return Json(new { success = isSuccess, message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult generateOTP(String empEmail)
        {
            urlstring = Convert.ToString(_configuration.GetSection("Appsettings")["apiurl"]) + "/getOTP"; ;
            DataSet result = new DataSet();
            string post_data = "";
            try
            {
                using (var client = new HttpClient())
                {
                    // client.BaseAddress = new Uri(urlstring);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    LoginModel context = new LoginModel();
                    context.empEmail = empEmail;
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(context), UTF8Encoding.UTF8, "application/json");
                    var response = client.PostAsync(urlstring, content).Result;
                    Stream data = response.Content.ReadAsStreamAsync().Result;
                    StreamReader reader = new StreamReader(data);
                    post_data = reader.ReadToEnd();
                    _data = JsonConvert.DeserializeObject<string>(post_data);
                    result = JsonConvert.DeserializeObject<DataSet>(_data);
                    _data = JsonConvert.SerializeObject(result.Tables[0]);

                }
                return Json(_data);
            }
            catch (Exception ex)
            {
                CommonController objcom = new CommonController(_configuration);
                objcom.errorlog(ex.Message, "setFieldlist");
                return Json(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Users_login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.empCode) || string.IsNullOrEmpty(model.con_pwd))
                return Json(new { success = false, message = "Username & Password required" });

            try
            {
                string preTokenUrl = _configuration["Appsettings:apiurl"] + "/auth/generate-token";
                string loginApiUrl = _configuration["Appsettings:apiurl"] + "/User_loginvalidate";

                //using var client = new HttpClient();
                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(5) // or seconds
                };

                // 🔹 Step 1: Get pre-token from API
                var preTokenResponse = await client.PostAsync(preTokenUrl, null);
                if (!preTokenResponse.IsSuccessStatusCode)
                    return Json(new { success = false, message = "Unable to get pre-token" });

                var preTokenJson = await preTokenResponse.Content.ReadAsStringAsync();
                var preToken = JObject.Parse(preTokenJson)["token"]?.ToString();
                if (string.IsNullOrEmpty(preToken))
                    return Json(new { success = false, message = "Pre-token not returned" });

                // 🔹 Step 2: Call login API with username, password, pre-token
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", preToken);

                var loginResponse = await client.PostAsync(loginApiUrl, content);
                if (!loginResponse.IsSuccessStatusCode)
                    return StatusCode((int)loginResponse.StatusCode, "Login failed");

                var rawJson = await loginResponse.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<dynamic>(rawJson);

                var user = apiResult.user;
                string id = apiResult.user.id;
                string name = apiResult.user.name;
                string email = apiResult.user.email;
                string role = apiResult.user.role;
                string user_code = apiResult.user.user_code;
                string idrole = id + "_" + role;
                userinfo.UserId = id.ToString();
                userinfo.UserRole = role.ToString();
                set_Apitoken = idrole;
                ApiTokenRefreshMiddleware.TokenUpdate(HttpContext, loginResponse, set_Apitoken);

                int min = apiResult.user.min_pwd_length ?? 0;
                int max = apiResult.user.max_pwd_length ?? 0;

                char require_uppercase = apiResult.user.require_uppercase ?? 'N';
                char require_lowercase = apiResult.user.require_lowercase ?? 'N';
                char require_number = apiResult.user.require_number ?? 'N';
                char require_special_char = apiResult.user.require_special_char ?? 'N';

                HttpContext.Session.SetString("user_id", id.ToString());
                HttpContext.Session.SetString("user_name", name.ToString());
                HttpContext.Session.SetString("user_role", role.ToString());
                HttpContext.Session.SetString("user_email", email.ToString());
                HttpContext.Session.SetString("user_code", user_code.ToString());

                HttpContext.Session.SetInt32("min_pwd_length", min);
                HttpContext.Session.SetInt32("max_pwd_length", max); 
                HttpContext.Session.SetString("require_uppercase", require_uppercase.ToString());
                HttpContext.Session.SetString("require_lowercase", require_lowercase.ToString());
                HttpContext.Session.SetString("require_number", require_number.ToString());
                HttpContext.Session.SetString("require_special_char", require_special_char.ToString());

                // 🔹 Step 5: Cookie authentication for website
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Name, user.role.ToString()),
                    new Claim(ClaimTypes.Role, user.role.ToString()),
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity)
                );

                return Json(new { success = true, redirectUrl = Url.Action("Index", "Dashboard"), role = user.role.ToString(), message = "Login success" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        } 
        public async Task<IActionResult> Logout()
        {
            userinfo.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
            userinfo.UserRole = User.FindFirst(ClaimTypes.Role)?.Value.ToString();
            set_Apitoken = "APItoken-" + userinfo.UserId + "_" + userinfo.UserRole;
            Response.Cookies.Delete(set_Apitoken); 
            return RedirectToAction("Login", "Login");
        }
    }
}
