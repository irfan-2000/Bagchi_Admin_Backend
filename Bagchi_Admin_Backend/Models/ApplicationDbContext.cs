using Microsoft.EntityFrameworkCore;

namespace Bagchi_Admin_Backend.Models
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }



    }
}
