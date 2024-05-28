using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels
{
    public class BlogViewModel
    {
        [Key]
        public int? Id { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Blog name not specified")]
        public string? Name { get; set; }
        [Display(Name = "Mini description")]
        [Required(ErrorMessage = "Mini description not specified")]
        public string? MiniDescription { get; set; }

        [Display(Name = "Content")]
        public string? Content { get; set; }
        [Display(Name = "Image")]
        public IFormFile? File { get; set; }
        public string? Image { get; set; }
    }
}
