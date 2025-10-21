using Bagchi_Admin_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Bagchi_Admin_Backend.Services
{
    public class StudentService :IStudentService
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _config;
        public string GlobalFetchPath { get; set; }
        public StudentService(ApplicationDbContext dbContext, IConfiguration Iconfiguration)
        {
            _dbContext = dbContext;
            _config = Iconfiguration;

            GlobalFetchPath = _config["GlobalFetchPath"];

        }
        public async Task<List<StudentDto>> GetAllStudents()
        {
            List<StudentDto> Students = new List<StudentDto>();
            try
            {

                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "Sp_ManageStudentsDetails";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    
                    DbHelper.AddParameter(cmd, "@Flag", "S");

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                StudentDto student = new StudentDto();

                                student.StudentId = Convert.IsDBNull(reader["StudentId"]) ? 0 : Convert.ToInt32(reader["StudentId"]);
                                student.FullName = Convert.IsDBNull(reader["FullName"]) ? "" : reader["FullName"].ToString();
                                student.Gender = Convert.IsDBNull(reader["Gender"]) ? "" : reader["Gender"].ToString();
                                student.Student_Cast = Convert.IsDBNull(reader["Student_Cast"]) ? "" : reader["Student_Cast"].ToString();
                                student.Email = Convert.IsDBNull(reader["Email"]) ? "" : reader["Email"].ToString();
                                student.Phone = Convert.IsDBNull(reader["Phone"]) ? "" : reader["Phone"].ToString();
                                student.City = Convert.IsDBNull(reader["City"]) ? "" : reader["City"].ToString();
                                student.ClassId = Convert.IsDBNull(reader["ClassId"]) ? 0 : Convert.ToInt32(reader["ClassId"]);
                                student.InstitutionName = Convert.IsDBNull(reader["InstitutionName"]) ? "" : reader["InstitutionName"].ToString();
                                student.Status = Convert.IsDBNull(reader["Status"]) ? 0 : Convert.ToInt32(reader["Status"]);
                                student.ImageName = Convert.IsDBNull(reader["ImageName"]) ? "" : reader["ImageName"].ToString();
                                student.IsActive = Convert.IsDBNull(reader["IsActive"]) ? 0 : Convert.ToInt32(reader["IsActive"]);
                                student.CreatedAt = Convert.IsDBNull(reader["CreatedAt"]) ? DateTime.MinValue : Convert.ToDateTime(reader["CreatedAt"]);
                                student.ClassName = Convert.IsDBNull(reader["ClassName"]) ? "" : reader["ClassName"].ToString();
                                student.ImageUrl = Convert.IsDBNull(reader["ImageName"]) ? "" : GlobalFetchPath + "StudentImages/" + reader["ImageName"].ToString();
                                Students.Add(student);
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return Students;
        }

        public async Task<StudentDto> GetStudentsbyid(int studentid)
        {
            StudentDto student = new StudentDto();
            try
            {

                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "Sp_ManageStudentsDetails";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    
                    DbHelper.AddParameter(cmd, "@Flag", "SID");
                    DbHelper.AddParameter(cmd, "@StudentId", studentid);

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                             

                            student.StudentId = Convert.IsDBNull(reader["StudentId"]) ? 0 : Convert.ToInt32(reader["StudentId"]);
                            student.FullName = Convert.IsDBNull(reader["FullName"]) ? "" : reader["FullName"].ToString();
                            student.DateOfBirth = Convert.IsDBNull(reader["DateOfBirth"]) ? DateTime.MinValue : Convert.ToDateTime(reader["DateOfBirth"]);
                            student.Gender = Convert.IsDBNull(reader["Gender"]) ? "" : reader["Gender"].ToString();
                            student.Email = Convert.IsDBNull(reader["Email"]) ? "" : reader["Email"].ToString();
                            student.Phone = Convert.IsDBNull(reader["Phone"]) ? "" : reader["Phone"].ToString();
                            student.ParentName = Convert.IsDBNull(reader["ParentName"]) ? null : reader["ParentName"].ToString();
                            student.ParentMobile = Convert.IsDBNull(reader["ParentMobile"]) ? null : reader["ParentMobile"].ToString();
                            student.Address = Convert.IsDBNull(reader["Address"]) ? null : reader["Address"].ToString();
                            student.City = Convert.IsDBNull(reader["City"]) ? null : reader["City"].ToString();
                            student.State = Convert.IsDBNull(reader["State"]) ? null : reader["State"].ToString();
                            student.Pincode = Convert.IsDBNull(reader["Pincode"]) ? null : reader["Pincode"].ToString();
                            student.InstitutionId = Convert.IsDBNull(reader["InstitutionId"]) ? (int?)null : Convert.ToInt32(reader["InstitutionId"]);
                            student.InstitutionName = Convert.IsDBNull(reader["InstitutionName"]) ? null : reader["InstitutionName"].ToString();
                            student.BoardId = Convert.IsDBNull(reader["BoardId"]) ? 0 : Convert.ToInt32(reader["BoardId"]);
                            student.ClassId = Convert.IsDBNull(reader["ClassId"]) ? 0 : Convert.ToInt32(reader["ClassId"]);
                            student.BatchId = Convert.IsDBNull(reader["BatchId"]) ? 0 : Convert.ToInt32(reader["BatchId"]);
                            student.Status = Convert.IsDBNull(reader["Status"]) ? 0 : Convert.ToInt32(reader["Status"]); // 1 = Active, 0 = Inactive
                            student.Password = Convert.IsDBNull(reader["Password"]) ? "" : reader["Password"].ToString();
                            student.CreatedAt = Convert.IsDBNull(reader["CreatedAt"]) ? DateTime.MinValue : Convert.ToDateTime(reader["CreatedAt"]);
                            student.UpdatedAt = Convert.IsDBNull(reader["UpdatedAt"]) ? (DateTime?)null : Convert.ToDateTime(reader["UpdatedAt"]);
                            student.ImageName = Convert.IsDBNull(reader["ImageName"]) ? null : reader["ImageName"].ToString();
                            student.SubjectId = Convert.IsDBNull(reader["SubjectId"]) ? (int?)null : Convert.ToInt32(reader["SubjectId"]);
                            student.IsActive = Convert.IsDBNull(reader["IsActive"]) ? 0 : Convert.ToInt32(reader["IsActive"]);
                            student.Student_Cast = Convert.IsDBNull(reader["Student_Cast"]) ? null : reader["Student_Cast"].ToString();
                            student.ImageUrl = Convert.IsDBNull(reader["ImageName"]) ? "" : GlobalFetchPath + "StudentImages/" + reader["ImageName"].ToString();


                        }
                    }

                }

            }
            catch (Exception ex)
            {

            }

            return student;
        }



        public async Task<int> UpdateStudentdetails(StudentDto dto)
        {
            try
            {

                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "Sp_ManageStudentsDetails";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    DbHelper.AddParameter(cmd, "@Flag", "U");
                    DbHelper.AddParameter(cmd, "@StudentId", dto.StudentId);
                    DbHelper.AddParameter(cmd, "@FullName", dto.FullName);
                    DbHelper.AddParameter(cmd, "@DateOfBirth", dto.DateOfBirth.ToString("yyyy-MM-dd") );
                    DbHelper.AddParameter(cmd, "@Gender", dto.Gender);
                    DbHelper.AddParameter(cmd, "@Email", dto.Email);
                    DbHelper.AddParameter(cmd, "@Phone", dto.Phone);
                    DbHelper.AddParameter(cmd, "@ParentName", dto.ParentName);
                    DbHelper.AddParameter(cmd, "@ParentMobile", dto.ParentMobile);
                    DbHelper.AddParameter(cmd, "@Address", dto.Address);
                    DbHelper.AddParameter(cmd, "@City", dto.City);
                    DbHelper.AddParameter(cmd, "@State", dto.State);
                    DbHelper.AddParameter(cmd, "@Pincode", dto.Pincode);
                    DbHelper.AddParameter(cmd, "@InstitutionId", dto.InstitutionId);
                    DbHelper.AddParameter(cmd, "@InstitutionName", dto.InstitutionName);
                    DbHelper.AddParameter(cmd, "@BoardId", dto.BoardId);
                    DbHelper.AddParameter(cmd, "@ClassId", dto.ClassId);
                    DbHelper.AddParameter(cmd, "@Status", dto.Status);
                    DbHelper.AddParameter(cmd, "@Password", dto.Password);
                    DbHelper.AddParameter(cmd, "@ImageName", dto.ImageName);
                    DbHelper.AddParameter(cmd, "@StudentCast", dto.Student_Cast);


                    await _dbContext.Database.OpenConnectionAsync();

                    var result = await cmd.ExecuteNonQueryAsync();

                    if (result > 0)
                        return 1;



                }

            }
            catch (Exception ex)
            {
               
            }

                return 0;
        }



    }
}
