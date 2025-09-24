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
        public async Task<IActionResult> DeleteClass(int Classid)
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







    }
}
