using Bagchi_Admin_Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bagchi_Admin_Backend.Controllers
{
    [ApiController]
    [Route("api/")]
    public class QuizController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IQuizService _quizService;

        public QuizController(IConfiguration configuration,
            IHttpClientFactory httpClientFactory, IQuizService quizService)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _quizService = quizService;
        }





    }
}
