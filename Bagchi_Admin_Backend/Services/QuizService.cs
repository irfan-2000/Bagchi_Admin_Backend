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

            if (quiz.Questions == null || quiz.Questions.Count == 0)
                return new DbResponse { StatusCode = "400", Message = "Please add at least one question." };

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
                    DbHelper.AddParameter(cmd, "@StartDateTime", dto.StartTime.ToString(@"hh\:mm\:ss"));
                    DbHelper.AddParameter(cmd, "@EndDateTime", dto.EndTime.ToString(@"hh\:mm\:ss"));
                    DbHelper.AddParameter(cmd, "@Duration", dto.Duration.ToString(@"hh\:mm\:ss"));
                    DbHelper.AddParameter(cmd, "@BatchIdsCsv", string.Join(",", dto.BatchId));
                    DbHelper.AddParameter(cmd, "@QuestionsXml", questionsXml);

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

        private DateTime? CombineDateAndTime(string dateStr, string timeStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr) || string.IsNullOrWhiteSpace(timeStr)) return null;
            // dateStr expected "yyyy-MM-dd"
            // timeStr expected "HH:mm" or "HH:mm:ss"
            if (!DateTime.TryParseExact(dateStr, new[] { "yyyy-MM-dd", "yyyy-M-d" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var datePart))
                return null;

            if (!TimeSpan.TryParse(timeStr, out var timePart))
                return null;

            return datePart.Date.Add(timePart);
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


    }
}
