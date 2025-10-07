﻿using Azure.Core;
using Bagchi_Admin_Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bagchi_Admin_Backend.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _config;
        public string GlobalFetchPath { get; set; }
        public CourseService(ApplicationDbContext dbContext, IConfiguration Iconfiguration)
        {
            _dbContext = dbContext;
            _config = Iconfiguration;

            GlobalFetchPath = _config["GlobalFetchPath"];
                
        }


        public  async Task<List<Class_Dto>> GetAvailableClasses()
        {
            List<Class_Dto> classess = new List<Class_Dto>();

            try
            { 

                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_Manage_AvailableClasses";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    DbHelper.AddParameter(cmd, "@From", "A");
                    DbHelper.AddParameter(cmd, "@Flag", "S");

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Class_Dto data = new Class_Dto();

                            data.ClassId = Convert.IsDBNull(reader["ClassId"]) ? 0 : Convert.ToInt32(reader["ClassId"]);
                            data.ClassName = Convert.IsDBNull(reader["ClassName"]) ? "" : reader["ClassName"].ToString();
                            data.Status = Convert.IsDBNull(reader["Status"]) ? 0 : Convert.ToInt16(reader["Status"]);

                            classess.Add(data);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return classess;
        }

        public async Task<bool> AddUpdateClass(Class_Dto dto)
        {
            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "sp_Manage_AvailableClasses";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        await cmd.Connection.OpenAsync();
                    }


                    if (dto.Flag == "A")
                    {
                        DbHelper.AddParameter(cmd, "@Flag", "A");
                        DbHelper.AddParameter(cmd, "@ClassName", dto.ClassName);

                        DbHelper.AddParameter(cmd, "@Status",dto.Status);

                        var result = await cmd.ExecuteNonQueryAsync();
                        if(result > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }else if(dto.Flag == "U")
                    {
                        DbHelper.AddParameter(cmd, "@Flag", "U");
                        DbHelper.AddParameter(cmd, "@ClassName", dto.ClassName);
                        DbHelper.AddParameter(cmd, "@ClassId", dto.ClassId);
                        DbHelper.AddParameter(cmd, "@Status", dto.Status);

                        var result = await cmd.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }else if(dto.Flag == "D")
                    {
                        DbHelper.AddParameter(cmd, "@Flag", "D");

                        var result = await cmd.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    } 

                }


            }catch(Exception ex)
            {

            }

            return false;
        }
         
        public async Task<bool> DeleteClass(int Classid)
        {
            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_Manage_AvailableClasses";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                     DbHelper.AddParameter(cmd, "@Flag", "D");
                    DbHelper.AddParameter(cmd, "@ClassId", Classid);

                    await _dbContext.Database.OpenConnectionAsync();

                    var result = await cmd.ExecuteNonQueryAsync();

                    if(result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                } 
            }
            catch(Exception ex)
            {

            }

            return false;
        }




        public async Task<List<Board_Dto>> GetAvailableBoards()
        {
            List<Board_Dto> classess = new List<Board_Dto>();

            try
            {

                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_Manage_AvailableBoards";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    DbHelper.AddParameter(cmd, "@From", "A");
                    DbHelper.AddParameter(cmd, "@Flag", "S");

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Board_Dto data = new Board_Dto();

                            data.BoardId = Convert.IsDBNull(reader["BoardId"]) ? 0 : Convert.ToInt32(reader["BoardId"]);
                            data.BoardName = Convert.IsDBNull(reader["BoardName"]) ? "" : reader["BoardName"].ToString();
                            data.Status = Convert.IsDBNull(reader["Status"]) ? 0 : Convert.ToInt16(reader["Status"]);

                            classess.Add(data);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return classess;
        }
         
        public async Task<bool> AddUpdateBoards(Board_Dto dto)
        {
            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "sp_Manage_AvailableBoards";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        await cmd.Connection.OpenAsync();
                    }


                    if (dto.Flag == "I")
                    {
                        DbHelper.AddParameter(cmd, "@Flag", "I");
                        DbHelper.AddParameter(cmd, "@BoardName", dto.BoardName);

                        DbHelper.AddParameter(cmd, "@Status", dto.Status);

                        var result = await cmd.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else if (dto.Flag == "U")
                    {
                        DbHelper.AddParameter(cmd, "@Flag", "U");
                        DbHelper.AddParameter(cmd, "@BoardName", dto.BoardName);
                        DbHelper.AddParameter(cmd, "@BoardId", dto.BoardId);
                        DbHelper.AddParameter(cmd, "@Status", dto.Status);

                        var result = await cmd.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    

                }


            }
            catch (Exception ex)
            {

            }

            return false;
        }
        public async Task<bool> DeleteBoard(int boardid)
        {
            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_Manage_AvailableBoards";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    // Parameters
                    DbHelper.AddParameter(cmd, "@Flag", "D");
                    DbHelper.AddParameter(cmd, "@BoardId", boardid);

                    await _dbContext.Database.OpenConnectionAsync();

                    var result = await cmd.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }




        public async Task<List<Subject_Dto>> GetAvailableSubjects()
        {
            List<Subject_Dto> subjects = new List<Subject_Dto>();

            try
            {

                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "Sp_Manage_AvailableSubjects";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    DbHelper.AddParameter(cmd, "@From", "A");
                    DbHelper.AddParameter(cmd, "@Flag", "S");

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Subject_Dto data = new Subject_Dto();
 
                            data.BoardId = Convert.IsDBNull(reader["BoardId"]) ? 0 : Convert.ToInt32(reader["BoardId"]);
                            data.SubjectId = Convert.IsDBNull(reader["SubjectId"]) ? 0 : Convert.ToInt32(reader["SubjectId"]);
                            data.Status = Convert.IsDBNull(reader["Status"]) ? 0 : Convert.ToInt32(reader["Status"]);
                        data.ClassId = Convert.IsDBNull(reader["ClassId"]) ? 0 : Convert.ToInt32(reader["ClassId"]);
                        
                            data.SubjectName = Convert.IsDBNull(reader["SubjectName"]) ? "" : reader["SubjectName"].ToString();
 
                            subjects.Add(data);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return subjects;
        }

        public async Task<bool> AddUpdateSubjects(Subject_Dto dto)
        {
            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "Sp_Manage_AvailableSubjects";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        await cmd.Connection.OpenAsync();
                    }


                    if (dto.Flag == "I")
                    {
                        DbHelper.AddParameter(cmd, "@Flag", "I");
                        DbHelper.AddParameter(cmd, "@ClassId", dto.ClassId);
                        DbHelper.AddParameter(cmd, "@Subjectname", dto.SubjectName);
                        DbHelper.AddParameter(cmd, "@Status", dto.Status);
                        DbHelper.AddParameter(cmd, "@BoardId", dto.BoardId);
 
                        var result = await cmd.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else if (dto.Flag == "U")
                    {
                        DbHelper.AddParameter(cmd, "@Flag", "U");

                         DbHelper.AddParameter(cmd, "@ClassId",DBNull.Value);
                        DbHelper.AddParameter(cmd, "@Subjectname", dto.SubjectName);
                        DbHelper.AddParameter(cmd, "@Status", dto.Status);
                        DbHelper.AddParameter(cmd, "@BoardId",DBNull.Value);
                        DbHelper.AddParameter(cmd, "@SubjectId", dto.SubjectId);
                        var result = await cmd.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }


                }


            }
            catch (Exception ex)
            {

            }

            return false;
        }
        public async Task<bool> DeleteSubjects(int subjectid)
        {
            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "Sp_Manage_AvailableSubjects";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    // Parameters
                    DbHelper.AddParameter(cmd, "@Flag", "D");
                    DbHelper.AddParameter(cmd, "@SubjectId", subjectid);

                    await _dbContext.Database.OpenConnectionAsync();

                    var result = await cmd.ExecuteNonQueryAsync();

                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }



        public class ValidationResultDto
        {
            public int StatusCode { get; set; }
            public string ErrorMessage { get; set; }
        }

        public ValidationResultDto ValidateCoursePackage(CoursePackageDto course)
        {
            var errors = new List<string>();

            // Simple fields
            if (string.IsNullOrWhiteSpace(course.CourseName))
                errors.Add("Course Name is required.");

            if (string.IsNullOrWhiteSpace(course.CourseLevel))
                errors.Add("Course Level is required.");

            if (course.ClassId <= 0)
                errors.Add("Class is required.");

            if (course.BoardId <= 0)
                errors.Add("Board is required.");

            if (course.SubjectId == null || course.SubjectId.Count == 0)
                errors.Add("At least one subject is required.");

            if (course.Price <= 0)
                errors.Add("Price is required and must be greater than 0.");

            if (string.IsNullOrWhiteSpace(course.Duration))
                errors.Add("Course Duration is required.");

            if (string.IsNullOrWhiteSpace(course.ShortDescription))
                errors.Add("Short Description is required.");

            if (string.IsNullOrWhiteSpace(course.Overview))
                errors.Add("Overview is required.");

            // Highlights, Requirements, Objectives
            if (course.Highlights == null || course.Highlights.Count == 0)
                errors.Add("At least one valid highlight is required.");

            if (course.Requirements == null || course.Requirements.Count == 0)
                errors.Add("At least one valid requirement is required.");

            if (course.Objectives == null || course.Objectives.Count == 0)
                errors.Add("At least one valid objective is required.");

            // Batches
            if (course.Batches == null || course.Batches.Count == 0)
                errors.Add("At least one batch is required.");
            else
            {
                for (int i = 0; i < course.Batches.Count; i++)
                {
                    var batch = course.Batches[i];
                    if (string.IsNullOrWhiteSpace(batch.batchName))
                        errors.Add($"Batch Name is required for batch {i + 1}.");
                    if (batch.batch_classId <= 0)
                        errors.Add($"Class is required for batch {i + 1}.");
                    if (batch.batch_subjectId <= 0)
                        errors.Add($"Subject is required for batch {i + 1}.");
                    if (batch.batch_boardId <= 0)
                        errors.Add($"Board is required for batch {i + 1}.");
                    if (batch.startDate == default)
                        errors.Add($"Start Date is required for batch {i + 1}.");
                    if (batch.endDate == default)
                        errors.Add($"End Date is required for batch {i + 1}.");
                    if (batch.startTime == default)
                        errors.Add($"Start Time is required for batch {i + 1}.");
                    if (batch.endTime == default)
                        errors.Add($"End Time is required for batch {i + 1}.");
                }
            }

            // Payment & Installments
            if (string.IsNullOrWhiteSpace(course.PaymentType))
                errors.Add("Payment Type is required.");
            else if (course.PaymentType.ToLower() == "installments")
            {
                if (course.Installments == null || course.Installments.Count == 0)
                    errors.Add("At least one installment is required.");

                for (int i = 0; i < course.Installments.Count; i++)
                {
                    var inst = course.Installments[i];
                    if (inst.Amount <= 0)
                        errors.Add($"Amount is required for installment {i + 1}.");
                    if (inst.DueDaysFromStart < 0)
                        errors.Add($"Due Days From Start is required for installment {i + 1}.");
                }
            }

            if (course.Status != 0 && course.Status != 1)
                errors.Add("Status must be 0 (Inactive) or 1 (Active).");

            return new ValidationResultDto
            {
                StatusCode = errors.Count == 0 ? 200 : 400,
                ErrorMessage = errors.Count > 0 ? string.Join("\n", errors) : null
            };
        }
 

        public async Task<DbResponse> AddUpdateCoursePackagewithDetails(CoursePackageDto course)
        {
            DbResponse data = new DbResponse();


            try
            {

           
            // ---------------------------
            // 1. Construct CourseDescription object for DB storage
            // ---------------------------
            var courseDescription = new
            {
                ShortDescription = course.ShortDescription,
                Overview = course.Overview,
                Duration = course.Duration,
                Level = course.CourseLevel,
                Highlights =  course.Highlights // assuming Highlight has Te    xt property
            };

            string courseDescriptionJson = System.Text.Json.JsonSerializer.Serialize(courseDescription);

            // ---------------------------
            // 2. Convert Installments list to XML
            // ---------------------------
            var installmentsXml = new XElement("Installments",
                from inst in course.Installments ?? Enumerable.Empty<Installment>()
                select new XElement("Installment",
                    new XElement("TotalInstallments", course.Installments.Count),
                    new XElement("InstallmentNumber", inst.InstallmentNumber),
                    new XElement("Amount", inst.Amount),
                    new XElement("DueDaysFromStart", inst.DueDaysFromStart),
                    new XElement("Remarks", inst.Remarks)
                )
            );
            string installmentsXmlString = installmentsXml.ToString();

            // ---------------------------
            // 3. Convert Subjects to JSON string for SP
            // ---------------------------
            string subjectsJson = System.Text.Json.JsonSerializer.Serialize(course.SubjectId ?? new List<int>());

                // ---------------------------
                // 4. Convert Batches to XML
                // ---------------------------
                var batchesXml = new XElement("Batches",
         from batch in course.Batches ?? Enumerable.Empty<Batch>()
         select new XElement("Batch",
             new XElement("BatchName", batch.batchName ?? string.Empty),      // nvarchar(200)
             new XElement("Batch_ClassId", batch.batch_classId),               // int
             new XElement("Batch_SubjectId", batch.batch_subjectId),           // int
             new XElement("Batch_BoardId", batch.batch_boardId),               // int
             new XElement("StartDate", batch.startDate.ToString("yyyy-MM-dd")), // date
             new XElement("EndDate", batch.endDate.ToString("yyyy-MM-dd")),     // date
             new XElement("StartTime", batch.startTime.ToString(@"hh\:mm")),   // TimeSpan to HH:mm
             new XElement("EndTime", batch.endTime.ToString(@"hh\:mm"))        // TimeSpan to HH:mm
         )
     );

                string batchesXmlString = batchesXml.ToString();

                // ---------------------------
                // 5. Execute Stored Procedure
                // ---------------------------
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "AddUpdateCoursePackageWithDetails"; // Your SP name
                cmd.CommandType = CommandType.StoredProcedure;

                if (cmd.Connection.State != ConnectionState.Open)
                    await cmd.Connection.OpenAsync();



                //DbHelper.AddParameter(cmd, "@Flag", "I");
                
                DbHelper.AddParameter(cmd, "@CoursePackageId", course.CourseId);
                DbHelper.AddParameter(cmd, "@CourseName", course.CourseName);
                DbHelper.AddParameter(cmd, "@CourseLevel", course.CourseLevel);
                DbHelper.AddParameter(cmd, "@CourseImageName", course.CourseImageName);
                DbHelper.AddParameter(cmd, "@ClassId", course.ClassId);
                DbHelper.AddParameter(cmd, "@BoardId", course.BoardId);
                DbHelper.AddParameter(cmd, "@Price", course.Price);
                DbHelper.AddParameter(cmd, "@OldPrice", course.OldPrice);
                DbHelper.AddParameter(cmd,"@PaymentType", course.PaymentType);

                DbHelper.AddParameter(cmd, "@Status", course.Status);




                DbHelper.AddParameter(cmd, "@CourseFullDescription", courseDescriptionJson);


                    //string requirementsJson = System.Text.Json.JsonSerializer.Serialize(reqList);
                    DbHelper.AddParameter(cmd, "@Requirements", JsonConvert.SerializeObject(course.Requirements));


                    //var objectiveslist = course.Objectives?.Select(o => o.text).ToList() ?? new List<string>();
                    string objectivesjson = System.Text.Json.JsonSerializer.Serialize(course.Objectives);

                    DbHelper.AddParameter(cmd, "@Objectives", objectivesjson);








                    DbHelper.AddParameter(cmd, "@Installments", installmentsXmlString); 
                
                DbHelper.AddParameter(cmd, "@Subjects", subjectsJson); 
                 
                DbHelper.AddParameter(cmd, "@Batches", batchesXmlString);


                await _dbContext.Database.OpenConnectionAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    { 
                         data.StatusCode = Convert.IsDBNull(reader["StatusCode"]) ? "" : reader["StatusCode"].ToString();
                        data.Message = Convert.IsDBNull(reader["Message"]) ? "" : reader["Message"].ToString();
                         
                     }
                }

                return data;



            }

            }
            catch (Exception ex)
            {

            }


            return data; 

        }

 
    public async Task<Tuple<CoursePackageDto,CourseDetailsDto>> GetCoursePackageDetails(int courseId )
        {
            CoursePackageDto course = new CoursePackageDto();

            string CourseFullDescription = string.Empty;
            try
            {

                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = "GetCoursePackageWithDetails";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();

                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        await cmd.Connection.OpenAsync();
                    }


                    DbHelper.AddParameter(cmd, "@CoursePackageId", courseId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        // 1. Main course info
                        if (await reader.ReadAsync())
                        {
                            course.CourseId = reader.GetInt32(reader.GetOrdinal("CourseId"));
                            course.CourseName = reader.GetString(reader.GetOrdinal("CourseName"));
                            course.CourseLevel = reader.GetString(reader.GetOrdinal("CourseLevel"));
                            course.CourseImageName = reader.IsDBNull(reader.GetOrdinal("CourseImage"))? null: reader.GetString(reader.GetOrdinal("CourseImage"));
                            course.ImagewithPath = reader.IsDBNull(reader.GetOrdinal("CourseImage")) ? null : GlobalFetchPath + "CourseImages/" + reader.GetString(reader.GetOrdinal("CourseImage"));
                            course.ClassId = reader.GetInt32(reader.GetOrdinal("ClassId"));
                            course.BoardId = reader.GetInt32(reader.GetOrdinal("BoardId"));
                            course.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                            course.OldPrice = reader.GetDecimal(reader.GetOrdinal("OldPrice"));
                            course.PaymentType = reader.IsDBNull(reader.GetOrdinal("PaymentType"))? null: reader.GetString(reader.GetOrdinal("PaymentType"));
                            course.Status = reader.GetInt32(reader.GetOrdinal("Status"));

                            // Deserialize Objectives and Requirements stored as JSON strings
                            course.Objectives = reader.IsDBNull(reader.GetOrdinal("Objectives"))? new List<string>(): JsonConvert.DeserializeObject<List<string>>(reader.GetString(reader.GetOrdinal("Objectives")));
                            course.Requirements = reader.IsDBNull(reader.GetOrdinal("Requirements"))   ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(reader.GetString(reader.GetOrdinal("Requirements")));

                            // Full course description
                              CourseFullDescription = reader.IsDBNull(reader.GetOrdinal("CourseFullDescription"))? null: reader.GetString(reader.GetOrdinal("CourseFullDescription"));
                        }

                        // 2. Subjects
                        course.SubjectId = new List<int>();
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("SubjectId")))
                                    course.SubjectId.Add(reader.GetInt32(reader.GetOrdinal("SubjectId")));
                            }
                        }

                        // 3. Batches
                        course.Batches = new List<Batch>();
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                               

                                var batch = new Batch();


                                   batch. btachId = reader.GetInt32(reader.GetOrdinal("BatchId"));
                                batch.batchName = reader.GetString(reader.GetOrdinal("BatchName"));
                                batch.batch_classId = reader.GetInt32(reader.GetOrdinal("Batch_ClassId"));
                                batch.batch_subjectId = reader.GetInt32(reader.GetOrdinal("Batch_SubjectId"));
                                batch.batch_boardId = reader.GetInt32(reader.GetOrdinal("Batch_BoardId"));
                                batch.startDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                                batch.endDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));

                                TimeSpan startTime = reader.GetFieldValue<TimeSpan>(reader.GetOrdinal("StartTime"));
                                TimeSpan endTime = reader.GetFieldValue<TimeSpan>(reader.GetOrdinal("EndTime"));


                                batch.startTime = startTime;
                                batch.endTime = endTime;
                                  //  batch.EndTime = endTime.ToString(@"hh\:mm"),
                                
                                course.Batches.Add(batch);
                            }
                        }

                        // 4. Installments
                        course.Installments = new List<Installment>();
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var installment = new Installment
                                {
                                    TotalInstallments = reader.GetInt32(reader.GetOrdinal("TotalInstallments")).ToString(),
                                    InstallmentNumber = reader.GetInt32(reader.GetOrdinal("InstallmentNumber")),
                                    Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                    DueDaysFromStart = reader.GetInt32(reader.GetOrdinal("DueDaysFromStart")),
                                    Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks"))? null: reader.GetString(reader.GetOrdinal("Remarks"))
                                };
                                course.Installments.Add(installment);
                            }
                        }
                    }



                }



            }
            catch (Exception ex)
            {

            }

            CourseDetailsDto details = JsonConvert.DeserializeObject<CourseDetailsDto>(CourseFullDescription);
                   
        

        return new Tuple<CoursePackageDto, CourseDetailsDto>(course, details) ;
    }


        public async Task<List<AllCourseDetails>> GetAllCourses()
        {
            List<AllCourseDetails> allCourses = new List<AllCourseDetails>();

            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_Manage_CourseDetails";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    DbHelper.AddParameter(cmd, "@From", "U");
                    DbHelper.AddParameter(cmd, "@Flag", "GA");

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            AllCourseDetails data = new AllCourseDetails();

                            data.OldPrice = reader["OldPrice"]?.ToString() ?? "";
                            data.DetailId = reader["DetailId"]?.ToString() ?? "";
                            data.CourseId = reader["CourseId"]?.ToString() ?? "";
                            data.CourseName = reader["CourseName"]?.ToString() ?? "";
                            data.Price = reader["Price"]?.ToString() ?? "";
                            data.Status = reader["Status"]?.ToString() ?? "";
                            data.Description = reader["Description"]?.ToString() ?? "";
                            data.Objectives = reader["Objectives"]?.ToString() ?? "";
                            data.Requirements = reader["Requirements"]?.ToString() ?? "";
                            data.CourseLevel = reader["CourseLevel"]?.ToString() ?? "";
                              data.CourseImage = reader["CourseImage"] != DBNull.Value ? GlobalFetchPath + "CourseImages/" + reader["CourseImage"].ToString()
                                                  : "";

                            allCourses.Add(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
            }

            return allCourses;
        }




    }
}
