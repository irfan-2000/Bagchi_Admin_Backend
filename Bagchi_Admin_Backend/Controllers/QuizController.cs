using Bagchi_Admin_Backend.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Bagchi_Admin_Backend.Controllers
{
    [ApiController]
    [Route("api/")]
    public class QuizController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IQuizService _quizService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public string GlobalFetchPath;
        public string GlobalPhysicalPath;
        private readonly IMemoryCache _cache;

        public QuizController(IConfiguration configuration,
            IHttpClientFactory httpClientFactory, IQuizService quizService, IWebHostEnvironment webHostEnvironment, IMemoryCache cache)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _quizService = quizService;
            _webHostEnvironment = webHostEnvironment;
            GlobalFetchPath = _configuration["GlobalFetchPath"];
            GlobalPhysicalPath = _configuration["GlobalPhysicalPath"];
             _cache = cache;
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

            string cacheKey = "quizquestions";
           // _cache.TryGetValue(cacheKey, out object? cachedData);
            var quizdata = new List<Question>();

            if (_cache.TryGetValue(cacheKey, out List<Question> quizdata1))
            {
                // Convert object → JSON string
                quizdata = quizdata1;



            }
            else
            {


                return BadRequest(new
                {
                    
                        StatusCode = "404",
                        Message = "No quiz data found please refresh the page"
                    
                });
            }
            try
            {

                
                _cache.Remove(cacheKey);

                var result = await _quizService.CreateQuizAsync(dto, quizdata);

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

  

        [HttpPost("upload-questions")]
        public async Task<IActionResult> UploadQuestions(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            string cacheKey = "quizquestions";
            _cache.Remove(cacheKey);

            var questions = new List<Question>();

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                using (var doc = WordprocessingDocument.Open(ms, false))
                {
                    var body = doc.MainDocumentPart.Document.Body;

                    var tables = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>().ToList();
                    if (!tables.Any())
                        return BadRequest("No table found in document");

                    var table = tables.First(); // Taking first table only

                    var rows = table.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ToList();

                    // Skip header row (row 0)
                    for (int i = 1; i < rows.Count; i++)
                    {   
                        var cells = rows[i].Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ToList();

                        //if (cells.Count < 12) // ensure enough columns exist
                        //    continue;

                        var q = new Question();

                        q.QuestionId = int.Parse(GetCellText(cells[0]));

                        q.QuestionText = GetCellText(cells[1]);

                        var rawImageCell = GetCellText(cells[2]);

                        q.ImagePaths = ExtractImagesFromCell(cells[2],doc);

 
                         q.OptionA = GetCellText(cells[3]);
                        q.OptionB = GetCellText(cells[4]);
                        q.OptionC = GetCellText(cells[5]);
                        q.OptionD = GetCellText(cells[6]);
                        q.IsNumerical = GetCellText(cells[7]);
                        q.NumericalAnswer = GetCellText(cells[8]);
                        q.CorrectAnswer = GetCellText(cells[9]);                
                       
                        q.PositiveMarks = double.TryParse(GetCellText(cells[10]), out var p) ? p : 0;
                        q.NegativeMarks = double.TryParse(GetCellText(cells[11]), out var n) ? n : 0;
                        

                        questions.Add(q);
                    }
                }
            }


            var validation_result = _quizService.ValidateUploadedQuestions(questions);
            if (validation_result.StatusCode != "200")
                return BadRequest(validation_result);

            _cache.Set(cacheKey, questions, TimeSpan.FromMinutes(1));
            return Ok(new QuestionUploadResult
            {
                Questions = questions,
                Message = "Parsed successfully"
            });
        }


        private string GetCellText(DocumentFormat.OpenXml.Wordprocessing.TableCell cell)
        {
            StringBuilder sb = new StringBuilder();

            var texts = cell.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>();

            foreach (var t in texts)
            {
                if (t != null && !string.IsNullOrEmpty(t.Text))
                {
                    sb.Append(t.Text);
                }
            }

            return sb.ToString().Trim();
        }
         
        private List<string> ExtractImagesFromCell(TableCell cell, WordprocessingDocument doc)
        {
            var imagesList = new List<string>();

            // Find all <a:blip> elements inside the cell (images)
            var blips = cell.Descendants<DocumentFormat.OpenXml.Drawing.Blip>();

            foreach (var blip in blips)
            {
                var embedId = blip.Embed?.Value;
                if (string.IsNullOrEmpty(embedId))
                    continue;

                // Get the ImagePart using relationship ID
                var imagePart = doc.MainDocumentPart.GetPartById(embedId) as ImagePart;
                if (imagePart == null)
                    continue;

                // Save image to wwwroot/QuestionImages
                using (var stream = imagePart.GetStream())
                {
                    string folder = Path.Combine(GlobalPhysicalPath, "QuestionImages");
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string ext = imagePart.ContentType switch
                    {
                        "image/png" => ".png",
                        "image/jpeg" => ".jpg",
                        "image/jpg" => ".jpg",
                        "image/gif" => ".gif",
                        _ => ".img"
                    };

                    string fileName = Guid.NewGuid().ToString("N").Substring(0, 12) + ext;
                    string savePath = Path.Combine(folder, fileName);

                    using (var fs = new FileStream(savePath, FileMode.Create))
                    {
                        stream.CopyTo(fs);
                    }

                    imagesList.Add(fileName);
                }
            }

            return imagesList;
        }


    }
}
