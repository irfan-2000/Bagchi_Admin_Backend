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



        [HttpPost("createQuiz")]
        public async Task<IActionResult> Create([FromBody] QuizDto dto)
        {
            if (dto == null) 
                return BadRequest("Payload is required.");

            if (dto.BatchId == null || dto.BatchId.Count == 0)
                return BadRequest("At least one batch must be selected.");

            if (string.IsNullOrWhiteSpace(dto.Title)) 
                return BadRequest("Title is required.");

            var validation_result = _quizService. ValidateQuiz(dto);
            if (validation_result.StatusCode != "200")
                return BadRequest(validation_result);

            try
            {
                var result = await _quizService.CreateQuizAsync(dto);
                if(result)
                {
                    if(dto.QuizId > 0)
                    {
                        return Ok(new { statuscode = "200", message = "Quiz Updated!!" });

                        
                    }
                    else
                    {
                        return Ok(new { statuscode = "200", message = "Details has been saved/New Record Added" });


                    }


                }
                else
                {
                    return Ok(new { statuscode = "500", message = "Error saving details" });

                }
            }
            catch (Exception ex)
            {
                // log ex
                return StatusCode(500, "An error occurred while creating the quiz.");
            }
        }



        [HttpGet("Getquizdata")]
        public async Task<IActionResult> Getquizdatabyid(string flag ,int quizid= 0)
        {
            try
            {
                var result = await _quizService.GetQuizDataById(flag , quizid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while getting the quiz data");

            }

        }

        [HttpGet("Getallquizzes")]
        public async Task<IActionResult> Getallquizzes( )
        {
            try
            {
                var result = await _quizService.GetAllQuizzes();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while getting the quiz data");

            }

        }





    }
}
