using CourseAcademyProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CourseAcademyProject.ViewModels
{
    public class CourseViewModel
    {
        [Key]
        public int? Id { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "No name given")]
        public string? Name { get; set; }

        [Display(Name = "About course")]
        [Required(ErrorMessage = "No description given")]
        public string? AboutCourse { get; set; }

        [Display(Name = "Prerequisites")]
        [Required(ErrorMessage = "No description given")]
        public string? Prerequisites { get; set; }

        [Display(Name = "You`ll learn")]
        [Required(ErrorMessage = "No description given")]
        public string? YoullLearn { get; set; }

        [Display(Name = "Image")]
        public IFormFile? File { get; set; }
        public string? Image { get; set; }
        public DateTime DateOfPublication { get; set; }

        [Display(Name = "Difficulty level")]
        public DifficultyLevel DifficultyLevel { get; set; }

        [Display(Name = "Video url", Prompt = "https://www.youtube.com/")]
        [Required(ErrorMessage = "No video url given")]
        public string? VideoUrl { get; set; }

        [Display(Name = "Price")]
        public decimal? Price { get; set; }

        [Display(Name = "Price id from https://dashboard.stripe.com")]
        public string? PriceId { get; set; }

        [Display(Name = "Discount", Prompt = "Enter the price of discount")]
        public decimal? Discount { get; set; }

        [Display(Name = "Days discount", Prompt = "Enter the number of days")]
        public byte? DaysDiscount { get; set; }


        [Display(Name = "Hour passed", Prompt = "05:30")]
        [Required(ErrorMessage = "No hour passed given")]
        public string? Hourpassed { get; set; }


        [Display(Name = "Language", Prompt = "Eanglish")]
        [Required(ErrorMessage = "No language given")]
        public string? Language { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "Categories")]
        public SelectList? AllCategories { get; set; } = null!;
    }
}
