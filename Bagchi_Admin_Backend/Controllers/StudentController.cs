using Bagchi_Admin_Backend.Models;
using Bagchi_Admin_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Bagchi_Admin_Backend.Controllers
{
    [ApiController]
    [Route("api/")]
    public class StudentController : Controller
    {
        private readonly IWebHostEnvironment _env;
         private readonly IConfiguration _config;
                private readonly IStudentService _studentservice;

        public string GlobalPhysicalPath;

        public StudentController(IConfiguration configuration, IWebHostEnvironment env, IStudentService studentservice)
        {
            _config = configuration;
             _studentservice = studentservice; 
            GlobalPhysicalPath = _config["GlobalPhysicalPath"];
        }



        [HttpGet("Getallstudents")]
        public async Task<IActionResult> Getallquizzes()
        {
            try
            {
                var result = await _studentservice.GetAllStudents();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while getting the quiz data");

            }

        }



        [HttpGet("GetStudentsbyid")]
        public async Task<IActionResult> GetStudentsbyid(int studentid)
        {
            try
            {
                var result = await _studentservice.GetStudentsbyid(studentid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while getting the quiz data");

            }

        }


        [HttpPost("UpdateStudentdetails")]
        public async Task<IActionResult> UpdateStudentdetails()
        {
            try
            {
                StudentDto studentdata = new StudentDto();

                var form = Request.Form;

                studentdata.StudentId = int.Parse(form["StudentId"]);
                studentdata.FullName = form["FullName"];
                studentdata.DateOfBirth = DateTime.Parse(form["DateOfBirth"]);
                studentdata.Gender = form["Gender"];
                studentdata.Email = form["Email"];
                studentdata.Phone = form["Phone"];
                studentdata.ParentName = form["ParentName"];
                studentdata.ParentMobile = form["ParentMobile"];
                studentdata.Address = form["Address"];
                studentdata.City = form["City"];
                studentdata.State = form["State"];
                studentdata.Pincode = form["Pincode"]; 

                studentdata.BoardId = !string.IsNullOrEmpty(form["BoardId"].ToString())? int.Parse(form["BoardId"])  : 0;
                studentdata.ClassId = int.Parse(form["ClassId"]);
                 studentdata.Status = int.Parse(form["Status"]);
                studentdata.Password = form["Password"];
                 studentdata.Student_Cast = form["Student_Cast"];
                 

              IFormFile Profileimage = form.Files["ProfileImage"];
                studentdata.ImageName = "";

                try
                {
                    if(Profileimage != null && Profileimage.Length > 0)
                        {
                        var fullPath = Path.Combine(GlobalPhysicalPath);

                        var uploadsFolder = Path.Combine(GlobalPhysicalPath, "StudentImages");

                        var extension = Path.GetExtension(Profileimage.FileName);
                        var randomFileName = Guid.NewGuid().ToString() + extension;
                        var fullPathwithfile = Path.Combine(uploadsFolder, randomFileName);
                        using (var fileStream = new FileStream(fullPathwithfile, FileMode.Create))
                        {
                            await Profileimage.CopyToAsync(fileStream);
                        }
                        studentdata.ImageName = randomFileName; 
                    }
                    else
                    {
                        studentdata.ImageName = form["OldImageName"];
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
                var result = await _studentservice.UpdateStudentdetails(studentdata);
                if(result > 0)
                {
                    return Ok(new { statuscode = "200", message = "Details has been saved" });

                }
                else
                {
                    return Ok(new { statuscode = "500", message = "Error saving details" });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while  saving student data");

            }

        }


    }
}
