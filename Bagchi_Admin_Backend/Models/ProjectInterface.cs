namespace Bagchi_Admin_Backend.Models
{
    public interface ProjectInterface
    {

    }



    public interface ILiveStreamingService
    {
       // Task<List<>> GetAllCourses();
        Task<List<LiveSession>> GetOngoingClasses();
        Task<List<BatchDetails>> GetBatchesById(string CourseId);

        Task<bool> InsertLiveClass_start(LiveSessionRequest request, long meetingId, string startUrl, string joinUrl);
        string GenerateSdkSignature(string meetingNumber, int role);
        Task<MeetingDetails> GetMeetingDetailsById(long MeetingId);

    }

    public interface IQuizService
    {

    }

    public interface ICourseService
    {
        Task<List<Class_Dto>> GetAvailableClasses();
        Task<bool> AddUpdateClass(Class_Dto dto);

        Task<bool> DeleteClass(int Classid);


    }
}
