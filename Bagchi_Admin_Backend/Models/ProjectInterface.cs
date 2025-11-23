using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using static Bagchi_Admin_Backend.Services.CourseService;

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
        Task<int> UpdateMeetingStatus(long meetingid, int status);
        Task<int> insertwebhookdata(ZoomWebhookEvent webhookdata,string eventType);

        Task<List<ClassHistoryDto>> GetClassHistoryAsync();

    }

    
    public interface ICourseService
    {
        Task<List<Class_Dto>> GetAvailableClasses();
        Task<bool> AddUpdateClass(Class_Dto dto);
        Task<bool> DeleteClass(int Classid);

        Task<List<Board_Dto>> GetAvailableBoards();
        Task<bool> AddUpdateBoards(Board_Dto dto);
        Task<bool> DeleteBoard(int boardid);


        Task<List<Subject_Dto>> GetAvailableSubjects();
        Task<bool> AddUpdateSubjects(Subject_Dto dto);
        Task<bool> DeleteSubjects(int subjectid);



        ValidationResultDto ValidateCoursePackage(CoursePackageDto course);
        Task<DbResponse> AddUpdateCoursePackagewithDetails(CoursePackageDto course, List<CourseContent> coursecontent);



        Task<Tuple<CoursePackageDto, CourseDetailsDto, List<CourseContent>>> GetCoursePackageDetails(int courseId,string FromAdmin);

        Task<List<AllCourseDetails>> GetAllCourses();
        ValidationResultDto ValidateCourseInfo(CoursePackageDto course);
        Task<DbResponse> AddUpdateCourseInfoDetails(CoursePackageDto course, List<CourseContent> coursecontent);
        ValidationResultDto ValidateBatches(List<Batch> batches);
        Task<DbResponse> AddUpdateCourseBatches(List<Batch> batches, string CourseId);
    }


    public interface IQuizService
    {
        DbResponse ValidateQuiz(QuizDto quiz);
        Task<bool> CreateQuizAsync(QuizDto dto, List<Question> question);
        Task<QuizDto> GetQuizDataById(string flag, int quizid = 0);

          Task<List<QuizDto>> GetAllQuizzes();

        DbResponse ValidateUploadedQuestions(List<Question> questions);
    }

    public interface IStudentService
    {
        Task<List<StudentDto>> GetAllStudents();
        Task<StudentDto> GetStudentsbyid(int studentid);
        Task<int> UpdateStudentdetails(StudentDto dto);

        //Task<DbResponse> AddUpdateStudent(StudentDto dto);
        //Task<DbResponse> DeleteStudent(int studentId);
    }

 
}
