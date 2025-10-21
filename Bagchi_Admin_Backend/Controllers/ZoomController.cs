using Azure.Core;
using Bagchi_Admin_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/zoom")]
public class ZoomController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILiveStreamingService _liveStreamingService; // Your service to handle DB/SP

    public string clientId { get; } 
    public string clientSecret   { get; } 

    public string redirectUri { get; }

    public ZoomController(IConfiguration config, IHttpClientFactory httpClientFactory, ILiveStreamingService liveStreamingService)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _liveStreamingService = liveStreamingService;

        clientId = _config["Zoom:ClientId"];
        clientSecret = _config["Zoom:ClientSecret"];
        redirectUri = _config["Zoom:RedirectUri"];
    }

    //public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
    
    //[HttpGet("CreateLiveClasss")]
    //public async Task<IActionResult> CreateMeeting([FromQuery] string state)
    //{
    //    try
    //    {

    //        var IsOngGoingClass = await _liveStreamingService.GetOngoingClasses();   //_liveStreamingService.GetAllCourses();

    //        if (IsOngGoingClass.Count > 0)
    //        {
    //            return BadRequest(new
    //            {
    //                status = 409,
    //                message = "Already a Live Class is going on Please Close it"
    //            });
    //        }

    //        string clientId = _config["Zoom:ClientId"];
    //        string clientSecret = _config["Zoom:ClientSecret"];
    //        string redirectUri = _config["Zoom:RedirectUri"];

    //        var httpClient = _httpClientFactory.CreateClient();
    //        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
    //        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

    //        // Exchange code for access token
    //        var tokenResponse = await httpClient.PostAsync(
    //            $"https://zoom.us/oauth/token?grant_type=authorization_code&code={code}&redirect_uri={redirectUri}",
    //            null);

    //        if (!tokenResponse.IsSuccessStatusCode)
    //            return StatusCode((int)tokenResponse.StatusCode, await tokenResponse.Content.ReadAsStringAsync());

    //        var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
    //        var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenJson);

    //        string accessToken = tokenData.GetProperty("access_token").GetString();
    //        string refreshToken = tokenData.GetProperty("refresh_token").GetString();
    //        int expiresIn = tokenData.GetProperty("expires_in").GetInt32();

    //        // 2️⃣ Create a Zoom meeting
    //        httpClient.DefaultRequestHeaders.Clear();
    //        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

    //        var meetingData = new
    //        {
    //            topic = "Instant Class",       // Can be dynamic from state/course
    //            type = 1,                      // Instant meeting
    //            duration = 60,
    //            timezone = "Asia/Kolkata",
    //            agenda = "Live class session",
    //            settings = new
    //            {
    //                host_video = true,
    //                participant_video = true,
    //                join_before_host = true,
    //                mute_upon_entry = false,
    //                approval_type = 2 // Automatically approve
    //            }
    //        };

    //        var content = new StringContent(JsonSerializer.Serialize(meetingData), Encoding.UTF8, "application/json");
    //        var meetingResponse = await httpClient.PostAsync("https://api.zoom.us/v2/users/me/meetings", content);

    //        if (!meetingResponse.IsSuccessStatusCode)
    //            return StatusCode((int)meetingResponse.StatusCode, await meetingResponse.Content.ReadAsStringAsync());

    //        var meetingJson = await meetingResponse.Content.ReadAsStringAsync();
    //        var meetingInfo = JsonSerializer.Deserialize<JsonElement>(meetingJson);

    //        long meetingId = meetingInfo.GetProperty("id").GetInt64();
    //        string startUrl = meetingInfo.GetProperty("start_url").GetString();
    //        string joinUrl = meetingInfo.GetProperty("join_url").GetString();
 
    //        return Ok(new {meetingid = meetingId,zoom = "success"});

    //      }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, new { error = ex.Message });
    //    }
    //}


   
    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code, string state)
    {
        try
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Zoom code missing");

            // Decode state safely
            string decodedState = System.Net.WebUtility.UrlDecode(state);
            decodedState = decodedState.Replace("&quot;", "\""); // Fix HTML entity
 
            // Exchange code for access token (optional here)
            // ...

            // Return small HTML for popup to send message back to Angular
            string html = $@"
            <html>
            <body>
            <script>
                window.opener.postMessage({{ zoomCode: '{code}', state: '{decodedState}' }}, '*');
                window.close();
            </script>
            </body>
            </html>";

            return Content(html, "text/html");
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("GetZakToken")]
    public async Task<IActionResult> GetZakToken([FromQuery] string accessToken)
    {
        try
        {
            if (string.IsNullOrEmpty(accessToken))
                return BadRequest("Access token is required");

            var httpClient = _httpClientFactory.CreateClient();

            // Set Bearer token
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // Call Zoom API to generate ZAK token for host
            var response = await httpClient.GetAsync("https://api.zoom.us/v2/users/me/token?type=zak");
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { error = err });
            }

            var json = await response.Content.ReadAsStringAsync();
            var zakData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(json);

            string zakToken = zakData.GetProperty("token").GetString();

            return Ok(new
            {
                StatusCode = 200,
                ZAK = zakToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(20) // ZAK tokens are short-lived
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
     


    [HttpPost("CreateLiveClass")]
    public async Task<IActionResult> Callbacks([FromBody] LiveSessionRequest liveSessionRequest)
    {
        try
        { 

            var payloadobj = new LiveSessionRequest();

            payloadobj.topic = liveSessionRequest.topic;
            payloadobj.type = liveSessionRequest.type;
            payloadobj.duration = liveSessionRequest.duration;
            payloadobj.timezone = liveSessionRequest.timezone;
            payloadobj.agenda = liveSessionRequest.agenda;
            payloadobj.host_video = liveSessionRequest.host_video;
            payloadobj.participant_video = liveSessionRequest.participant_video;
            payloadobj.join_before_host = liveSessionRequest.join_before_host;
            payloadobj.mute_upon_entry = liveSessionRequest.mute_upon_entry;
            payloadobj.approval_type = liveSessionRequest.approval_type;
            payloadobj.batchId = liveSessionRequest.batchId;
            payloadobj.CourseId = liveSessionRequest.CourseId;
            payloadobj.zoomcode = liveSessionRequest.zoomcode;

            string clientId = _config["Zoom:ClientId"];
            string clientSecret = _config["Zoom:ClientSecret"];
            string redirectUri = _config["Zoom:RedirectUri"];

            var httpClient = _httpClientFactory.CreateClient();

            // 2️⃣ Exchange code for access token
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

            var tokenResponse = await httpClient.PostAsync(
                $"https://zoom.us/oauth/token?grant_type=authorization_code&code={payloadobj.zoomcode}&redirect_uri={redirectUri}",
                null);

            if (!tokenResponse.IsSuccessStatusCode)
                return StatusCode((int)tokenResponse.StatusCode, await tokenResponse.Content.ReadAsStringAsync());

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(tokenJson);

            string accessToken = tokenData.GetProperty("access_token").GetString();
            string refreshToken = tokenData.GetProperty("refresh_token").GetString();
            int expiresIn = tokenData.GetProperty("expires_in").GetInt32();

            // 3️⃣ Create Zoom meeting using access token
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var meetingData = new
            {
                topic = "Instant Class", // You can pass dynamically from Angular
                type = payloadobj.type, // 1 = Instant meeting
                duration = 60,
                timezone = "Asia/Kolkata",
                agenda = "Live class session",
                settings = new
                {
                    host_video = payloadobj.host_video,
                    participant_video = payloadobj.participant_video,
                    join_before_host = payloadobj.join_before_host,
                    mute_upon_entry = payloadobj.mute_upon_entry,
                    approval_type = payloadobj.approval_type,
                   password= "123"

                }
            };

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(meetingData), Encoding.UTF8, "application/json");
            var meetingResponse = await httpClient.PostAsync("https://api.zoom.us/v2/users/me/meetings", content);

            if (!meetingResponse.IsSuccessStatusCode)
                return StatusCode((int)meetingResponse.StatusCode, await meetingResponse.Content.ReadAsStringAsync());

            var meetingJson = await meetingResponse.Content.ReadAsStringAsync();
            var meetingInfo = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(meetingJson);

            long meetingId = meetingInfo.GetProperty("id").GetInt64();
            string startUrl = meetingInfo.GetProperty("start_url").GetString();
            string joinUrl = meetingInfo.GetProperty("join_url").GetString();

            string meetingPasscode = meetingInfo.TryGetProperty("password", out var pwdProp)    ? pwdProp.GetString()    : null;

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var zakResponse = await httpClient.GetAsync("https://api.zoom.us/v2/users/me/token?type=zak");
            zakResponse.EnsureSuccessStatusCode();

            var zakJson = await zakResponse.Content.ReadAsStringAsync();
            var zakData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(zakJson);
            string zakToken = zakData.GetProperty("token").GetString();

             


            payloadobj.accesstoken = accessToken;
            payloadobj.refreshtoken = refreshToken;
            payloadobj.expiresin = expiresIn;
            payloadobj.meetingpassword = meetingPasscode;
            payloadobj.zaktoken = zakToken;
            payloadobj.Signature = GenerateSignature(meetingId,1);
            payloadobj.teachername = liveSessionRequest.teachername;
            payloadobj.specialClassType = liveSessionRequest.specialClassType;
            payloadobj.studentIds = liveSessionRequest.studentIds;
            //get signature 

            var savemeetingsdetails =   await _liveStreamingService.InsertLiveClass_start(payloadobj, meetingId, startUrl, joinUrl);

            string redirectUrl = $"http://localhost:4200/home/classroom?courseId={payloadobj.CourseId}&zoom=success&meetingid={meetingId}";


            return Ok(new { StatusCode = 200, Message = "Call back success", Url = redirectUrl,meetingid = meetingId });
         }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    } 


    /// <summary>
    /// Generate a Meeting SDK JWT for web usage.
    /// </summary>
    /// <param name="meetingNumber">Zoom meeting number (string or long)</param>
    /// <param name="role">0 = participant, 1 = host</param>
    /// <param name="expirySeconds">How many seconds from now the token should be valid (min 1800, max ~172800)</param>
    /// <returns>JWT string</returns>
    [HttpGet("GetSDJWT")]
    public string GenerateSignature(long meetingNumber, int role = 0, int expirySeconds = 3600)
    {
        role = 1;
        string clientId = _config["Zoom:ClientId"];      // SDK Key
        string clientSecret = _config["Zoom:ClientSecret"]; // SDK Secret

        if (expirySeconds < 1800 || expirySeconds > 172800)
            throw new ArgumentException("expirySeconds must be between 1800 and 172800");

        long iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 30;  // allow clock skew
        long exp = iat + expirySeconds;
        long tokenExp = exp;

        var header = new JwtHeader(
            new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clientSecret)), // ✅ Use secret
                SecurityAlgorithms.HmacSha256
            )
        );

        var payload = new JwtPayload()
    {
        { "appKey", clientId },        // SDK Key
        { "mn", meetingNumber.ToString() },
        { "role", role },
        { "iat", iat },
        { "exp", exp },
        { "tokenExp", tokenExp }
    };

        var token = new JwtSecurityToken(header, payload);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<IActionResult> EndMeeting(long meetingId, string accessToken)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(new { action = "end" }),
            Encoding.UTF8,
            "application/json"
        );

        var response = await httpClient.PutAsync(
            $"https://api.zoom.us/v2/meetings/{meetingId}/status",
            content
        );

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

        return Ok("Meeting ended successfully.");
    }



    [HttpGet("GetMeetingDetails")]
    public async Task<IActionResult> GetMeetingDetails(long MeetingId)
    {
        try
        {

            var result = await _liveStreamingService.GetMeetingDetailsById(MeetingId);
            return Ok(result);

        }catch(Exception ex)
        {

        }

        return BadRequest();
        
    }


     

    [HttpGet("GetActiveMeetingsAsync")] 
     public async Task<JsonElement> GetLiveMeetings(string accessToken)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        string url = "https://api.zoom.us/v2/users/me/meetings?type=live";

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Zoom API Error: {response.StatusCode}, {content}");
        }

        var json = await response.Content.ReadAsStringAsync();
        return System.Text.Json.JsonSerializer.Deserialize<JsonElement>(json);
    }


    

    [HttpPost("meetingstatus_webhook")]
     public async Task<IActionResult> MeetingStatusWebhook([FromBody] dynamic payload)
    {
        try
        {
            string jsonString = Convert.ToString(payload);
            ZoomWebhookEvent zoomEvent = JsonConvert.DeserializeObject<ZoomWebhookEvent>(jsonString);

            if (zoomEvent == null)
                return BadRequest("Invalid payload");

            string eventType = zoomEvent.Event;
            var meeting = zoomEvent.Payload.Object;

            switch (eventType)
            {
                case "meeting.started":
                   await  _liveStreamingService.insertwebhookdata(zoomEvent,eventType);
                    Console.WriteLine($"Meeting Started: {meeting.Topic}, Host: {meeting.HostId}");
                     break;

                case "meeting.ended":
                    await _liveStreamingService.UpdateMeetingStatus(meeting.Id, 3);
                   await  _liveStreamingService.insertwebhookdata(zoomEvent,eventType);
                    Console.WriteLine($"Meeting Ended for All: {meeting.Topic}, Host: {meeting.HostId}");
                         break;

                case "meeting.participant_left":
                    await _liveStreamingService.insertwebhookdata(zoomEvent, eventType);
                    string userId = meeting.UserId;
                    if (userId == meeting.HostId)
                    {
                        Console.WriteLine($"Host Left Meeting: {meeting.Topic}, Host: {meeting.HostId}");
                    }
                    else
                    {
                        Console.WriteLine($"Participant Left: {meeting.Topic}, User: {meeting.UserName}");
                    }
                    break;

                case "meeting.participant_joined":
                    Console.WriteLine($"Participant Joined: {meeting.Topic}, User: {meeting.UserName}");
                    break;
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing webhook: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    private readonly string _baseUrl = "https://api.zoom.us/v2";

    [HttpPost("GetLiveMeetingsAsync")]
    public async Task<List<MeetingInfo>> GetLiveMeetingsAsync(string userId, string accessToken)
    {
        // Create a new HttpClient from the factory
        var client = _httpClientFactory.CreateClient();

        var url = $"{_baseUrl}/users/{userId}/meetings?type=live&page_size=30";
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var resp = await client.SendAsync(req);
        if (!resp.IsSuccessStatusCode)
        {
            var err = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Zoom API error: {resp.StatusCode} - {err}");
        }

        var json = await resp.Content.ReadAsStringAsync();
        var obj = JsonConvert.DeserializeObject<ZoomListMeetingsResponse>(json);

        return obj?.Meetings ?? new List<MeetingInfo>();
    }

    [HttpPost("EndMeetingAsync")]
    public async Task EndMeetingAsync(long meetingId, string accessToken)
    {
        var client = _httpClientFactory.CreateClient();

        var url = $"{_baseUrl}/meetings/{meetingId}/status";
        var req = new HttpRequestMessage(HttpMethod.Put, url);
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var payload = new { action = "end" };
        req.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        var resp = await client.SendAsync(req);
        if (!resp.IsSuccessStatusCode)
        {
            var err = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to end meeting: {resp.StatusCode} - {err}");
        }
    }


    public class ZoomListMeetingsResponse
    {
        [JsonProperty("page_count")] public int PageCount { get; set; }
        [JsonProperty("page_size")] public int PageSize { get; set; }
        [JsonProperty("total_records")] public int TotalRecords { get; set; }
        [JsonProperty("meetings")] public List<MeetingInfo> Meetings { get; set; }
    }

    public class MeetingInfo
    {
        [JsonProperty("id")] public long Id { get; set; }
        [JsonProperty("topic")] public string Topic { get; set; }
        [JsonProperty("start_time")] public string StartTime { get; set; }
        // add other fields as needed
    }

    //[HttpPost("GetAccessToken")]
    //public async Task<ZoomTokenResponse> GetAccessTokenAsync(string authorizationCode)
    //{
    //    var httpClient = _httpClientFactory.CreateClient();
    //    string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
    //    var request = new HttpRequestMessage(HttpMethod.Post, "https://zoom.us/oauth/token");
    //    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
    //    request.Content = new FormUrlEncodedContent(new[] {
    //        new KeyValuePair<string, string>("grant_type", "authorization_code"), 
    //        new KeyValuePair<string, string>("code", authorizationCode), 
    //        new KeyValuePair<string, string>("redirect_uri", redirectUri) });
    //    var response = await httpClient.SendAsync(request); 
    //    var content = await response.Content.ReadAsStringAsync(); 
    //    if (!response.IsSuccessStatusCode) 
    //    { throw new Exception($"Failed to get access token: {response.StatusCode} {content}"); } 
    //    return JsonConvert.DeserializeObject<ZoomTokenResponse>(content); 
    //}


    //public class ZoomTokenResponse { 
    //    [JsonProperty("access_token")] public string AccessToken { get; set; } 
    //    [JsonProperty("token_type")] public string TokenType { get; set; }
    //    [JsonProperty("refresh_token")] public string RefreshToken { get; set; } 
    //    [JsonProperty("expires_in")] public int ExpiresIn { get; set; }
    //    [JsonProperty("scope")] public string Scope { get; set; }
    //}


    [HttpPost("RefreshAccessToken")]
    public async Task<IActionResult> RefreshAccessToken([FromBody]  string  refreshTokenRequest)
    {
        try
        {
            string clientId = _config["Zoom:ClientId"];
            string clientSecret = _config["Zoom:ClientSecret"];
            string refreshToken = refreshTokenRequest;

            var httpClient = _httpClientFactory.CreateClient();

            // Prepare the authorization header
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

            // Prepare the request body
            var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken }
        };

            var content = new FormUrlEncodedContent(requestBody);

            // Send the request to Zoom's token endpoint
            var tokenResponse = await httpClient.PostAsync("https://zoom.us/oauth/token", content);

            if (!tokenResponse.IsSuccessStatusCode)
                return StatusCode((int)tokenResponse.StatusCode, await tokenResponse.Content.ReadAsStringAsync());

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(tokenJson);

            string newAccessToken = tokenData.GetProperty("access_token").GetString();
            string newRefreshToken = tokenData.GetProperty("refresh_token").GetString();
            int expiresIn = tokenData.GetProperty("expires_in").GetInt32();

            // Update the stored tokens in your database
          //  await _liveStreamingService.UpdateTokens(refreshTokenRequest.UserId, newAccessToken, newRefreshToken, expiresIn);

            return Ok(new
            {
                StatusCode = 200,
                Message = "Access token refreshed successfully",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = expiresIn
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
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
        catch (Exception ex)
        {
            return StatusCode(500, new { Status = 500, Message = "Internal Server Error: " + ex.Message });


        }
    }



    [HttpGet("recordings")]
    public async Task<IActionResult> GetRecording(string meetingId,string accesstoken)
    {
        // Replace with your OAuth access token
        var accessToken = accesstoken;

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var url = $"https://api.zoom.us/v2/meetings/{meetingId}/recordings";

        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(new { message = "Failed to fetch Zoom recording", error });
        }

        var content = await response.Content.ReadAsStringAsync();
        var data = JObject.Parse(content); // JSON response
        return Ok(data);
    }

    //[HttpPost("get-access-token")]
    //public async Task<IActionResult> GetAccessToken([FromBody] ZoomAuthRequest request)
    //{
    //    if (string.IsNullOrEmpty(request.Code))
    //        return BadRequest("Authorization code is required.");

    //    string clientId = _config["Zoom:ClientId"];
    //    string clientSecret = _config["Zoom:ClientSecret"];
    //    string redirectUri = _config["Zoom:RedirectUri"];

    //    var httpClient = _httpClientFactory.CreateClient();
    //    var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
    //    httpClient.DefaultRequestHeaders.Authorization =
    //        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

    //    var tokenResponse = await httpClient.PostAsync(
    //        $"https://zoom.us/oauth/token?grant_type=authorization_code&code={request.Code}&redirect_uri={redirectUri}",
    //        null
    //    );

    //    var content = await tokenResponse.Content.ReadAsStringAsync();

    //    if (!tokenResponse.IsSuccessStatusCode)
    //        return StatusCode((int)tokenResponse.StatusCode, content);

    //    return Ok(content);
    //}

    [HttpGet("GetClassHistory")]

    public async Task<IActionResult> GetClassHistory()
    {
        try
        {
            var result = await  _liveStreamingService.GetClassHistoryAsync();
            return Ok(result);

        }catch(Exception ex)
        {
            return Ok(new { statuscode = "500", message = "Error getting details" });

        }
    }

}



