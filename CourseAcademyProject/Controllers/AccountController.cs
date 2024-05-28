using CourseAcademyProject.Data.Helpers;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseAcademyProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailSender _emailSender;
        private readonly IWebHostEnvironment _appEnvironment;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, EmailSender emailSender, IWebHostEnvironment appEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _appEnvironment = appEnvironment;
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Index", "Panel");
                    }
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login and/or password");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AcceptVerbs("Get","Post")]
        [AllowAnonymous]
        public IActionResult IsEmailInUse(string email)
        {
            if (_userManager.Users.Any(e=> e.Email!.Equals(email)))
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public IActionResult IsEmailExists(string email)
        {
            if (_userManager.Users.Any(e => e.Email!.Equals(email)))
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        [Route("registration")]
        [HttpGet]
        public IActionResult Registration()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [Route("registration")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Registration(RegViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, FirstName = model.FirstName!, LastName = model.LastName! };
                var result = await _userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, "Client");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }


        //Change Password

        [Route("forgot-password")]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [Route("forgot-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(RequestForResetViewModel requestForResetViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(requestForResetViewModel.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "There is no user with that e-mail address.");
                    return View(requestForResetViewModel);
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);
                if (_emailSender.SendResetPassword(user.Email,link,user.FirstName))
                {
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred, please try again later.");
                }
            }
            return View(requestForResetViewModel);
        }

        [Route("forgot-password-confirmation")]
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [Route("reset-password")]
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (String.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction(nameof(Login));
            }
            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        [Route("reset-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPassword.Email);
                if (user == null)
                {
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }
                var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (!resetPassResult.Succeeded)
                {
                    foreach (var error in resetPassResult.Errors)
                    {
                        if (error.Description.Equals("Invalid token."))
                        {
                            ModelState.AddModelError("", "Your token is either missing or obsolete. Request recovery again.");
                        }
                        else
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                    return View(resetPassword);
                }
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            return View(resetPassword);
        }

        [Route("reset-password-confirmation")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [Route("personal-cabinet")]
        [HttpGet]
        public async Task<IActionResult> PersonalCabinet([FromServices] ICourse course, [FromServices] ICategory category, int categoryId, string courseName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return NotFound();
                }
                return View(new PersonalCabinteViewModel
                {
                    Courses = await course.GetCoursesWithTestsAndPastedTestsByUserIdAndCategIdAndNameAsync(currentUser.Id,categoryId),
                    AllCategories = await category.GetAllCategoriesAsync(),
                    CategoryId = categoryId
            });
            }
            return Unauthorized();
        }

        [Route("find-course")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> FindCourse([FromServices] ICourse course, [FromServices] ICategory category, int categoryId, string courseName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return NotFound();
                }
                return View("PersonalCabinet",new PersonalCabinteViewModel
                {
                    Courses = await course.GetCoursesWithTestsAndPastedTestsByUserIdAndCategIdAndNameAsync(currentUser.Id, categoryId,courseName),
                    AllCategories = await category.GetAllCategoriesAsync(),
                    CategoryId = categoryId,
                    CourseName = courseName
                });
            }
            return Unauthorized();
        }

        [Route("profile-settings")]
        [HttpGet]
        public async Task<IActionResult> AccountSettings()
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) { 
                    return NotFound();
                }
                return View(new AccountSettingsViewModel
                {
                    Id = currentUser.Id,
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Email = currentUser.Email,
                    Image = currentUser.Image
                });
            }
           return Unauthorized();
        }

        [Route("profile-settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccountSettings(AccountSettingsViewModel model)
        {
            var currentUser = await _userManager.FindByIdAsync(model.Id);
            if (currentUser != null)
            {
                currentUser.FirstName = model.FirstName;
                currentUser.LastName = model.LastName;
                if (model.File != null)
                {
                    if (currentUser.Image != null && currentUser.Image != "/userImages/defaultUser.jpg")
                    {
                        if (System.IO.File.Exists(_appEnvironment.WebRootPath + currentUser.Image))
                        {
                            System.IO.File.Delete(_appEnvironment.WebRootPath + currentUser.Image);
                        }
                    }
                    string fileName = model.File.FileName;
                    string filePath = FileService.СreateFilePathFromFileName(fileName, foldername: "userImages");
                    currentUser.Image = filePath;
                    await FileService.SaveFile(filePath, model.File, _appEnvironment);
                }

                var result = await _userManager.UpdateAsync(currentUser);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(AccountSettings));
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while saving");
                }
                return View(model);
            }
            return NotFound();
        }

        [Route("password-settings")]
        [HttpGet]
        public async Task<IActionResult> AccountPasswordSettings()
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return NotFound();
                }
                return View(new ResetPasswordViewModel { Token = "true", Email = currentUser.Email});
            }
            return Unauthorized();
           
        }
    }
}
