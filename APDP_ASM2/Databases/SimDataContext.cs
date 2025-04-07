using APDP_ASM2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using APDP_ASM2.Databases;

namespace APDP_ASM2.Databases
{
    public class SimDataContext : DbContext
    {
        public SimDataContext(DbContextOptions<SimDataContext> options) : base(options)
        {
        }
        public DbSet<User> Users {  get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }

    }
}
