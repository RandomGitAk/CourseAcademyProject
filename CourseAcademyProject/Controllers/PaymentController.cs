using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Stripe.Checkout;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CourseAcademyProject.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IUserCourse _userCourses;
        private readonly ICourse _courses;

        private readonly UserManager<User> _userManager;
        
        public PaymentController(IUserCourse userCourses, UserManager<User> userManager, ICourse courses)
        {
            _userCourses = userCourses;
            _userManager = userManager;
            _courses = courses;
        }

        [HttpPost]
        public async Task<ActionResult> CreateCheckout(string priceId, int courseId)
        {
            User currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }
            Course currentCourse = await _courses.GetCourseByIdAsync(courseId);
            if (currentCourse == null)
            {
                return NotFound();
            }
            if (await _userCourses.IsUserInCourseAsync(currentUser.Id, courseId))
            {
                return RedirectToAction("CourseDetail", "Home", new { courseId = courseId });
            }
            string token = CreateJWTToken(courseId, currentUser.Id, currentCourse?.Price - (currentCourse.Discount ?? 0));
            var domain = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    Price = priceId,
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                SuccessUrl = domain + $"/payment-success?token={token}",
                CancelUrl = domain + $"/payment-cancel?courseId={courseId}"
            };
            var service = new SessionService();
            Session session = service.Create(options);
            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }

        [Route("payment-success")]
        [HttpGet]
        public async Task<IActionResult> Success(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = CreateTokenValidationParameters(token);
            ClaimsPrincipal claimsPrincipal;
            try
            {
                claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Invalid token");
            }

            var userIdClaim = claimsPrincipal.FindFirst("userId");
            var courseIdClaim = claimsPrincipal.FindFirst("courseId");
            var coursePriceClaim = claimsPrincipal.FindFirst("courseId");
            if (courseIdClaim == null || !int.TryParse(courseIdClaim.Value, out var courseId)
                || coursePriceClaim == null || !decimal.TryParse(coursePriceClaim.Value, out var coursePrice)
                || userIdClaim == null)
            {
                return BadRequest("Invalid token data");
            }
          
            await _userCourses.AddUserToCourseAsync(new UserCourse
            {
                CourseId = courseId,
                PayPrice = coursePrice,
                UserId = userIdClaim.Value
            });

            return View(courseId);
        }

 
        [Route("payment-cancel")]
        [HttpGet]
        public async Task<IActionResult> Cancel(int courseId)
        {
            return View(courseId);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Route("sign-up-to-course")]
        public async Task<IActionResult> SignUpUserToFreeCourse(int courseId)
        {
            User currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }
            if (!await _userCourses.IsUserInCourseAsync(currentUser.Id,courseId))
            {
                var currentCourse = await _courses.GetCourseByIdAsync(courseId);
                if (currentCourse?.Price == null && currentCourse?.PriceId == null) 
                {
                    await _userCourses.AddUserToCourseAsync(new UserCourse
                    {
                        CourseId = courseId,
                        UserId = currentUser.Id,
                    });
                }
            }
            return RedirectToAction("CourseDetail","Home", new {courseId = courseId});    
        }


        private string CreateJWTToken(int courseId, string userId, decimal? coursePrice)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("mysupersecret_secretkey!123");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("courseId", courseId.ToString()),
                    new Claim("coursePrice", coursePrice.ToString()),
                    new Claim("userId", userId.ToString()) 
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private TokenValidationParameters CreateTokenValidationParameters(string token)
        {

            var key = Encoding.ASCII.GetBytes("mysupersecret_secretkey!123");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            return tokenValidationParameters;
        }
    }
}
