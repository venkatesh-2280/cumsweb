using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STAWeb.Models;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.IO.Compression;
using ClosedXML.Excel;

namespace STAWeb.Controllers
{
    public class PanValidationController : Controller
    {
        public IActionResult PanValidation()
        {
            return View();
        }

        private IConfiguration _configuration;
        public PanValidationController(IConfiguration configuration)
        {

            _configuration = configuration;
        }
        string urlstring = "";
        public async Task<JsonResult> DividendList([FromBody] DividendListRequest mymodel)
        {
            var urlstring = _configuration.GetSection("Appsettings")["apiurl"] + "/DividendList";
            DataSet result = new DataSet();
            string post_data = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var json = JsonConvert.SerializeObject(mymodel);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(urlstring, content);
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
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> PanValidationProcess([FromBody] PanValidationModel mymodel)
        {
            var urlstring = _configuration.GetSection("Appsettings")["apiurl"] + "/PanValidationProcess";

            DataTable dt = new DataTable();

            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(mymodel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(urlstring, content);

                var post_data = await response.Content.ReadAsStringAsync();

                var jsonObj = JObject.Parse(post_data);

                dt = JsonConvert.DeserializeObject<DataTable>(jsonObj["Table"].ToString());
            }

            // ✅ Split Size
            int chunkSize = 500;

            // Total Excel Files Needed
            int totalFiles = (int)Math.Ceiling((double)dt.Rows.Count / chunkSize);

            // ✅ Create ZIP File in Memory
            using (var zipStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    for (int fileIndex = 0; fileIndex < totalFiles; fileIndex++)
                    {
                        // Take 500 Rows Chunk
                        var chunkRows = dt.AsEnumerable()
                                          .Skip(fileIndex * chunkSize)
                                          .Take(chunkSize)
                                          .CopyToDataTable();

                        // ✅ Create Excel for Each Chunk
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("PAN Details");

                            // Headers
                            ws.Cell(1, 1).Value = "S.No";
                            ws.Cell(1, 2).Value = "PAN";
                            ws.Cell(1, 3).Value = "Name";
                            ws.Cell(1, 4).Value = "PAN_Status";

                            // Rows
                            int row = 2;
                            int sno = (fileIndex * chunkSize) + 1;

                            foreach (DataRow dr in chunkRows.Rows)
                            {
                                ws.Cell(row, 1).Value = sno;
                                ws.Cell(row, 2).Value = dr["PAN"]?.ToString();
                                ws.Cell(row, 3).Value = dr["Name"]?.ToString();
                                ws.Cell(row, 4).Value = dr["PAN_Status"]?.ToString();

                                row++;
                                sno++;
                            }

                            ws.Columns().AdjustToContents();

                            // Save Excel into MemoryStream
                            using (var excelStream = new MemoryStream())
                            {
                                workbook.SaveAs(excelStream);
                                excelStream.Position = 0;

                                // Add Excel File to ZIP
                                var zipEntry = archive.CreateEntry($"PAN_Validation_{fileIndex + 1}.xlsx");

                                using (var entryStream = zipEntry.Open())
                                {
                                    excelStream.CopyTo(entryStream);
                                }
                            }
                        }
                    }
                }

                // Return ZIP Download
                return File(
                    zipStream.ToArray(),
                    "application/zip",
                    "PAN_Validation_Files.zip"
                );
            }
        }


        [HttpGet]
        public JsonResult GetAllowedPanStatus()
        {
            var allowedStatus = _configuration.GetSection("Appsettings:AllowedPanStatus").Get<List<string>>();
            return Json(allowedStatus);
        }


        [HttpPost]
        public async Task<JsonResult> GetExpectedPanCount(int dividendgid)
        {
            try
            {
                string urlstring = _configuration.GetSection("Appsettings")["apiurl"]
                                   + "/GetExpectedPanCount";

                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(new { dividendgid });
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(urlstring, content);

                    string apiResponse = await response.Content.ReadAsStringAsync();

                    // API returns expectedCount directly
                    var obj = JObject.Parse(apiResponse);

                    int expectedCount = Convert.ToInt32(obj["expectedCount"]);

                    return Json(new { success = true, expectedCount = expectedCount });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpPost]
        public async Task<JsonResult> PanUpdateProcess([FromBody] PanUpdateModel request)
        {
            try
            {
                string urlstring = _configuration.GetSection("Appsettings")["apiurl"]
                                   + "/PanUpdateProcess";   // ✅ Bulk API endpoint

                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(request);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(urlstring, content);

                    string result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new
                        {
                            success = true,
                            message = "Bulk PAN Status Updated Successfully",
                            apiResponse = result
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "API Error",
                            apiResponse = result
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult CompDetails([FromBody] CompanyDetailsModel mymodel)
        {
            urlstring = Convert.ToString(_configuration.GetSection("Appsettings")["apiurl"]) + "/CompDetails";
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var json = JsonConvert.SerializeObject(mymodel);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(urlstring, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string resultMessage = response.Content.ReadAsStringAsync().Result;

                        // Deserialize the JSON string into an object
                        var companyData = JsonConvert.DeserializeObject<object>(resultMessage);

                        // Return that object directly, not as a string
                        return Json(new { success = true, data = companyData });
                    }


                    else
                    {
                        return Json(new { success = false, message = "API call failed: " + response.StatusCode });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<JsonResult> IudDividend([FromBody] DividendMasterModel mymodel)
        {
            var urlstring = _configuration.GetSection("Appsettings")["apiurl"] + "/IudDividend";
            DataSet result = new DataSet();
            string post_data = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var json = JsonConvert.SerializeObject(mymodel);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(urlstring, content);
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
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
