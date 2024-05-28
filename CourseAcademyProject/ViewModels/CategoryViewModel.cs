using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels
{
    public class CategoryViewModel
    {
        [Key]
        public int? Id { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Category name not specified")]
        public string? Name { get; set; }
        [Display(Name = "Description")]
        public string? Description { get; set; }
        [Display(Name = "Image")]
        public IFormFile? File { get; set; }
        public string? Image { get; set; }

    }
}
