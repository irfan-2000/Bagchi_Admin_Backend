using System.Data.Common;
using System.Data;

namespace Bagchi_Admin_Backend.Models
{
    public class TemplateClasses
    {
    }


    public class LiveSessionRequest
    {
        public string topic { get; set; }
        public int type { get; set; }
        public DateTime? start_time { get; set; }   // nullable because instant type may not send it
        public int duration { get; set; }
        public string timezone { get; set; }
        public string agenda { get; set; }

        public bool host_video { get; set; }
        public bool participant_video { get; set; }
        public bool join_before_host { get; set; }
        public bool mute_upon_entry { get; set; }
        public int approval_type { get; set; }
        public string batchId { get; set; }
        public string CourseId { get; set; }
        public string teachername { get; set; }

         
        public string? accesstoken { get; set; }

        public string? refreshtoken { get; set; }

         public int? expiresin { get; set; }

        public string?   meetingpassword { get; set; }

        public string? zaktoken { get; set; }

        public string? Signature { get; set; }
    
            public string? zoomcode { get; set; }
    }





    public static class DbHelper
    {
        public static void AddParameter(DbCommand cmd, string name, object value, DbType dbType = DbType.String)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = string.IsNullOrEmpty(value?.ToString()) ? DBNull.Value : value;
            param.DbType = dbType;
            cmd.Parameters.Add(param);
        }
    }

    public class Class_Dtos
    {
        public string DetailId { get; set; }
        public string CourseId { get; set; }
                
        public string CourseName { get; set; }
        public string Price { get; set; }
        public string Status { get; set; }

        public string Description { get; set; }
        public string Objectives { get; set; }

        public string Requirements { get; set; }
        public string CourseLevel { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string CourseImage { get; set; }
        public string OldPrice { get; set; }
    }



    public class LiveSession
    {
        public string LiveSessionId { get; set; }
        public string BatchId { get; set; }
        public string CourseId { get; set; }
        public string Topic { get; set; }
        public string Agenda { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Duration { get; set; }
        public string ZoomMeetingId { get; set; }
        public string ZoomStartUrl { get; set; }
        public string ZoomJoinUrl { get; set; }
        public string ZoomPassword { get; set; }
        public string HostVideo { get; set; }
        public string ParticipantVideo { get; set; }
        public string JoinBeforeHost { get; set; }
        public string MuteUponEntry { get; set; }
        public string ApprovalType { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }



    public class BatchDetails
    {
        public string BatchId { get; set; }
        public string BatchName { get; set; }
        public string ClassId { get; set; }
        public string SubjectId { get; set; }
        public string BoardId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }
        public string CourseId { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }




    public class ZoomRequest
    {
        public string MeetingNumber { get; set; }
        public int Role { get; set; } // 1 = host, 0 = attendee
    }


    public class MeetingDetails
    {
        public string Signature { get; set; }
        public string ZoomPassword { get; set; }
        public string Teacher { get; set; }
        public string TeacherName { get; set; }
        public string ZakToken { get; set; }
        public string MeetingPassword { get; set; }
    }



    public class Class_Dto
    {
        public int ClassId { get; set; } =  0;
        public string ClassName { get; set; } = string.Empty;
        public int Status { get; set; } = 0;

        public string Flag { get; set; } = string.Empty;
    }

    public class Board_Dto
    {
        public int BoardId { get; set; } = 0;
        public string BoardName { get; set; } = string.Empty;
        public int Status { get; set; } = 0;

        public string Flag { get; set; } = string.Empty;
    }

    public class Subject_Dto
    {
        public int SubjectId { get; set; } = 0;
        public int ClassId { get; set; } = 0;
        public string SubjectName { get; set; } = string.Empty;
        public int Status { get; set; } = 0;

        public int BoardId { get; set; } = 0;

        public string Flag { get; set; } = string.Empty;
    }


}
