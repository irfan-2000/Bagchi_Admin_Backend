using Bagchi_Admin_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Bagchi_Admin_Backend.Services
{
    public class LiveStreamingService : ILiveStreamingService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _Iconfiguration;

        public static String GlobalFetchPath;


        public string GlobalFetchMediaPath { get; }
        public LiveStreamingService(ApplicationDbContext dbContext, IConfiguration Iconfiguration)
        {
            _dbContext = dbContext;
            _Iconfiguration = Iconfiguration;
            GlobalFetchMediaPath = _Iconfiguration["GlobalFetchMediaPath"];
            GlobalFetchPath = _Iconfiguration["GlobalFetchPath"];

        }


        public async Task<List<AllCourseDetails>> GetAllCourses()
        {
            List<AllCourseDetails> allCourses = new List<AllCourseDetails>();

            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_Manage_CourseDetails";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    DbHelper.AddParameter(cmd, "@From", "U");
                    DbHelper.AddParameter(cmd, "@Flag", "GA");

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AllCourseDetails data = new AllCourseDetails();

                            data.OldPrice = reader["OldPrice"]?.ToString() ?? "";
                            data.DetailId = reader["DetailId"]?.ToString() ?? "";
                            data.CourseId = reader["CourseId"]?.ToString() ?? "";
                            data.CourseName = reader["CourseName"]?.ToString() ?? "";
                            data.Price = reader["Price"]?.ToString() ?? "";
                            data.Status = reader["Status"]?.ToString() ?? "";
                            data.Description = reader["Description"]?.ToString() ?? "";
                            data.Objectives = reader["Objectives"]?.ToString() ?? "";
                            data.Requirements = reader["Requirements"]?.ToString() ?? "";
                            data.CourseLevel = reader["CourseLevel"]?.ToString() ?? "";
                            data.CreatedAt = reader["CreatedAt"]?.ToString() ?? "";
                            data.UpdatedAt = reader["UpdatedAt"]?.ToString() ?? "";
                            data.CourseImage = reader["CourseImage"] != DBNull.Value ? GlobalFetchPath + "CourseImages/" + reader["CourseImage"].ToString()
                                                  : "";

                            allCourses.Add(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
            }

            return allCourses;
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






    }
}
