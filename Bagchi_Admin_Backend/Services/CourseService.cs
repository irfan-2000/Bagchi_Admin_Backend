using Bagchi_Admin_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Bagchi_Admin_Backend.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _config;

        public CourseService(ApplicationDbContext dbContext, IConfiguration Iconfiguration)
        {
            _dbContext = dbContext;
            _config = Iconfiguration;

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

                         DbHelper.AddParameter(cmd, "@ClassId", dto.ClassId);
                        DbHelper.AddParameter(cmd, "@Subjectname", dto.SubjectName);
                        DbHelper.AddParameter(cmd, "@Status", dto.Status);
                        DbHelper.AddParameter(cmd, "@BoardId", dto.BoardId);
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
                    cmd.CommandText = "sp_Manage_AvailableBoards";
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

    }
}
