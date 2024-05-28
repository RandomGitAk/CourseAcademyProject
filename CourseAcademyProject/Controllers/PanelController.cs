using CourseAcademyProject.Data;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Pages;
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseAcademyProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PanelController : Controller
    {
        private readonly UserManager<User> _userManager;
        public PanelController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        [Route("/panel/users")]
        [HttpGet]
        public IActionResult Users(QueryOptions options)
        {
            return View(new PagedList<User>(_userManager.Users, options));
        }
        
        [Route("/panel/create-update-user")]
        [HttpGet]
        public async Task<IActionResult> CreateOrUpdateUser(string? userId)
        {
            if (userId is null)
            {
                return View(new CreateOrUpdateUserViewModel());
            }
            else
            {
                User user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                return View(new CreateOrUpdateUserViewModel{
                   Id = user.Id,
                   Email = user.Email,
                   FirstName = user.FirstName,
                   LastName = user.LastName
                });
            }
        }

        [Route("/panel/create-update-user")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateOrUpdateUser(CreateOrUpdateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id is null)
                {
                    User userCheck = await _userManager.FindByEmailAsync(model.Email);
                    if (userCheck != null)
                    {
                        ModelState.AddModelError("Email", "This e-mail address is already taken.");
                        return View(model);
                    }
                    User user = new User { Email = model.Email, UserName = model.Email, FirstName = model.FirstName, LastName = model.LastName};
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddToRoleAsync(user, "Client");
                        return RedirectToAction(nameof(Users));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    User user = await _userManager.FindByIdAsync(model.Id);
                    if (user != null)
                    {
                        if (!user.Email.Equals(model.Email))
                        {
                            User userCheck = await _userManager.FindByEmailAsync(model.Email);
                            if (userCheck != null)
                            {
                                ModelState.AddModelError("Email", "This e-mail address is already taken.");
                                return View(model);
                            }
                        }
                        user.Email = model.Email;
                        user.UserName = model.Email;
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            return RedirectToAction(nameof(Users));
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                }
            }
            return View(model);
        }
        [Route("/panel/delete-user")]
        [HttpDelete]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return Ok();
        }

        [Route("/panel/index")]
        [HttpGet]
        public IActionResult Index([FromServices] ApplicationContext context)
        {
            var results = context.Set<MainAdminPageViewModel>().FromSqlRaw("EXEC dbo.CountForAdmin")?.AsEnumerable()?.FirstOrDefault();
            return View(new MainAdminPageViewModel
            {
                CountCategories = results.CountCategories,
                CountCourses = results.CountCourses,
                CountUsers = results.CountUsers,
                CountUsersOnCourses = results.CountUsersOnCourses,
                IncomeByCurrentMonth = results.IncomeByCurrentMonth ?? 0,
                TotalIncome = results.TotalIncome ?? 0
            });
        }
    }
}
