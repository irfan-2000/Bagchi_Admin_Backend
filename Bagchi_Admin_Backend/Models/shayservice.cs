using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Bagchi_Admin_Backend.Models
{
    public class shayservice:IShayariService
    {

        private readonly ShayDbContext _dbContext;
        private readonly IConfiguration _config;
        public string GlobalFetchPath { get; set; }
        public shayservice(ShayDbContext dbContext, IConfiguration Iconfiguration)
        {
            _dbContext = dbContext;
            _config = Iconfiguration;

            GlobalFetchPath = _config["GlobalFetchPath"];

        }

        public async Task<List<ShayariDto>> GetAllShayaris()
        {
            List<ShayariDto> shayaris = new List<ShayariDto>();

            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_Shayari_CRUD";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Pass flag to get all shayaris
                    DbHelper.AddParameter(cmd, "@Flag", "GET_ALL");

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ShayariDto data = new ShayariDto();

                            data.ShayariId = Convert.IsDBNull(reader["ShayariId"]) ? 0 : Convert.ToInt32(reader["ShayariId"]);
                            data.CategoryId = Convert.IsDBNull(reader["CategoryId"]) ? 0 : Convert.ToInt32(reader["CategoryId"]);
                            data.Title = Convert.IsDBNull(reader["Title"]) ? "" : reader["Title"].ToString();
                            data.ShayariText = Convert.IsDBNull(reader["ShayariText"]) ? "" : reader["ShayariText"].ToString();
                            data.Author = Convert.IsDBNull(reader["Author"]) ? "" : reader["Author"].ToString();
                            data.LikesCount = Convert.IsDBNull(reader["LikesCount"]) ? 0 : Convert.ToInt32(reader["LikesCount"]);
                            data.IsActive = Convert.IsDBNull(reader["IsActive"]) ? false : Convert.ToBoolean(reader["IsActive"]);
                            data.CreatedDate = Convert.IsDBNull(reader["CreatedDate"]) ? DateTime.MinValue : Convert.ToDateTime(reader["CreatedDate"]);

                            shayaris.Add(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Optional: log the exception
            }

            return shayaris;
        }



        public async Task<ShayariDto> GetShayariById(int shayariId)
        {
            ShayariDto shayari = null;

            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_Shayari_CRUD";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    DbHelper.AddParameter(cmd, "@Flag", "GET_BY_ID");
                    DbHelper.AddParameter(cmd, "@ShayariId", shayariId);

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            shayari = new ShayariDto
                            {
                                ShayariId = Convert.IsDBNull(reader["ShayariId"]) ? 0 : Convert.ToInt32(reader["ShayariId"]),
                                CategoryId = Convert.IsDBNull(reader["CategoryId"]) ? 0 : Convert.ToInt32(reader["CategoryId"]),
                                Title = Convert.IsDBNull(reader["Title"]) ? "" : reader["Title"].ToString(),
                                ShayariText = Convert.IsDBNull(reader["ShayariText"]) ? "" : reader["ShayariText"].ToString(),
                                Author = Convert.IsDBNull(reader["Author"]) ? "" : reader["Author"].ToString(),
                                LikesCount = Convert.IsDBNull(reader["LikesCount"]) ? 0 : Convert.ToInt32(reader["LikesCount"]),
                                IsActive = Convert.IsDBNull(reader["IsActive"]) ? false : Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.IsDBNull(reader["CreatedDate"]) ? DateTime.MinValue : Convert.ToDateTime(reader["CreatedDate"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Optional: log exception
            }

            return shayari;
        }
         
        public async Task<List<ShayariDto>> GetShayarisByCategory(int categoryId)
        {
            List<ShayariDto> shayaris = new List<ShayariDto>();

            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_Shayari_CRUD";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    DbHelper.AddParameter(cmd, "@Flag", "GET_BY_CATEGORY");
                    DbHelper.AddParameter(cmd, "@CategoryId", categoryId);

                    await _dbContext.Database.OpenConnectionAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ShayariDto data = new ShayariDto
                            {
                                ShayariId = Convert.IsDBNull(reader["ShayariId"]) ? 0 : Convert.ToInt32(reader["ShayariId"]),
                                CategoryId = Convert.IsDBNull(reader["CategoryId"]) ? 0 : Convert.ToInt32(reader["CategoryId"]),
                                Title = Convert.IsDBNull(reader["Title"]) ? "" : reader["Title"].ToString(),
                                ShayariText = Convert.IsDBNull(reader["ShayariText"]) ? "" : reader["ShayariText"].ToString(),
                                Author = Convert.IsDBNull(reader["Author"]) ? "" : reader["Author"].ToString(),
                                LikesCount = Convert.IsDBNull(reader["LikesCount"]) ? 0 : Convert.ToInt32(reader["LikesCount"]),
                                IsActive = Convert.IsDBNull(reader["IsActive"]) ? false : Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.IsDBNull(reader["CreatedDate"]) ? DateTime.MinValue : Convert.ToDateTime(reader["CreatedDate"])
                            };

                            shayaris.Add(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Optional: log exception
            }

            return shayaris;
        }


        public async Task<int> InsertUser(UserDto user)
        {
            int newUserId = 0;

            try
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.CommandText = "sp_User_CRUD";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parameters
                    DbHelper.AddParameter(cmd, "@Flag", "INSERT");
                    DbHelper.AddParameter(cmd, "@UserName", user.UserName);
                    DbHelper.AddParameter(cmd, "@Email", user.Email);
                    DbHelper.AddParameter(cmd, "@IPAddress", user.IPAddress);
                    DbHelper.AddParameter(cmd, "@Country", user.Country);

                    await _dbContext.Database.OpenConnectionAsync();

                    // Execute and get newly created UserId
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null && int.TryParse(result.ToString(), out int id))
                    {
                        newUserId = id;
                    }
                }
            }
            catch (Exception ex)
            {
                // Optional: log exception
            }

            return newUserId;
        }

    }
}
