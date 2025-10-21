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
        Task<DbResponse> AddUpdateCoursePackagewithDetails(CoursePackageDto course);
      

        Task<Tuple<CoursePackageDto, CourseDetailsDto>> GetCoursePackageDetails(int courseId);

        Task<List<AllCourseDetails>> GetAllCourses();
    }


    public interface IQuizService
    {
        DbResponse ValidateQuiz(QuizDto quiz);
        Task<bool> CreateQuizAsync(QuizDto dto);
        Task<QuizDto> GetQuizDataById(string flag, int quizid = 0);

          Task<List<QuizDto>> GetAllQuizzes();
    }

    public interface IStudentService
    {
        Task<List<StudentDto>> GetAllStudents();
        Task<StudentDto> GetStudentsbyid(int studentid);
        Task<int> UpdateStudentdetails(StudentDto dto);

        //Task<DbResponse> AddUpdateStudent(StudentDto dto);
        //Task<DbResponse> DeleteStudent(int studentId);
    }


    public interface IShayariService
    {
        Task<List<ShayariDto>> GetAllShayaris();
        Task<ShayariDto> GetShayariById(int shayariId);
        Task<List<ShayariDto>> GetShayarisByCategory(int categoryId);
        Task<int> InsertUser(UserDto user);
    }
}
