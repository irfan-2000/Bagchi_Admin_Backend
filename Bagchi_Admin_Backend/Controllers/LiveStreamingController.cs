using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Bagchi_Admin_Backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Azure.Core;
using System.IdentityModel.Tokens.Jwt;

namespace Bagchi_Admin_Backend.Controllers
{
    [ApiController]
    [Route("api/")]
    public class LiveStreamingController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILiveStreamingService _liveStreamingService;

        public LiveStreamingController(IConfiguration configuration, 
            IHttpClientFactory httpClientFactory,ILiveStreamingService liveStreamingService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _liveStreamingService = liveStreamingService;
        }


      
        private async Task<IActionResult> CreateLiveSession([FromBody] LiveSessionRequest request)
        {
            try
            {

                var IsOngGoingClass = await _liveStreamingService.GetOngoingClasses();   //_liveStreamingService.GetAllCourses();

                if(IsOngGoingClass.Count > 0)
                {
                    return BadRequest(new
                    {
                        status = 409,
                        message = "Already a Live Class is going on Please Close it"
                    });
                }


                // 1️⃣ Get OAuth Token
                string clientId = _configuration["Zoom:ClientId"];
                string clientSecret = _configuration["Zoom:ClientSecret"];
                string accountId = _configuration["Zoom:AccountId"];

                var httpClient = _httpClientFactory.CreateClient();
                var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

                var tokenResponse = await httpClient.PostAsync(
                    $"https://zoom.us/oauth/token?grant_type=account_credentials&account_id={accountId}", null);

                if (!tokenResponse.IsSuccessStatusCode)
                    return StatusCode((int)tokenResponse.StatusCode, await tokenResponse.Content.ReadAsStringAsync());

                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenJson);
                string accessToken = tokenData.GetProperty("access_token").GetString();

                // 2️⃣ Create Meeting
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var meetingData = new
                {
                    topic = request.topic ?? "Biology Instant Class",
                    type = request.type, // 1 = Instant, 2 = Scheduled
                    duration = request.duration,
                    timezone = request.timezone ?? "Asia/Kolkata",
                    agenda = request.agenda ?? "Instant test session",
                    settings = new
                    {
                        host_video = request.host_video,
                        participant_video = request.participant_video,
                        join_before_host = request.join_before_host,
                        mute_upon_entry = request.mute_upon_entry,
                        approval_type = request.approval_type
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(meetingData), Encoding.UTF8, "application/json");
                var meetingResponse = await httpClient.PostAsync("https://api.zoom.us/v2/users/me/meetings", content);

                if (!meetingResponse.IsSuccessStatusCode)
                    return StatusCode((int)meetingResponse.StatusCode, await meetingResponse.Content.ReadAsStringAsync());

                var meetingJson = await meetingResponse.Content.ReadAsStringAsync();
                var meetingInfo = JsonSerializer.Deserialize<JsonElement>(meetingJson);

                 string startUrl = meetingInfo.GetProperty("start_url").GetString();
                string joinUrl = meetingInfo.GetProperty("join_url").GetString();
                long meetingId = meetingInfo.GetProperty("id").GetInt64();


                var result = await _liveStreamingService.InsertLiveClass_start(request, meetingId, startUrl, joinUrl);



                if(!Convert.ToBoolean(result))
                {
                    return BadRequest(new
                    {
                        status = 400,
                        error = "Failed to save meeting details"
                    });
                }


                 return Ok(new
                {
                    status = 200,
                    message = "Meeting created successfully",
                    meeting_id = meetingId,
                    start_url = startUrl,
                    join_url = joinUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, error = ex.Message });
            }
        }



        [HttpGet("zoom/authorize")]
        public IActionResult ZoomAuthorize()
        {
            string clientId = _configuration["Zoom:ClientId"];
            string redirectUri = _configuration["Zoom:RedirectUri"];
            string scope = "meeting:write:admin"; // Required scope
            string authorizeUrl = $"https://zoom.us/oauth/authorize?response_type=code&client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={scope}";

            return Redirect(authorizeUrl);
        }



        
        private async Task<IActionResult> ZoomCallback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Authorization code is missing.");

            string clientId = _configuration["Zoom:ClientId"];
            string clientSecret = _configuration["Zoom:ClientSecret"];
            string redirectUri = _configuration["Zoom:RedirectUri"];

            var httpClient = _httpClientFactory.CreateClient();
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            var content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("grant_type", "authorization_code"),
        new KeyValuePair<string, string>("code", code),
        new KeyValuePair<string, string>("redirect_uri", redirectUri)
    });

            var tokenResponse = await httpClient.PostAsync("https://zoom.us/oauth/token", content);
            if (!tokenResponse.IsSuccessStatusCode)
                return StatusCode((int)tokenResponse.StatusCode, await tokenResponse.Content.ReadAsStringAsync());

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenJson);
            string accessToken = tokenData.GetProperty("access_token").GetString();

            // You can store this access token in DB/session for creating meetings
            return Ok(new { accessToken });
        }






        [HttpPost("zoom/create-meeting")]
        public async Task<IActionResult> CreateZoomMeeting([FromBody] LiveSessionRequest request, [FromQuery] string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return BadRequest("Access token is required.");

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var meetingData = new
            {
                topic = request.topic ?? "Biology Instant Class",
                type = request.type, // 1 = Instant, 2 = Scheduled
                duration = request.duration,
                timezone = request.timezone ?? "Asia/Kolkata",
                agenda = request.agenda ?? "Instant test session",
                settings = new
                {
                    host_video = request.host_video,
                    participant_video = request.participant_video,
                    join_before_host = request.join_before_host,
                    mute_upon_entry = request.mute_upon_entry,
                    approval_type = request.approval_type
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(meetingData), Encoding.UTF8, "application/json");
            var meetingResponse = await httpClient.PostAsync("https://api.zoom.us/v2/users/me/meetings", content);

            if (!meetingResponse.IsSuccessStatusCode)
                return StatusCode((int)meetingResponse.StatusCode, await meetingResponse.Content.ReadAsStringAsync());

            var meetingJson = await meetingResponse.Content.ReadAsStringAsync();
            var meetingInfo = JsonSerializer.Deserialize<JsonElement>(meetingJson);

            string startUrl = meetingInfo.GetProperty("start_url").GetString();
            string joinUrl = meetingInfo.GetProperty("join_url").GetString();
            long meetingId = meetingInfo.GetProperty("id").GetInt64();

            // Optional: Save meeting info to DB
            var result = await _liveStreamingService.InsertLiveClass_start(request, meetingId, startUrl, joinUrl);

            if (!Convert.ToBoolean(result))
                return BadRequest(new { status = 400, error = "Failed to save meeting details" });

            return Ok(new
            {
                status = 200,
                message = "Meeting created successfully",
                meeting_id = meetingId,
                start_url = startUrl,
                join_url = joinUrl
            });
        }






        [HttpGet("GetAllCourses")]
        public async  Task<IActionResult> GetAllCourses()
        {
            try
            {
              var result = await  _liveStreamingService.GetAllCourses();   //_liveStreamingService.GetAllCourses();

                if (result != null && result.Any())
                {
                    return Ok(new { Status = 200, Message = "Success", Result = result });
                }
                else
                {
                    return Ok(new { Status = 204, Message = "No subjects found", Result = (object)null });
                }
            }
            catch (Exception ex)
            {
                string errorMessage = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Unknown error";

                return StatusCode(500, new { Status = 500, Message = "Internal Server Error: " + errorMessage });
            }
        }

        [HttpGet("GetOnGoingClasses")]
        public async Task<IActionResult> GetOnGoingClasses()
        {
            try
            {
                var result = await _liveStreamingService.GetOngoingClasses();   //_liveStreamingService.GetAllCourses();

                if (result != null && result.Any())
                {
                    return Ok(new { Status = 200, Message = "Success", Result = result });
                }
                else
                {
                    return Ok(new { Status = 204, Message = "No classes found", Result = (object)null });
                } 

            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Status = 500, Message = "Internal Server Error: " + ex.Message });


            }
        }



        [HttpGet("GetBatchesById")]
        public async Task<IActionResult>GetBatchesById(string CourseId)
        {
            try
            {
                var result = await _liveStreamingService.GetBatchesById(CourseId);   //_liveStreamingService.GetAllCourses();

                if (result != null && result.Any())
                {
                    return Ok(new { Status = 200, Message = "Success", Result = result });
                }
                else
                {
                    return Ok(new { Status = 204, Message = "No classes found", Result = (object)null });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Message = "Internal Server Error: " + ex.Message });


            }


        } 
     
        
        
        [HttpGet("generateSignature")]
        public IActionResult GenerateSignature(string meetingNumber, int role)
        {
            try
            {
                var signature = _liveStreamingService.GenerateSdkSignature(meetingNumber, role);
                return Ok(new { signature });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpGet("getZak")]
         public async Task<IActionResult> GetZak(string email)
        {
            try
            {
                string clientId = _configuration["Zoom:ClientId"];
                string clientSecret = _configuration["Zoom:ClientSecret"];
                string accountId = _configuration["Zoom:AccountId"];

                var httpClient = _httpClientFactory.CreateClient();

                // Step 1: Get access token from Zoom
                var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

                var tokenResponse = await httpClient.PostAsync(
                    $"https://zoom.us/oauth/token?grant_type=account_credentials&account_id={accountId}",
                    null
                );

                if (!tokenResponse.IsSuccessStatusCode)
                    return StatusCode((int)tokenResponse.StatusCode, await tokenResponse.Content.ReadAsStringAsync());

                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(tokenJson);
                string accessToken = doc.RootElement.GetProperty("access_token").GetString();

                // Step 2: Use access token to get ZAK token for host
                var zakClient = _httpClientFactory.CreateClient();
                zakClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var zakResponse = await zakClient.GetAsync($"https://api.zoom.us/v2/users/{email}/token?type=zak");

                var zakJson = await zakResponse.Content.ReadAsStringAsync();
                if (!zakResponse.IsSuccessStatusCode)
                    return StatusCode((int)zakResponse.StatusCode, zakJson);

                return Content(zakJson, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }










    }



}







