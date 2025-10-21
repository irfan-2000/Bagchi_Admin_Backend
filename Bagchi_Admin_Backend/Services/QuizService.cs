using Bagchi_Admin_Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Xml.Linq;

namespace Bagchi_Admin_Backend.Services
{
    public class QuizService: IQuizService
    {
        private readonly ApplicationDbContext _dbContext;

        public QuizService(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public DbResponse ValidateQuiz(QuizDto quiz)
        {
            if (quiz.CourseId == 0)
                return new DbResponse { StatusCode = "400", Message = "Please select the course." };

            if (quiz.BatchId == null || quiz.BatchId.Count == 0)
                return new DbResponse { StatusCode = "400", Message = "Please select the batch." };

            if (string.IsNullOrWhiteSpace(quiz.Title))
                return new DbResponse { StatusCode = "400", Message = "Please enter the quiz title." };

            if (quiz.StartDate == DateTime.MinValue )
                return new DbResponse { StatusCode = "400", Message = "Please select the quiz start date." };

            if (quiz.EndDate == DateTime.MinValue)
                return new DbResponse { StatusCode = "400", Message = "Please select the quiz end date." };


            if (quiz.Duration == null || quiz.Duration == TimeSpan.Zero)
                return new DbResponse { StatusCode = "400", Message = "Please enter the quiz duration." };

            if (quiz.StartTime == TimeSpan.Zero)
                return new DbResponse { StatusCode = "400", Message = "Please select the quiz start time." };

            if (quiz.EndTime == TimeSpan.Zero)
                return new DbResponse { StatusCode = "400", Message = "Please select the quiz end time." };

            if (quiz.EndDate < quiz.StartDate)
                return new DbResponse { StatusCode = "400", Message = "Quiz end date must be after start date." };



            if (quiz.Status <= 0) // Assuming Status: 1=Active/Upcoming, 2=Inactive, 3=Completed
                return new DbResponse { StatusCode = "400", Message = "Please select the quiz status." };


            if (quiz.Questions == null || quiz.Questions.Count == 0)
                return new DbResponse { StatusCode = "400", Message = "Please add at least one question." };

            if (quiz.TotalQuestions <= 0)
                return new DbResponse { StatusCode = "400", Message = "Please enter total number of questions." };

            if (quiz.MarksPerQuestion <= 0)
                return new DbResponse { StatusCode = "400", Message = "Please enter marks per question." };

            if (quiz.AllowNegative != true && quiz.AllowNegative != false)
                return new DbResponse { StatusCode = "400", Message = "AllowNegative can't be undefined." };

            if (quiz.AllowNegative && quiz.Negativemarks<= 0)
                return new DbResponse { StatusCode = "400", Message = "Negative marks are required else uncheck the allow negative marking." };

            if (quiz.ShuffleQuestions != true && quiz.ShuffleQuestions != false)
                return new DbResponse { StatusCode = "400", Message = "ShuffleQuestions can't be undefined." };

            if (quiz.TotalMarks <= 0)
                return new DbResponse { StatusCode = "400", Message = "TotalMarks can't be undefined or 0." };

            if (quiz.Subjects == null)
                return new DbResponse { StatusCode = "400", Message = "Subjects is required." };

            if (quiz.AllowSkip != true && quiz.AllowSkip != false)
                return new DbResponse { StatusCode = "400", Message = "AllowSkip can't be undefined." };



            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                var q = quiz.Questions[i];

                if (string.IsNullOrWhiteSpace(q.Question_text))
                    return new DbResponse { StatusCode = "400", Message = $"Question {i + 1} is missing text." };

                if (q.Options == null || q.Options.Count == 0)
                    return new DbResponse { StatusCode = "400", Message = $"Question {i + 1} has no options." };

                for (int j = 0; j < q.Options.Count; j++)
                {
                    if (string.IsNullOrWhiteSpace(q.Options[j].Text))
                        return new DbResponse { StatusCode = "400", Message = $"Option {j + 1} of Question {i + 1} is missing." };
                }

                bool hasCorrect = q.Options.Any(opt => opt.IsCorrect);
                if (!hasCorrect)
                    return new DbResponse { StatusCode = "400", Message = $"Question {i + 1} does not have a correct answer selected." };
            }

            return new DbResponse { StatusCode = "200", Message = "Quiz is valid." };
        }

        public async Task<bool> CreateQuizAsync(QuizDto dto)
        {    

            var questionsXml = BuildQuestionsXml(dto.Questions);

             await _dbContext.Database.OpenConnectionAsync();
            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "usp_InsertQuizWithBatchesAndQuestions";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters

                    //batch.startTime.ToString(@"hh\:mm"))
                    DbHelper.AddParameter(cmd, "@QuizId", dto.QuizId);
                    DbHelper.AddParameter(cmd, "@CourseId",dto.CourseId);
                    DbHelper.AddParameter(cmd, "@Title", dto.Title);
                    DbHelper.AddParameter(cmd, "@Status", dto.Status);


                    DbHelper.AddParameter(cmd, "@TotalQuestions", Convert.ToInt32(dto.TotalQuestions));
                    DbHelper.AddParameter(cmd, "@MarksPerQuestion", Convert.ToDecimal(dto.MarksPerQuestion));
                    DbHelper.AddParameter(cmd, "@AllowNegative", dto.AllowNegative ? 1 : 0);
                    DbHelper.AddParameter(cmd, "@NegativeMarking", Convert.ToDecimal(dto.Negativemarks));
                    DbHelper.AddParameter(cmd, "@ShuffleQuestions", dto.ShuffleQuestions ? 1 : 0);
                    DbHelper.AddParameter(cmd, "@TotalMarks", Convert.ToDecimal(dto.TotalMarks));
                    DbHelper.AddParameter(cmd, "@Subjects", dto.Subjects ?? string.Empty);
                    DbHelper.AddParameter(cmd, "@AllowSkip", dto.AllowSkip ? 1 : 0);


                    DbHelper.AddParameter(cmd, "@StartDateTime", DateTime.ParseExact(dto.StartDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture));
                    DbHelper.AddParameter(cmd, "@EndDateTime", DateTime.ParseExact(dto.EndDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture));
                    DbHelper.AddParameter(cmd, "@Duration", dto.Duration.ToString(@"hh\:mm\:ss"));
                    DbHelper.AddParameter(cmd, "@BatchIdsCsv", string.Join(",", dto.BatchId));
                    DbHelper.AddParameter(cmd, "@QuestionsXml", questionsXml);
                    DbHelper.AddParameter(cmd, "@Starttime", dto.StartTime.ToString(@"hh\:mm\:ss"));
                    DbHelper.AddParameter(cmd, "@Endtime", dto.EndTime.ToString(@"hh\:mm\:ss"));

                   int result = await cmd.ExecuteNonQueryAsync();

                    if(result > 0)
                    {
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error creating quiz: " + ex.Message);
            }
            return false;
        }

        private string BuildQuestionsXml(List<QuizQuestionDto> questions)
        {
            // XML structure:
            // <Questions>
            //   <Question text="...">
            //     <Option isCorrect="1">Answer text</Option>
            //     ...
            //   </Question>
            // </Questions>\\
 
            XElement root = new XElement("Questions");

            foreach(var q in questions)
            {
                XElement questionElem = new XElement("Question");
                questionElem.Add(new XAttribute("text", q.Question_text ?? string.Empty));

                if(q.Options != null)
                {
                    foreach(var o in q.Options)
                    {
                        XElement optionElem = new XElement("option",o.Text ?? string.Empty);
                        optionElem.Add(new XAttribute("iscorrect", o.IsCorrect));
                        questionElem.Add(optionElem);

                    }
                }

                root.Add(questionElem);

            }
            XDocument doc = new XDocument(root); 
            return doc.ToString(SaveOptions.DisableFormatting);
        }


        public async Task<QuizDto> GetQuizDataById(string flag, int quizid = 0)
        {
            QuizDto quizMasterData = new QuizDto();
            quizMasterData.Questions = new List<QuizQuestionDto>();

            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "usp_getquizdata";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    DbHelper.AddParameter(cmd, "@Flag", flag);

                    if (quizid > 0)
                        DbHelper.AddParameter(cmd, "@quizid", quizid);

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                         if (await reader.ReadAsync())
                        {
                            quizMasterData.QuizId = Convert.IsDBNull(reader["QuizId"]) ? 0 : Convert.ToInt32(reader["QuizId"]);
                            quizMasterData.CourseId = Convert.IsDBNull(reader["CourseId"]) ? 0 : Convert.ToInt32(reader["CourseId"]);
                            quizMasterData.Title = Convert.IsDBNull(reader["Title"]) ? "" : reader["Title"].ToString();
                            quizMasterData.StartDate = Convert.IsDBNull(reader["StartDateTime"]) ? DateTime.MinValue : Convert.ToDateTime(reader["StartDateTime"]);
                            quizMasterData.EndDate = Convert.IsDBNull(reader["EndDateTime"]) ?  DateTime.MinValue :Convert.ToDateTime(reader["EndDateTime"]);
                            quizMasterData.Duration = reader["Duration"] is DBNull ? TimeSpan.MinValue : (TimeSpan)reader["Duration"];
                            quizMasterData.StartTime = reader["Duration"] is DBNull ? TimeSpan.MinValue : (TimeSpan)reader["StartTime"];                            
                            quizMasterData.EndTime = Convert.IsDBNull(reader["EndTime"]) ? TimeSpan.MinValue : (TimeSpan)reader["EndTime"];
                            quizMasterData.Status = Convert.IsDBNull(reader["Status"]) ? 0 : Convert.ToInt32(reader["Status"]);
                            var batchIdsStr = Convert.IsDBNull(reader["BatchIds"]) ? "" : reader["BatchIds"].ToString();
                            quizMasterData.BatchId = string.IsNullOrWhiteSpace(batchIdsStr)
                                ? new List<string>()
                                : batchIdsStr.Split(',').Select(b => b.Trim()).ToList();
                        }

                        // Move to 2nd Result Set: Questions
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var question = new QuizQuestionDto();


                                question.QuestionId = Convert.IsDBNull(reader["QuestionId"]) ? 0 : Convert.ToInt32(reader["QuestionId"]);
                                question.Question_text = Convert.IsDBNull(reader["QuestionText"]) ? "" : reader["QuestionText"].ToString();
                                question.Options = new List<QuizOptionDto>();
                                
                                quizMasterData.Questions.Add(question);
                            }
                        }
                         
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var option = new QuizOptionDto();
                                option.QuestionId = Convert.IsDBNull(reader["QuestionId"]) ? 0 : Convert.ToInt32(reader["QuestionId"]);
                                option.Text = Convert.IsDBNull(reader["OptionText"]) ? "" : reader["OptionText"].ToString();
                                option.IsCorrect = Convert.IsDBNull(reader["IsCorrect"]) ? false : Convert.ToBoolean(reader["IsCorrect"]);

                                quizMasterData.Questions.FirstOrDefault(q => q.QuestionId == option.QuestionId)?.Options?.Add(option);


                                // var question = quizMasterData.Questions.FirstOrDefault(q => q.QuestionId == option.QuestionId).Options.Add(option);
                                //if (question != null)
                                //{
                                //    question.Options.Add(option);
                                //}
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            { 
                
            }
            finally
            {
                await _dbContext.Database.CloseConnectionAsync();
            }

            return quizMasterData;
        }


        public async Task<List<QuizDto>> GetAllQuizzes()
        {
            List<QuizDto> quizzes = new List<QuizDto>();

            try
            {

                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "usp_getquizdata";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    DbHelper.AddParameter(cmd, "@Flag", "All");
                     
                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var quiz = new QuizDto();
                           quiz.QuizId = Convert.IsDBNull(reader["QuizId"]) ? 0 : Convert.ToInt32(reader["QuizId"]);
                           quiz.CourseId = Convert.IsDBNull(reader["CourseId"]) ? 0 : Convert.ToInt32(reader["CourseId"]);
                           quiz.Title = Convert.IsDBNull(reader["Title"]) ? "" : reader["Title"].ToString();
                           quiz.StartDate = Convert.IsDBNull(reader["StartDateTime"]) ? DateTime.MinValue : Convert.ToDateTime(reader["StartDateTime"]);
                           quiz.EndDate = Convert.IsDBNull(reader["EndDateTime"]) ? DateTime.MinValue : Convert.ToDateTime(reader["EndDateTime"]);
                           quiz.Duration = reader["Duration"] is DBNull ? TimeSpan.MinValue : (TimeSpan)reader["Duration"];
                           quiz.StartTime = reader["Duration"] is DBNull ? TimeSpan.MinValue : (TimeSpan)reader["StartTime"];
                           quiz.EndTime = Convert.IsDBNull(reader["EndTime"]) ? TimeSpan.MinValue : (TimeSpan)reader["EndTime"];
                            quiz.Status = Convert.IsDBNull(reader["Status"]) ? 0 : Convert.ToInt32(reader["Status"]);

                            quizzes.Add(quiz);
                        }
                    }
                }
            }catch(Exception ex)
            {

            }

            return quizzes;
        }

    }
}
