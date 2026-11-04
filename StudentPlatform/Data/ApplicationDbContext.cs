using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentPlatform.Models;

namespace StudentPlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<StudentSubmission> StudentSubmissions { get; set; }
    }
}