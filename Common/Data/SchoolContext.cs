using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext() : base()
        {

        }
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<CourseStudent> CourseStudents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseStudent>()
                .HasKey(cs => new { cs.CourseID, cs.StudentID });
            modelBuilder.Entity<CourseStudent>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(cs => cs.CourseID);
            modelBuilder.Entity<CourseStudent>()
                .HasOne(cs => cs.Student)
                .WithMany(s => s.Courses)
                .HasForeignKey(cs => cs.StudentID);
        }

    }
}
