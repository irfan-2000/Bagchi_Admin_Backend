namespace Bagchi_Admin_Backend.Models
{
    public interface ProjectInterface
    {

    }



    public interface ILiveStreamingService
    {
        Task<List<AllCourseDetails>> GetAllCourses();
        Task<List<LiveSession>> GetOngoingClasses();
        Task<List<BatchDetails>> GetBatchesById(string CourseId);

        Task<bool> InsertLiveClass_start(LiveSessionRequest request, long meetingId, string startUrl, string joinUrl);
    }
}
