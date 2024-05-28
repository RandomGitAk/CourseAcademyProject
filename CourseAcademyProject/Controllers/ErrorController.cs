using Microsoft.AspNetCore.Mvc;

namespace CourseAcademyProject.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/notFound")]
        public IActionResult NotFound()
        {
            return View();
        }

        [Route("/forbitten")]
        public IActionResult Forbitten()
        {
            return View();
        }
    }
}
