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
                //return new DbResponse { StatusCode = "400", Message = "Please add at least one question." };

            if (quiz.TotalQuestions <= 0)
                return new DbResponse { StatusCode = "400", Message = "Please enter total number of questions." };

            if (quiz.MarksPerQuestion <= 0)
               // return new DbResponse { StatusCode = "400", Message = "Please enter marks per question." };

            if (quiz.AllowNegative != true && quiz.AllowNegative != false)
                //return new DbResponse { StatusCode = "400", Message = "AllowNegative can't be undefined." };

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



            //for (int i = 0; i < quiz.Questions.Count; i++)
            //{
            //    var q = quiz.Questions[i];

            //    if (string.IsNullOrWhiteSpace(q.Question_text))
            //        return new DbResponse { StatusCode = "400", Message = $"Question {i + 1} is missing text." };

            //    if (q.Options == null || q.Options.Count == 0)
            //        return new DbResponse { StatusCode = "400", Message = $"Question {i + 1} has no options." };

            //    for (int j = 0; j < q.Options.Count; j++)
            //    {
            //        if (string.IsNullOrWhiteSpace(q.Options[j].Text))
            //            return new DbResponse { StatusCode = "400", Message = $"Option {j + 1} of Question {i + 1} is missing." };
            //    }

            //    bool hasCorrect = q.Options.Any(opt => opt.IsCorrect);
            //    if (!hasCorrect)
            //        return new DbResponse { StatusCode = "400", Message = $"Question {i + 1} does not have a correct answer selected." };
            //}

            return new DbResponse { StatusCode = "200", Message = "Quiz is valid." };
        }




        public async Task<bool> CreateQuizAsync(QuizDto dto ,List<Question> question)
        {    

            var questionsXml = BuildQuestionsXml(question);

             await _dbContext.Database.OpenConnectionAsync();
            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "usp_InsertQuizWithBatchesAndQuestions";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
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


                    //DbHelper.AddParameter(cmd, "@StartDateTime", DateTime.ParseExact(dto.StartDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture));
                    //DbHelper.AddParameter(cmd, "@EndDateTime", DateTime.ParseExact(dto.EndDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture));

                    DbHelper.AddParameter(cmd, "@StartDateTime", dto.StartDate. ToString("yyyy-MM-dd"));
                    DbHelper.AddParameter(cmd, "@EndDateTime", dto.EndDate.ToString("yyyy-MM-dd"));

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


        private string BuildQuestionsXml(List<Question> questions)
        {
            XElement root = new XElement("Questions");

            foreach (var q in questions)
            {
                XElement questionElem = new XElement("Question");
                questionElem.Add(new XAttribute("text", q.QuestionText ?? string.Empty));
                questionElem.Add(new XAttribute("isNumerical", q.IsNumerical ?? "0"));
                questionElem.Add(new XAttribute("positiveMarks", q.PositiveMarks ));
                questionElem.Add(new XAttribute("negativeMarks", q.NegativeMarks));
                // Add image names if available
                // Add image list (comma-separated)
                if (q.ImagePaths != null && q.ImagePaths.Any())
                {
                    string imageList = string.Join(",", q.ImagePaths);
                    questionElem.Add(new XAttribute("images", imageList));
                }

                if (q.IsNumerical == "1")
                {
                    // Add numerical answer
                    questionElem.Add(new XElement("NumericalAnswer", q.NumericalAnswer ?? string.Empty));
                }
                else
                {
                    // MCQ: normalize correct answers (A,B,C,D)
                  //  var correctAnswers = (q.CorrectAnswers ?? "")
                    //    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                      //  .Select(a => a.Trim().ToUpper())
                        //.ToList();

                    var correctAnswers = q.CorrectAnswers
                      .Select(a => a.Trim().ToUpper())
                      .ToList();

                    XElement optionElem1 = new XElement("Option", q.OptionA ?? string.Empty);
                    XElement optionElem2 = new XElement("Option", q.OptionB ?? string.Empty);
                    XElement optionElem3 = new XElement("Option", q.OptionC ?? string.Empty);
                    XElement optionElem4 = new XElement("Option", q.OptionD ?? string.Empty);

                    optionElem1.Add(new XAttribute("iscorrect", correctAnswers.Contains("A") ? 1 : 0));
                    optionElem2.Add(new XAttribute("iscorrect", correctAnswers.Contains("B") ? 1 : 0));
                    optionElem3.Add(new XAttribute("iscorrect", correctAnswers.Contains("C") ? 1 : 0));
                    optionElem4.Add(new XAttribute("iscorrect", correctAnswers.Contains("D") ? 1 : 0));

                    questionElem.Add(optionElem1);
                    questionElem.Add(optionElem2);
                    questionElem.Add(optionElem3);
                    questionElem.Add(optionElem4);
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

                    DbHelper.AddParameter(cmd, "@Flag", flag);
                    if (quizid > 0)
                        DbHelper.AddParameter(cmd, "@quizid", quizid);

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        // -------------------- MASTER DATA --------------------
                        if (await reader.ReadAsync())
                        {
                            quizMasterData.QuizId = Convert.ToInt32(reader["QuizId"]);
                            quizMasterData.CourseId = Convert.ToInt32(reader["CourseId"]);
                            quizMasterData.Title = reader["Title"]?.ToString() ?? "";
                            quizMasterData.StartDate = Convert.ToDateTime(reader["StartDateTime"]);
                            quizMasterData.EndDate = Convert.ToDateTime(reader["EndDateTime"]);
                            quizMasterData.Duration = (TimeSpan)reader["Duration"];
                            quizMasterData.StartTime = (TimeSpan)reader["StartTime"];
                            quizMasterData.EndTime = (TimeSpan)reader["EndTime"];
                            quizMasterData.Status = Convert.ToInt32(reader["Status"]);

                            var batchIdsStr = reader["BatchIds"]?.ToString() ?? "";
                            quizMasterData.BatchId = string.IsNullOrWhiteSpace(batchIdsStr)
                                ? new List<string>()
                                : batchIdsStr.Split(',').Select(b => b.Trim()).ToList();
                        }

                        // -------------------- QUESTIONS --------------------
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var question = new QuizQuestionDto();

                                question.QuestionId = Convert.ToInt32(reader["QuestionId"]);
                                question.QuestionText = reader["QuestionText"]?.ToString() ?? "";
                                question.IsNumerical = reader["IsNumerical"]?.ToString() ?? "0";
                                question.PositiveMarks = Convert.ToString(reader["PositiveMarks"]);
                                question.NegativeMarks = Convert.ToString(reader["NegativeMarks"]);

                                string images = reader["Images"]?.ToString() ?? "";
                                question.ImagePaths = string.IsNullOrWhiteSpace(images)
                                    ? new List<string>()
                                    : images.Split(',').ToList();

                                question.NumericalAnswer = reader["NumericalAnswer"]?.ToString() ?? "";

                                question.OptionA = "";
                                question.OptionB = "";
                                question.OptionC = "";
                                question.OptionD = "";
                                question.CorrectAnswer = "";

                                question.Options = new List<QuizOptionDto>();

                                quizMasterData.Questions.Add(question);
                            }
                        }

                        // -------------------- OPTIONS --------------------
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int qid = Convert.ToInt32(reader["QuestionId"]);
                                var q = quizMasterData.Questions.FirstOrDefault(x => x.QuestionId == qid);

                                if (q == null) continue;

                                string text = reader["OptionText"]?.ToString() ?? "";
                                bool isCorrect = Convert.ToBoolean(reader["IsCorrect"]);

                                // Map OptionA/B/C/D
                                if (string.IsNullOrEmpty(q.OptionA)) q.OptionA = text;
                                else if (string.IsNullOrEmpty(q.OptionB)) q.OptionB = text;
                                else if (string.IsNullOrEmpty(q.OptionC)) q.OptionC = text;
                                else if (string.IsNullOrEmpty(q.OptionD)) q.OptionD = text;

                                // Correct Answer letter
                                if (isCorrect)
                                {
                                    if (q.OptionA == text) q.CorrectAnswer = "A";
                                    else if (q.OptionB == text) q.CorrectAnswer = "B";
                                    else if (q.OptionC == text) q.CorrectAnswer = "C";
                                    else if (q.OptionD == text) q.CorrectAnswer = "D";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception
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


        public DbResponse ValidateUploadedQuestions(List<Question> questions)
        {
            if (questions == null || questions.Count == 0)
                return new DbResponse { StatusCode = "400", Message = "No questions found in document." };

            for (int i = 0; i < questions.Count; i++)
            {
                var q = questions[i];

                // 1. QuestionId must be present
                if (q.QuestionId <= 0)
                {
                    return new DbResponse
                    {
                        StatusCode = "400",
                        Message = $"QuestionId missing for row {i + 1}."
                    };
                }

                // 2. Question text is mandatory
                if (string.IsNullOrWhiteSpace(q.QuestionText))
                {
                    return new DbResponse
                    {
                        StatusCode = "400",
                        Message = $"Question {q.QuestionId}: Question text is missing."
                    };
                }

                // Check if at least one option exists
                bool hasOption = false;
                if (!string.IsNullOrWhiteSpace(q.OptionA)) hasOption = true;
                if (!string.IsNullOrWhiteSpace(q.OptionB)) hasOption = true;
                if (!string.IsNullOrWhiteSpace(q.OptionC)) hasOption = true;
                if (!string.IsNullOrWhiteSpace(q.OptionD)) hasOption = true;

                // 3. At least 1 option OR IsNumerical = true
                if (!hasOption && q.IsNumerical == "0")
                {
                    return new DbResponse
                    {
                        StatusCode = "400",
                        Message = $"Question {q.QuestionId}: Must have options OR be numerical."
                    };
                }

                // 4. Cannot have both options + numerical
                if (hasOption && (q.IsNumerical == "1" || Convert.ToInt32(q.IsNumerical) > 0))
                {
                    return new DbResponse
                    {
                        StatusCode = "400",
                        Message = $"Question {q.QuestionId}: Cannot be both MCQ and Numerical."
                    };
                }

                // 5. If MCQ → correct answer must exist
                if (hasOption && string.IsNullOrWhiteSpace(q.CorrectAnswers.ToString()))
                {
                    return new DbResponse
                    {
                        StatusCode = "400",
                        Message = $"Question {q.QuestionId}: Correct answer missing for MCQ."
                    };
                }

                // 6. If Numerical → answer required
                if ((q.IsNumerical == "1" || Convert.ToInt32(q.IsNumerical) > 0) && string.IsNullOrWhiteSpace(q.NumericalAnswer))
                {
                    return new DbResponse
                    {
                        StatusCode = "400",
                        Message = $"Question {q.QuestionId}: Numerical answer missing."
                    };
                }

                // 7. Positive marking required
                if (q.PositiveMarks <= 0)
                {
                    return new DbResponse
                    {
                        StatusCode = "400",
                        Message = $"Question {q.QuestionId}: Positive marks must be greater than 0."
                    };
                }

                // 8. Negative marks empty = treat as 0 → allowed
                // If negative marks is < 0 or weird, you can add more checks
            }

            return new DbResponse
            {
                StatusCode = "200",
                Message = "All uploaded questions are valid."
            };
        }


    }
}
