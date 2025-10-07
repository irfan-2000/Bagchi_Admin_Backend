using Azure.Core;
using Bagchi_Admin_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bagchi_Admin_Backend.Services
{
    public class LiveStreamingService : ILiveStreamingService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _config;

        public static String GlobalFetchPath;


        public string GlobalFetchMediaPath { get; }
        public LiveStreamingService(ApplicationDbContext dbContext, IConfiguration Iconfiguration)
        {
            _dbContext = dbContext;
            _config = Iconfiguration;
            GlobalFetchMediaPath = _config["GlobalFetchMediaPath"];
            GlobalFetchPath = _config["GlobalFetchPath"];

        }



        public async Task<List<LiveSession>> GetOngoingClasses()
        {
            List<LiveSession> classes = new List<LiveSession>();

            try
            {

                using (var cmd= _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "Get_OnGoingLiveClassDetails";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();


                    await _dbContext.Database.OpenConnectionAsync();


                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            LiveSession session = new LiveSession();

                            session.LiveSessionId = reader["LiveSessionId"]?.ToString() ?? "";
                            session.BatchId = reader["BatchId"]?.ToString() ?? "";
                            session.CourseId = reader["CourseId"]?.ToString() ?? "";
                            session.Topic = reader["Topic"]?.ToString() ?? "";
                            session.Agenda = reader["Agenda"]?.ToString() ?? "";
                            session.StartTime = reader["StartTime"]?.ToString() ?? "";
                            session.EndTime = reader["EndTime"]?.ToString() ?? "";
                            session.Duration = reader["Duration"]?.ToString() ?? "";
                            session.ZoomMeetingId = reader["ZoomMeetingId"]?.ToString() ?? "";
                            session.ZoomStartUrl = reader["ZoomStartUrl"]?.ToString() ?? "";
                            session.ZoomJoinUrl = reader["ZoomJoinUrl"]?.ToString() ?? "";
                            session.ZoomPassword = reader["ZoomPassword"]?.ToString() ?? "";
                            session.HostVideo = reader["HostVideo"]?.ToString() ?? "";
                            session.ParticipantVideo = reader["ParticipantVideo"]?.ToString() ?? "";
                            session.JoinBeforeHost = reader["JoinBeforeHost"]?.ToString() ?? "";
                            session.MuteUponEntry = reader["MuteUponEntry"]?.ToString() ?? "";
                            session.ApprovalType = reader["ApprovalType"]?.ToString() ?? "";
                            session.Status = reader["Status"]?.ToString() ?? "";
                            session.CreatedAt = reader["CreatedAt"]?.ToString() ?? "";
                            session.UpdatedAt = reader["UpdatedAt"]?.ToString() ?? "";

                            classes.Add(session);
                        }

                    }


                } 
            }
            catch (Exception ex)
            {
                throw new Exception("error" + ex.Message);
            }
            return classes;

        }


       public async Task<List<BatchDetails>> GetBatchesById(string CourseId)
        {
            List<BatchDetails> batches = new List<BatchDetails>();

            try
            {

                using (var cmd= _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "GetbatchesById";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();


                    DbHelper.AddParameter(cmd, "@CourseId", CourseId);



                    await _dbContext.Database.OpenConnectionAsync();
                     
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            BatchDetails batch = new BatchDetails();

                            batch.BatchId = reader["BatchId"]?.ToString() ?? "";
                            batch.BatchName = reader["BatchName"]?.ToString() ?? "";
                            batch.ClassId = reader["ClassId"]?.ToString() ?? "";
                            batch.SubjectId = reader["SubjectId"]?.ToString() ?? "";
                            batch.BoardId = reader["BoardId"]?.ToString() ?? "";
                            batch.StartDate = reader["StartDate"]?.ToString() ?? "";
                            batch.EndDate = reader["EndDate"]?.ToString() ?? "";
                            batch.Status = reader["Status"]?.ToString() ?? "";
                            batch.CourseId = reader["CourseId"]?.ToString() ?? "";
                            batch.CreatedAt = reader["CreatedAt"]?.ToString() ?? "";
                            batch.UpdatedAt = reader["UpdatedAt"]?.ToString() ?? "";
                            batch.StartTime = reader["StartTime"]?.ToString() ?? "";
                            batch.EndTime = reader["EndTime"]?.ToString() ?? "";


                            batches.Add(batch);
                        }

                    }


                } 
            }
            catch (Exception ex)
            {
                throw new Exception("error" + ex.Message);

            }
            return batches;

        }


        public async Task<bool> InsertLiveClass_start(LiveSessionRequest request,long meetingId,string startUrl ,string joinUrl)
        {
            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_ManageLiveSession";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();


                    DbHelper.AddParameter(cmd, "@CourseId", Convert.ToInt32(request.CourseId));
                    DbHelper.AddParameter(cmd, "@BatchId", Convert.ToInt32(request.batchId));
                    DbHelper.AddParameter(cmd, "@Topic", request.topic);
                    DbHelper.AddParameter(cmd, "@ZoomMeetingId", meetingId);
                    DbHelper.AddParameter(cmd, "@ZoomStartUrl", startUrl);
                    DbHelper.AddParameter(cmd, "@ZoomJoinUrl", joinUrl);
                    DbHelper.AddParameter(cmd, "@HostVideo", Convert.ToByte(request.host_video));
                    DbHelper.AddParameter(cmd, "@ParticipantVideo", Convert.ToByte(request.participant_video));
                    DbHelper.AddParameter(cmd, "@JoinBeforeHost", Convert.ToByte(request.join_before_host));
                    DbHelper.AddParameter(cmd, "@MuteUponEntry", Convert.ToByte(request.mute_upon_entry));
                    DbHelper.AddParameter(cmd, "@ApprovalType", request.approval_type);
                    DbHelper.AddParameter(cmd, "@Status", request.type);
                    DbHelper.AddParameter(cmd, "@accesstoken", request.accesstoken);
                    DbHelper.AddParameter(cmd, "@refreshtoken", request.refreshtoken);
                    DbHelper.AddParameter(cmd, "@expiresin", request.expiresin);
                    DbHelper.AddParameter(cmd, "@meetingpassword", request.meetingpassword);
                    DbHelper.AddParameter(cmd, "@zaktoken", request.zaktoken);
                     DbHelper.AddParameter(cmd, "@signature", request.Signature);
                    DbHelper.AddParameter(cmd, "teachername", request.teachername);
                    DbHelper.AddParameter(cmd, "@Action", "INSERT"); 

                    await _dbContext.Database.OpenConnectionAsync(); 

                    var result = await cmd.ExecuteNonQueryAsync();

                    if(result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;

                    }

                }


            }
            catch(Exception ex)
            {

            }

            return false;


        }
         


  
        private readonly string sdkKey = "zQJMdPLnTZCvtE70iMLcSA"; // Your SDK Key
        private readonly string sdkSecret = "UPtL6W9bySR4J7i23Juy8G6OAj96XfCT"; // Your SDK Secret

        public string GenerateSdkSignature(string meetingNumber, int role)
        {
            meetingNumber = "84914005384";

            long iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 30;
            long exp = iat + 60 * 60 * 2; // Signature valid for 2 hours

            var claims = new List<Claim>
        {
            new Claim("appKey", sdkKey),
            new Claim("mn", meetingNumber),
            new Claim("role", role.ToString()),
            new Claim("iat", "1646937553"),
            new Claim("exp", "1646944753"),
            new Claim("tokenExp", "1646944753")
        };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sdkSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(credentials);
            var payload = new JwtPayload(claims);
            var token = new JwtSecurityToken(header, payload);

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            // Validate JWT structure
            if (!IsValidJwt(jwt))
            {
                throw new InvalidOperationException("Invalid JWT structure.");
            }

            // Validate timestamps
            ValidateTimestamps(iat, exp);

            return jwt;
        }

        private bool IsValidJwt(string jwt)
        {
            try
            {
                var parts = jwt.Split('.');
                if (parts.Length != 3) return false;

                var header = parts[0];
                var payload = parts[1];
                var signature = parts[2];

                // Decode Base64Url safely
                string Base64UrlDecode(string input)
                {
                    string base64 = input.Replace('-', '+').Replace('_', '/');
                    switch (base64.Length % 4)
                    {
                        case 2: base64 += "=="; break;
                        case 3: base64 += "="; break;
                    }
                    return base64;
                }

                var decodedHeader = Encoding.UTF8.GetString(Convert.FromBase64String(Base64UrlDecode(header)));
                var decodedPayload = Encoding.UTF8.GetString(Convert.FromBase64String(Base64UrlDecode(payload)));

                // Optional: you can parse payload as JSON and validate claims here
                // e.g., check exp, iss, aud, etc.

                return true;
            }
            catch
            {
                // Any error in decoding means JWT is invalid
                return false;
            }
        }


        private void ValidateTimestamps(long iat, long exp)
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (exp - iat < 1800)
            {
                throw new InvalidOperationException("Expiration time must be at least 30 minutes after issued time.");
            }

            if (exp - iat > 172800)
            {
                throw new InvalidOperationException("Expiration time must not exceed 48 hours after issued time.");
            }

            if (iat > currentTime)
            {
                throw new InvalidOperationException("Issued time cannot be in the future.");
            }

            if (exp < currentTime)
            {
                throw new InvalidOperationException("Expiration time cannot be in the past.");
            }
        }


        public static byte[] Base64UrlDecode(string input)
        {
            string base64 = input
                .Replace('-', '+')
                .Replace('_', '/');

            // Add padding if missing
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }


        public async Task<MeetingDetails> GetMeetingDetailsById(long MeetingId)
        {
            var details = new MeetingDetails();

            try 
            {

                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "usp_GetMeetingDetails";

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    DbHelper.AddParameter(cmd, "@MeetingId", MeetingId);

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                             details.Signature = reader["Signature"]?.ToString() ?? "";
                            details.ZoomPassword = reader["ZoomPassword"]?.ToString() ?? "";
                            details.TeacherName = reader["TeacherName"]?.ToString() ?? "";
                            details.ZakToken = reader["ZakToken"]?.ToString() ?? "";
                            details.MeetingPassword = reader["MeetingPassword"]?.ToString() ?? "";

                         }

                    }
                } 
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching meeting details: " + ex.Message, ex);
            }

            return details;
        }





    }




}













 



 