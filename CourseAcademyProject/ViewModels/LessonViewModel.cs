using CourseAcademyProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels
{
    public class LessonViewModel
    {
        [Key]
        public int? Id { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "No name given")]
        public string? Title { get; set; }

        [Display(Name = "Information")]
        [Required(ErrorMessage = "No information given")]
        public string? Information { get; set; }

        [Display(Name = "Lesson content")]
        [Required(ErrorMessage = "No content given")]
        public string LessonContent { get; set; }

        [Display(Name = "Video url")]
        [Required(ErrorMessage = "No video url given")]
        public string VideoUrl { get; set; }

        [Display(Name = "Material url")]
        [Required(ErrorMessage = "No material url given")]
        public string MaterialsUrl { get; set; }

        [Display(Name = "File with cours material")]
        public IFormFile? File { get; set; }
        public string? FileLessonContent { get; set; }

        public DateTime DateOfPublication { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [Display(Name = "Courses")]
        public SelectList? AllCourses { get; set; } = null!;
    }
}
