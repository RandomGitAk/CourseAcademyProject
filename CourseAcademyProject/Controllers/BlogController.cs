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
    public class BlogController : Controller
    {
        private readonly IBlog _blogs;
        private readonly IWebHostEnvironment _appEnvironment;

        public BlogController(IBlog blogs, IWebHostEnvironment appEnvironment)
        {
            _blogs = blogs;
            _appEnvironment = appEnvironment;
        }

        [Route("/panel/blogs")]
        [HttpGet]
        public IActionResult Blogs(QueryOptions options)
        {
            return View(_blogs.GetAllBlogs(options));
        }

        [Route("/panel/create-blog")]
        [HttpGet]
        public IActionResult CreateBlog()
        {
            return View();
        }

        [Route("/panel/create-blog")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateBlog(BlogViewModel blogViewModel)
        {
            if (ModelState.IsValid)
            {
                var currentBlog = new Blog
                {
                    Name = blogViewModel.Name,
                    MiniDescription = blogViewModel.MiniDescription,
                    Content = blogViewModel.Content
                };
                if (blogViewModel.File != null)
                {
                    string fileName = blogViewModel.File.FileName;
                    string filePath = FileService.СreateFilePathFromFileName(fileName, foldername: "blogImages");
                    currentBlog.Image = filePath;
                    await FileService.SaveFile(filePath, blogViewModel.File, _appEnvironment);
                }
                await _blogs.AddBlogAsync(currentBlog);
                return RedirectToAction(nameof(Blogs));
            }
            return View(blogViewModel);
        }

        [Route("/panel/edit-blog")]
        [HttpGet]
        public async Task<IActionResult> EditBlog(int blogId)
        {
            var currentBlog = await _blogs.GetBlogByIdAsync(blogId);    
            if (currentBlog != null)
            {
                BlogViewModel blogViewModel = new BlogViewModel
                {
                    Id = currentBlog.Id,
                    Name = currentBlog.Name,
                    Image = currentBlog.Image,
                    MiniDescription = currentBlog.MiniDescription,
                    Content = currentBlog.Content
                };
                return View(blogViewModel);
            }
            return NotFound();
        }

        [Route("/panel/edit-blog")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditBlog(BlogViewModel blogViewModel)
        {
            if (ModelState.IsValid)
            {
                var selectedBlog = await _blogs.GetBlogByIdAsync(blogViewModel.Id ?? 0); 
                if (selectedBlog == null)
                {
                    return NotFound();
                }

                var currentBlog = new Blog
                {
                    Id = selectedBlog.Id,
                    Name = blogViewModel.Name!,
                    MiniDescription = blogViewModel.MiniDescription,
                    Content = blogViewModel.Content,
                    Image = selectedBlog.Image
                };

                if (blogViewModel.File != null)
                {
                    if (selectedBlog.Image != null)
                    {
                        if (System.IO.File.Exists(_appEnvironment.WebRootPath + selectedBlog.Image))
                        {
                            System.IO.File.Delete(_appEnvironment.WebRootPath + selectedBlog.Image);
                        }
                    }
                    string fileName = blogViewModel.File.FileName;
                    string filePath = FileService.СreateFilePathFromFileName(fileName, foldername: "blogImages");
                    currentBlog.Image = filePath;
                    await FileService.SaveFile(filePath, blogViewModel.File, _appEnvironment);
                }
                await _blogs.EditBlogAsync(currentBlog);
                return RedirectToAction(nameof(Blogs));
            }
            return View(blogViewModel);
        }

        [Route("/panel/delete-blog")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBlog(int blogId)
        {
            var currentBlog = await _blogs.GetBlogByIdAsync(blogId);
            if (currentBlog != null)
            {
                if (currentBlog.Image != null)
                {
                    if (System.IO.File.Exists(_appEnvironment.WebRootPath + currentBlog.Image))
                    {
                        System.IO.File.Delete(_appEnvironment.WebRootPath + currentBlog.Image);
                    }
                }
                await _blogs.DeleteBlogAsync(currentBlog);
            }
            return Ok();
        }
    }
}
