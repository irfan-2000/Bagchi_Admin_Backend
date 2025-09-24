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










    }
}
