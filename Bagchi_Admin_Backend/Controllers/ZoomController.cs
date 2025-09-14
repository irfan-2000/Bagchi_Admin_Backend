using Azure.Core;
using Bagchi_Admin_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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
    public ZoomController(IConfiguration config, IHttpClientFactory httpClientFactory, ILiveStreamingService liveStreamingService)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _liveStreamingService = liveStreamingService;
    }

    [HttpGet("CreateMeeting")]
    //public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
    public async Task<IActionResult> CreateMeeting([FromQuery] string code, [FromQuery] string state)
    {
        try
        {
            string clientId = _config["Zoom:ClientId"];
            string clientSecret = _config["Zoom:ClientSecret"];
            string redirectUri = _config["Zoom:RedirectUri"];

            var httpClient = _httpClientFactory.CreateClient();
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

            // Exchange code for access token
            var tokenResponse = await httpClient.PostAsync(
                $"https://zoom.us/oauth/token?grant_type=authorization_code&code={code}&redirect_uri={redirectUri}",
                null);

            if (!tokenResponse.IsSuccessStatusCode)
                return StatusCode((int)tokenResponse.StatusCode, await tokenResponse.Content.ReadAsStringAsync());

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenJson);

            string accessToken = tokenData.GetProperty("access_token").GetString();
            string refreshToken = tokenData.GetProperty("refresh_token").GetString();
            int expiresIn = tokenData.GetProperty("expires_in").GetInt32();

            // 2️⃣ Create a Zoom meeting
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var meetingData = new
            {
                topic = "Instant Class",       // Can be dynamic from state/course
                type = 1,                      // Instant meeting
                duration = 60,
                timezone = "Asia/Kolkata",
                agenda = "Live class session",
                settings = new
                {
                    host_video = true,
                    participant_video = true,
                    join_before_host = true,
                    mute_upon_entry = false,
                    approval_type = 2 // Automatically approve
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(meetingData), Encoding.UTF8, "application/json");
            var meetingResponse = await httpClient.PostAsync("https://api.zoom.us/v2/users/me/meetings", content);

            if (!meetingResponse.IsSuccessStatusCode)
                return StatusCode((int)meetingResponse.StatusCode, await meetingResponse.Content.ReadAsStringAsync());

            var meetingJson = await meetingResponse.Content.ReadAsStringAsync();
            var meetingInfo = JsonSerializer.Deserialize<JsonElement>(meetingJson);

            long meetingId = meetingInfo.GetProperty("id").GetInt64();
            string startUrl = meetingInfo.GetProperty("start_url").GetString();
            string joinUrl = meetingInfo.GetProperty("join_url").GetString();

            // 3️⃣ Save to database via stored procedure
            //await _liveStreamingService.SaveZoomMeeting(
            //    state,          // e.g., courseId passed in state
            //    meetingId,
            //    startUrl,
            //    joinUrl,
            //    accessToken,
            //    refreshToken,
            //    DateTime.UtcNow.AddSeconds(expiresIn)
            //);

            // 4️⃣ Redirect frontend to classroom page with success flag
            string redirectUrl = $"http://localhost:4200/home/classroom?courseId={state}&zoom=success";
            return Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }


     //public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state = "")
    //{
    //    if (string.IsNullOrEmpty(code))
    //        return BadRequest(new { error = "Authorization code is missing" });

    //    try
    //    {
    //        string clientId = _config["Zoom:ClientId"];
    //        string clientSecret = _config["Zoom:ClientSecret"];
    //        string redirectUri = "https://localhost:7091/api/zoom/callback"; // must match Zoom App settings

    //        var httpClient = _httpClientFactory.CreateClient();
    //        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
    //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

    //        // 🔑 Exchange code for access token
    //        var response = await httpClient.PostAsync(
    //            $"https://zoom.us/oauth/token?grant_type=authorization_code&code={code}&redirect_uri={redirectUri}",
    //            null);

    //        var content = await response.Content.ReadAsStringAsync();

    //        if (!response.IsSuccessStatusCode)
    //            return StatusCode((int)response.StatusCode, content);

    //        var tokenData = JsonSerializer.Deserialize<JsonElement>(content);

    //        return Ok(new
    //        {
    //            message = "Zoom OAuth Success",
    //            data = tokenData
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, new { error = ex.Message });
    //    }
    //}

     

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
    {
        try
        {
            // 1️⃣ Parse courseId and batchId from state
            string decodedState = System.Net.WebUtility.HtmlDecode(state);



 
            // 2️⃣ Parse as JsonDocument
            using var doc = JsonDocument.Parse(decodedState);

            var payload = doc.RootElement.GetProperty("payload");

            var payloadobj = new LiveSessionRequest();

            payloadobj.topic = payload.GetProperty("topic").GetString();
            payloadobj.type = payload.GetProperty("type").GetInt32();
            payloadobj.duration  = payload.GetProperty("duration").GetInt32();
             payloadobj.timezone = payload.GetProperty("timezone").GetString();
            payloadobj.agenda = payload.GetProperty("agenda").GetString();
            payloadobj. host_video = payload.GetProperty("host_video").GetBoolean();
             payloadobj.participant_video = payload.GetProperty("participant_video").GetBoolean();
             payloadobj. join_before_host = payload.GetProperty("join_before_host").GetBoolean();
             payloadobj. mute_upon_entry = payload.GetProperty("mute_upon_entry").GetBoolean();
             payloadobj. approval_type = payload.GetProperty("approval_type").GetInt32();
             payloadobj. batchId = payload.GetProperty("batchId").GetInt32().ToString();
            payloadobj.CourseId = payload.GetProperty("courseId").GetInt32().ToString();
            // Deserialize JSON

            var stateObj = JsonSerializer.Deserialize<LiveSessionRequest>(decodedState);
            string clientId = _config["Zoom:ClientId"];
            string clientSecret = _config["Zoom:ClientSecret"];
            string redirectUri = _config["Zoom:RedirectUri"];

            var httpClient = _httpClientFactory.CreateClient();

            // 2️⃣ Exchange code for access token
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

            var tokenResponse = await httpClient.PostAsync(
                $"https://zoom.us/oauth/token?grant_type=authorization_code&code={code}&redirect_uri={redirectUri}",
                null);

            if (!tokenResponse.IsSuccessStatusCode)
                return StatusCode((int)tokenResponse.StatusCode, await tokenResponse.Content.ReadAsStringAsync());

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenJson);

            string accessToken = tokenData.GetProperty("access_token").GetString();
            string refreshToken = tokenData.GetProperty("refresh_token").GetString();
            int expiresIn = tokenData.GetProperty("expires_in").GetInt32();

            // 3️⃣ Create Zoom meeting using access token
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var meetingData = new
            {
                topic = "Instant Class", // You can pass dynamically from Angular
                type = 1, // 1 = Instant meeting
                duration = 60,
                timezone = "Asia/Kolkata",
                agenda = "Live class session",
                settings = new
                {
                    host_video = true,
                    participant_video = true,
                    join_before_host = true,
                    mute_upon_entry = false,
                    approval_type = 2,
                   password= "123"

                }
            };

            var content = new StringContent(JsonSerializer.Serialize(meetingData), Encoding.UTF8, "application/json");
            var meetingResponse = await httpClient.PostAsync("https://api.zoom.us/v2/users/me/meetings", content);

            if (!meetingResponse.IsSuccessStatusCode)
                return StatusCode((int)meetingResponse.StatusCode, await meetingResponse.Content.ReadAsStringAsync());

            var meetingJson = await meetingResponse.Content.ReadAsStringAsync();
            var meetingInfo = JsonSerializer.Deserialize<JsonElement>(meetingJson);

            long meetingId = meetingInfo.GetProperty("id").GetInt64();
            string startUrl = meetingInfo.GetProperty("start_url").GetString();
            string joinUrl = meetingInfo.GetProperty("join_url").GetString();

            // 4️⃣ Save everything to database
            //await _liveStreamingService.SaveZoomMeeting(
            //    stateObj.CourseId,
            //    stateObj.BatchId,
            //    meetingId,
            //    startUrl,
            //    joinUrl,
            //    accessToken,
            //    refreshToken,
            //    DateTime.UtcNow.AddSeconds(expiresIn)
            //);
            string meetingPasscode = meetingInfo.TryGetProperty("password", out var pwdProp)
    ? pwdProp.GetString()
    : null;


            payloadobj.accesstoken = accessToken;
            payloadobj.refreshtoken = refreshToken;
            payloadobj.expiresin = expiresIn;
            payloadobj.meetingpassword = meetingPasscode;

            var savemeetingsdetails =   await _liveStreamingService.InsertLiveClass_start(payloadobj, meetingId, startUrl, joinUrl);


            // 5️⃣ Redirect to Angular classroom page
            string redirectUrl = $"http://localhost:4200/home/classroom?courseId={stateObj.CourseId}&zoom=success";
            return Redirect(redirectUrl);
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
}