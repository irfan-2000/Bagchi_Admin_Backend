using Bagchi_Admin_Backend.Models;
using Bagchi_Admin_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Bagchi_Admin_Backend.Controllers
{
    [ApiController]
    [Route("api/")]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseservice;

        private readonly IWebHostEnvironment _env;

        public string GlobalPhysicalPath;
        private readonly IConfiguration _config;


        public CourseController(ICourseService courseService, IWebHostEnvironment env, IConfiguration Iconfiguration)
        {
            _courseservice = courseService;
            _env = env;
            _config = Iconfiguration;
            GlobalPhysicalPath = _config["GlobalPhysicalPath"];

        }


        [HttpGet("GetAvailableClasses")]
        public async Task<IActionResult> GetAvailableClasses()
        {
            try
            {
                var result = await _courseservice.GetAvailableClasses();
                return Ok(new { Result = result, Message = "Result fetch success", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("AddUpdateClass")]
        public async Task<IActionResult> AddUpdateClass(Class_Dto class_Dto)
        {

            try
            {
                var result = await _courseservice.AddUpdateClass(class_Dto);
                return Ok(new { Result = result, Message = "Result fetch success", StatusCode = 200 });


            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });

            }
        }

        [HttpPost("DeleteClass")]
        public async Task<IActionResult> DeleteClass([FromQuery] int Classid)
        {

            try
            {
                var result = await _courseservice.DeleteClass(Classid);
                return Ok(new { Result = result, Message = "Result fetch success", StatusCode = 200 });


            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });

            }
        }



        [HttpGet("GetAvailableBoards")]
        public async Task<IActionResult> GetAvailableBoards()
        {
            try
            {
                var result = await _courseservice.GetAvailableBoards();
                return Ok(new { Result = result, Message = "Result fetch success", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });
            }
        }


        [HttpPost("AddUpdateBoards")]
        public async Task<IActionResult> AddUpdateBoards(Board_Dto board_Dto)
        {

            try
            {
                var result = await _courseservice.AddUpdateBoards(board_Dto);
                return Ok(new { Result = result, Message = "Result fetch success", StatusCode = 200 });


            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });

            }
        }

        [HttpPost("DeleteBoard")]
        public async Task<IActionResult> DeleteBoard([FromQuery] int Boardid)
        {

            try
            {
                var result = await _courseservice.DeleteBoard(Boardid);
                return Ok(new { Result = result, Message = "Result fetch success", StatusCode = 200 });


            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });

            }
        }

        //Subjects
        [HttpGet("GetAvailableSubjects")]
        public async Task<IActionResult> GetAvailableSubjects()
        {
            try
            {
                var result = await _courseservice.GetAvailableSubjects();
                return Ok(new { Result = result, Message = "Result fetch success", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("AddUpdateSubjects")]
        public async Task<IActionResult> AddUpdateSubjects(Subject_Dto subjectdto)
        {

            try
            {
                var result = await _courseservice.AddUpdateSubjects(subjectdto);
                return Ok(new { Result = result, Message = "Result fetch success", StatusCode = 200 });


            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });

            }
        }

        [HttpPost("DeleteSubjects")]
        public async Task<IActionResult> DeleteSubjects([FromQuery] int subjectid)
        {

            try
            {
                var result = await _courseservice.DeleteSubjects(subjectid);
                return Ok(new { Result = result, Message = "Result fetch success", StatusCode = 200 });


            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });

            }
        }




        [HttpPost("AddUpdateCoursePackagewithDetails")]
        public async Task<IActionResult> AddUpdateCoursePackagewithDetails()
        {
            try
            {


                var form = Request.Form;

                var courseName = form["courseName"].ToString();
                var courseLevel = form["courseLevel"].ToString();
                var classId = int.Parse(form["classId"]);
                var boardId = int.Parse(form["boardId"]);
                var price = decimal.Parse(form["price"]);
                var oldPrice = decimal.TryParse(form["oldPrice"], out var op) ? op : 0;
                var duration = form["duration"].ToString();
                var shortDescription = form["shortDescription"].ToString();
                var overview = form["overview"].ToString();
                var paymentType = form["paymentType"].ToString();
                var status = int.Parse(form["status"]);
                string teachername = form["teacher"].ToString();

                var subjects = JsonSerializer.Deserialize<List<int>>(form["subjectId"]);
                var highlights = JsonSerializer.Deserialize<List<string>>(form["highlights"]) ?? new List<string>();
                var requirements = JsonSerializer.Deserialize<List<string>>(form["requirements"]) ?? new List<string>();
                var objectives = JsonSerializer.Deserialize<List<string>>(form["objectives"]) ?? new List<string>();
                var batches = JsonSerializer.Deserialize<List<Batch>>(form["batches"]);
                var installments = JsonSerializer.Deserialize<List<Installment>>(form["installments"]);

                var CourseId = form["CourseId"].ToString();


                string IsEditing = form["IsEditing"];

                if (Convert.ToBoolean(IsEditing))
                {
                    if (CourseId == null || CourseId == "" || CourseId == "undefined" || CourseId == "0")
                    {
                        return BadRequest(new { Message = "CourseId not Found" });
                    }

                }
                IFormFile CourseImage = form.Files["courseImage"];
                string Old_courseImage = form["Old_courseImage"];
                string CourseImageName = string.Empty;



                var courseDto = new CoursePackageDto();

                courseDto.CourseName = courseName;
                courseDto.CourseLevel = courseLevel;
                courseDto.ClassId = classId;
                courseDto.BoardId = boardId;
                courseDto.SubjectId = subjects;
                courseDto.Price = price;
                courseDto.OldPrice = oldPrice;
                courseDto.Duration = duration;
                courseDto.ShortDescription = shortDescription;
                courseDto.Overview = overview;
                courseDto.Highlights = highlights;
                courseDto.Requirements = requirements;
                courseDto.Objectives = objectives;
                courseDto.Batches = batches;
                courseDto.PaymentType = paymentType;
                courseDto.Installments = installments;
                courseDto.Status = status;
                courseDto.IsEditing = IsEditing;

                // Safe conversion for CourseId
                courseDto.CourseId = string.IsNullOrWhiteSpace(CourseId) ? 0 : Convert.ToInt32(CourseId);

                courseDto.Teacher = teachername;

                var validationResult = _courseservice.ValidateCoursePackage(courseDto);
                if (validationResult.StatusCode != 200)
                    return BadRequest(validationResult);


                if (Convert.ToBoolean(IsEditing))
                {

                    if (CourseImage == null || CourseImage.Length <= 0)
                    {
                        CourseImageName = Old_courseImage;
                    }
                    else
                    {
                        try
                        {
                            if (CourseImage != null && CourseImage.Length > 0)
                            {
                                var fullPath = Path.Combine(GlobalPhysicalPath);

                                var uploadsFolder = Path.Combine(GlobalPhysicalPath, "CourseImages");

                                var extension = Path.GetExtension(CourseImage.FileName);
                                var randomFileName = Guid.NewGuid().ToString() + extension;
                                var fullPathwithfile = Path.Combine(uploadsFolder, randomFileName);
                                using (var fileStream = new FileStream(fullPathwithfile, FileMode.Create))
                                {
                                    await CourseImage.CopyToAsync(fileStream);
                                }
                                CourseImageName = randomFileName;

                            }

                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, new
                            {
                                message = "Failed upload .",
                                error = ex.Message
                            });

                        }
                    }
                }
                else
                {
                    try
                    {
                        if (CourseImage != null && CourseImage.Length > 0)
                        {
                            var fullPath = Path.Combine(GlobalPhysicalPath);

                            var uploadsFolder = Path.Combine(GlobalPhysicalPath, "CourseImages");

                            var extension = Path.GetExtension(CourseImage.FileName);
                            var randomFileName = Guid.NewGuid().ToString() + extension;
                            var fullPathwithfile = Path.Combine(uploadsFolder, randomFileName);
                            using (var fileStream = new FileStream(fullPathwithfile, FileMode.Create))
                            {
                                await CourseImage.CopyToAsync(fileStream);
                            }
                            CourseImageName = randomFileName;

                        }

                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new
                        {
                            message = "Failed upload .",
                            error = ex.Message
                        });

                    }
                }



                courseDto.CourseImageName = CourseImageName;


                var result = await _courseservice.AddUpdateCoursePackagewithDetails(courseDto);
                if (result.StatusCode == "1")
                {
                    return Ok(new { statuscode = "200", message = "Details has been saved" });
                }
                else
                {
                    return BadRequest();
                }


            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });
            }
        }




        [HttpGet("GetCoursebyId")]
        public async Task<IActionResult> GetCoursebyId(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
                return BadRequest();

            var result = await _courseservice.GetCoursePackageDetails(id);
            return Ok(result);
        }



        [HttpGet("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                var result = await _courseservice.GetAllCourses();
                return Ok(new { Result = result, Message = "Result fetch success", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 500,
                    Message = ex.Message
                });
            }





        }
    }
}

