using CourseAcademyProject.Data.Helpers;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseAcademyProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategory _categories;
        private readonly IWebHostEnvironment _appEnvironment;

        public CategoryController(ICategory categories, IWebHostEnvironment appEnvironment)
        {
            _categories = categories;
            _appEnvironment = appEnvironment;
        }

        [Route("/panel/categories")]
        [HttpGet]
        public IActionResult Categories(QueryOptions options)
        {
            return View(_categories.GetAllCategories(options));
        }

        [Route("/panel/create-category")]
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [Route("/panel/create-category")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                var currentCategory = new Category
                {
                    Name = categoryViewModel.Name,
                    Description = categoryViewModel.Description
                };
                if (categoryViewModel.File != null)
                {
                    string fileName = categoryViewModel.File.FileName;
                    string filePath = FileService.СreateFilePathFromFileName(fileName, foldername: "categoryImages");
                    currentCategory.Image = filePath;
                    await FileService.SaveFile(filePath, categoryViewModel.File, _appEnvironment);
                }
                await _categories.AddCategoryAsync(currentCategory);
                return RedirectToAction(nameof(Categories));
            }
            return View(categoryViewModel);
        }

        [Route("/panel/edit-category")]
        [HttpGet]
        public async Task<IActionResult> EditCategory(int categoryId)
        {
            var currentCategory = await _categories.GetCategoryByIdAsync(categoryId); 
            if (currentCategory != null)
            {
                CategoryViewModel categoryViewModel = new CategoryViewModel 
                {
                    Id = currentCategory.Id,
                    Name = currentCategory.Name,
                    Image = currentCategory.Image,
                    Description = currentCategory.Description
                };
                return View(categoryViewModel);
            }
            return NotFound();
        }

        [Route("/panel/edit-category")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditCategory(CategoryViewModel categoryViewModel)
        {
            if(ModelState.IsValid)
            {
                var selectedCategory = await _categories.GetCategoryByIdAsync(categoryViewModel.Id ?? 0);
                if (selectedCategory == null)
                {
                    return NotFound();
                }

                var currentCategory = new Category
                {
                    Id = selectedCategory.Id,
                    Name = categoryViewModel.Name!,
                    Description = categoryViewModel.Description,
                    Image = selectedCategory.Image
                };

                if (categoryViewModel.File != null)
                {
                    if (selectedCategory.Image != null)
                    {
                        if (System.IO.File.Exists(_appEnvironment.WebRootPath + selectedCategory.Image))
                        {
                            System.IO.File.Delete(_appEnvironment.WebRootPath + selectedCategory.Image);
                        }
                    }
                    string fileName = categoryViewModel.File.FileName;
                    string filePath = FileService.СreateFilePathFromFileName(fileName, foldername: "categoryImages");
                    currentCategory.Image = filePath;
                    await FileService.SaveFile(filePath, categoryViewModel.File, _appEnvironment);
                }
                await _categories.EditCategoryAsync(currentCategory);
                return RedirectToAction(nameof(Categories));
            }
            return View(categoryViewModel);
        }

        [Route("/panel/delete-category")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var currentCategory = await _categories.GetCategoryByIdAsync(categoryId);
            if (currentCategory != null)
            {
                if (currentCategory.Image != null)
                {
                    if (System.IO.File.Exists(_appEnvironment.WebRootPath + currentCategory.Image))
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath + currentCategory.Image);
                    }
                }
                await _categories.DeleteCategoryAsync(currentCategory);
            }
            return Ok();
        }
    }
}

