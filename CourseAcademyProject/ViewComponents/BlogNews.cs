using CourseAcademyProject.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseAcademyProject.ViewComponents
{
    public class BlogNews : ViewComponent
    {
        private readonly IBlog _blog;
        public BlogNews(IBlog blog)
        {
            _blog = blog;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("BlogNews", await _blog.GetThreeLatestBlogsAsync());
        }
    }
}
