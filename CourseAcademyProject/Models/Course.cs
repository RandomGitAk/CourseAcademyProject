using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.Models
{
    public enum DifficultyLevel
    {
        Beginner,
        Middle,
        Advanced
    }

    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AboutCourse { get; set; }
        public string Prerequisites { get; set; }
        public string YoullLearn {  get; set; }
        public string? Image { get; set; }
        public DateTime DateOfPublication { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public string VideoUrl {  get; set; }
        public decimal? Price {  get; set; }
        public string? PriceId { get; set; }
        public decimal? Discount {  get; set; }
        public byte? DaysDiscount {  get; set; }
        public string Hourpassed { get; set; }
        public string Language {  get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public IEnumerable<Lesson>? Lessons { get; set; }
        public IEnumerable<Comment>? Comments { get; set; }
        public IEnumerable<UserCourse>? CoursesUser { get; set; }
        public IEnumerable<Test.Test>? Tests { get; set; }

    }
}
