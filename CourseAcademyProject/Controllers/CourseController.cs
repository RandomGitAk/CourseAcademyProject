using CourseAcademyProject.Data.Helpers;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseAcademyProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CourseController : Controller
    {
        private readonly ICourse _courses;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly ICategory _categories;

        public CourseController(ICourse courses, IWebHostEnvironment appEnvironment, ICategory categories)
        {
            _courses = courses;
            _appEnvironment = appEnvironment;
            _categories = categories;
        }

        [Route("/panel/courses")]
        public IActionResult Courses(QueryOptions options)
        {
            return View(_courses.GetAllCourses(options));
        }


        [Route("/panel/delete-course")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var currentCourse = await _courses.GetCourseByIdAsync(courseId);
            if (currentCourse != null)
            {
                if (currentCourse.Image != null)
                {
                    if (System.IO.File.Exists(_appEnvironment.WebRootPath + currentCourse.Image))
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath + currentCourse.Image);
                    }
                }
                await _courses.DeleteCourseAsync(currentCourse);
            }
            return Ok();
        }

        [Route("/panel/create-course")]
        [HttpGet]
        public async Task<IActionResult> CreateCourse()
        {
            var categories = await _categories.GetAllCategoriesAsync();
            return View(new CourseViewModel
            {
                AllCategories = new SelectList(categories, "Id", "Name")
            });
        }

        [Route("/panel/create-course")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateCourse(CourseViewModel courseViewModel)
        {
            if (ModelState.IsValid)
            {
                var currentCourse = new Course
                {
                    Name = courseViewModel.Name!,
                    AboutCourse = courseViewModel.AboutCourse!,
                    DifficultyLevel = courseViewModel.DifficultyLevel,
                    Prerequisites = courseViewModel.Prerequisites!,
                    YoullLearn = courseViewModel.YoullLearn!,
                    Hourpassed = courseViewModel.Hourpassed!,
                    Price = courseViewModel.Price,
                    PriceId = courseViewModel.PriceId,
                    Discount = courseViewModel.Discount,
                    DaysDiscount = courseViewModel.DaysDiscount,
                    Language = courseViewModel.Language!,
                    CategoryId = courseViewModel.CategoryId,
                    VideoUrl = courseViewModel.VideoUrl!
                };
                if (courseViewModel.File != null)
                {
                    string fileName = courseViewModel.File.FileName;
                    string filePath = FileService.СreateFilePathFromFileName(fileName,foldername:"courseImages");
                    currentCourse.Image = filePath;
                    await FileService.SaveFile(filePath, courseViewModel.File, _appEnvironment);
                }
                await _courses.AddCourseAsync(currentCourse);
                return RedirectToAction(nameof(Courses));
            }
            return View(courseViewModel);
        }

        [Route("/panel/edit-course")]
        [HttpGet]
        public async Task<IActionResult> EditCourse(int courseId)
        {
            var currentCourse = await _courses.GetCourseByIdAsync(courseId);
            if (currentCourse != null)
            {
                var categories = await _categories.GetAllCategoriesAsync();
                CourseViewModel courseViewModel = new CourseViewModel
                {
                    Id = currentCourse.Id,
                    Name = currentCourse.Name,
                    AboutCourse = currentCourse.AboutCourse,
                    DifficultyLevel = currentCourse.DifficultyLevel,
                    Language = currentCourse.Language,
                    VideoUrl = currentCourse.VideoUrl,
                    Hourpassed = currentCourse.Hourpassed,
                    Discount = currentCourse.Discount,
                    DaysDiscount = currentCourse.DaysDiscount,
                    Prerequisites = currentCourse.Prerequisites,
                    YoullLearn = currentCourse.YoullLearn,
                    Price = currentCourse.Price,
                    PriceId = currentCourse.PriceId,
                    CategoryId = currentCourse.CategoryId,
                    Image = currentCourse.Image,
                    AllCategories = new SelectList(categories, "Id", "Name")
                };
                return View(courseViewModel);
            }
            return NotFound();
        }

        [Route("/panel/edit-course")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditCourse(CourseViewModel courseViewModel)
        {
            if (ModelState.IsValid)
            {
                var selectedCourse = await _courses.GetCourseByIdAsync(courseViewModel.Id ?? 0);
                if (selectedCourse == null)
                {
                    return NotFound();
                }
                var currentCourse = new Course
                {
                    Id = courseViewModel.Id ?? 0,
                    Name = courseViewModel.Name!,
                    AboutCourse = courseViewModel.AboutCourse!,
                    DifficultyLevel = courseViewModel.DifficultyLevel,
                    Language = courseViewModel.Language!,
                    VideoUrl = courseViewModel.VideoUrl!,
                    Hourpassed = courseViewModel.Hourpassed!,
                    Discount = courseViewModel.Discount,
                    DaysDiscount = courseViewModel.DaysDiscount,
                    Prerequisites = courseViewModel.Prerequisites!,
                    YoullLearn = courseViewModel.YoullLearn!,
                    Price = courseViewModel.Price,
                    PriceId = courseViewModel.PriceId,
                    CategoryId = courseViewModel.CategoryId,
                    Image = courseViewModel.Image,
                };
                if (courseViewModel.File != null)
                {
                    if (selectedCourse.Image != null)
                    {
                        if (System.IO.File.Exists(_appEnvironment.WebRootPath + selectedCourse.Image))
                        {
                            System.IO.File.Delete(_appEnvironment.WebRootPath + selectedCourse.Image);
                        }
                    }
                    string fileName = courseViewModel.File.FileName;
                    string filePath = FileService.СreateFilePathFromFileName(fileName, foldername: "courseImages");
                    currentCourse.Image = filePath;
                    await FileService.SaveFile(filePath, courseViewModel.File, _appEnvironment);
                }
                await _courses.EditCourseAsync(currentCourse);
                return RedirectToAction(nameof(Courses));
            }
            return View(courseViewModel);
        }

    }
}
