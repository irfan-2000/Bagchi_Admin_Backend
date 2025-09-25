using Bagchi_Admin_Backend.Models;
using Bagchi_Admin_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bagchi_Admin_Backend.Controllers
{
    [ApiController]
    [Route("api/")]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseservice;


        public CourseController(ICourseService courseService)
        {
            _courseservice = courseService;

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
        public async Task<IActionResult> DeleteClass([FromQuery]  int Classid)
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




    }
}
