using backendAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace backendAPI
{
    public class DatabaseContextCla : DbContext
    {
        public DatabaseContextCla(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<LocationModel> Location { get; set; }
    }
}  


