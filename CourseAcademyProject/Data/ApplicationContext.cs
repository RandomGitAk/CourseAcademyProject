using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Test;
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CourseAcademyProject.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) {}
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Lesson> Lessons { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Test> Tests { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<Answer> Answers { get; set; } = null!;
        public DbSet<TestResult> TestResults { get; set; } = null!;
        public DbSet<UserAnswer> UserAnswers { get; set; } = null!;
        public DbSet<UserCourse> UserCourses {  get; set; } = null!;
        public DbSet<Blog> Blogs { get; set; } = null!;
        public DbSet<MainAdminPageViewModel> MainAdminPageViewModels { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Course>().Property(e => e.DateOfPublication).HasDefaultValueSql("GETDATE()");
            builder.Entity<Lesson>().Property(e=> e.DateOfPublication).HasDefaultValueSql("GETDATE()");
            builder.Entity<Comment>().Property(e=> e.DateOfPublication).HasDefaultValueSql("GETDATE()");
            builder.Entity<UserCourse>().Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
            builder.Entity<Blog>().Property(e=> e.DateOfPublication).HasDefaultValueSql("GETDATE()");
            builder.Entity<User>().Property(e => e.Image).HasDefaultValue("/userImages/defaultUser.jpg");
            builder.Entity<MainAdminPageViewModel>().HasNoKey();

            builder.Entity<UserAnswer>()
           .HasOne(ua => ua.Question)
           .WithMany(q => q.UserAnswers)
           .HasForeignKey(ua => ua.QuestionId)
           .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<UserAnswer>()
                .HasOne(ua => ua.TestResult)
                .WithMany(tr => tr.UserAnswers)
                .HasForeignKey(ua => ua.TestResultId)
                .OnDelete(DeleteBehavior.Restrict); 

            base.OnModelCreating(builder);
        }

    }
}
