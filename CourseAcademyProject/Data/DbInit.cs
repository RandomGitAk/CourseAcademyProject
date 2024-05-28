using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Test;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Bcpg.Sig;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Reflection.Metadata;
using System.Security.AccessControl;

namespace CourseAcademyProject.Data
{
    public class DbInit
    {
        public static async Task CreateProcedureAllCountForAdminAsync(ApplicationContext context)
        {
            context.Database.ExecuteSqlRaw("drop procedure if exists dbo.CountForAdmin");
            context.Database.ExecuteSqlRaw(@"
			CREATE PROCEDURE dbo.CountForAdmin
            AS
            BEGIN
                 SELECT 
                (SELECT COUNT(*) FROM Categories) AS CountCategories,
                (SELECT COUNT(*) FROM Courses) AS CountCourses,
                (SELECT COUNT(*) FROM AspNetUsers) AS CountUsers,
                (SELECT COUNT(DISTINCT UserId) FROM UserCourses) AS CountUsersOnCourses,
                (SELECT SUM(PayPrice) FROM UserCourses) AS TotalIncome,
                (SELECT SUM(PayPrice) FROM UserCourses 
                    WHERE MONTH(CreatedDate) = MONTH(GETDATE()) 
                    AND YEAR(CreatedDate) = YEAR(GETDATE())) AS IncomeByCurrentMonth
            END;
            ");
        }
        public static async Task InitializeAsync(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (await roleManager.FindByNameAsync("Client") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Client"));
            }

            string adminEmail = "admin@gmail.com", password = "qwerty";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                User admin = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    PhoneNumber = "0998564789",
                    FirstName = "Sam",
                    LastName = "Greenwood"
                };
                IdentityResult res = await userManager.CreateAsync(admin, password);
                if (res.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }

                User tom = new User
                {
                    Email = "tom@gmail.com",
                    UserName = "tom@gmail.com",
                    PhoneNumber = "0665459874",
                    FirstName = "Matt",
                    LastName = "Cerriker"
                };
                res = await userManager.CreateAsync(tom, password);
                if (res.Succeeded)
                {
                    await userManager.AddToRoleAsync(tom, "Client");
                }

            }
        }

        public static async Task InitializeContentAsync(ApplicationContext context)
        {
            if (!await context.Categories.AnyAsync())
            {
                await context.Categories.AddRangeAsync
                    (
                        new Category[]{
                            new Category{ Name = "C#", Description = "C# courses", Image = "/categoryImages/c.png" },
                            new Category{ Name = "Web dev", Description = "HTML, CSS and other web",Image = "/categoryImages/html.png" },
                            new Category{ Name = "JS", Description = "JS courses",Image = "/categoryImages/js.png" }
                        }
                    );
                await context.SaveChangesAsync();
            }

            if (!await context.Courses.AnyAsync())
            {
                await context.Courses.AddRangeAsync
                    (
                        new Course
                        {
                            Name = "C# Programming for Beginners",
                            AboutCourse = "This course covers the basics of C# programming language.",
                            Prerequisites = "No prior programming experience required.",
                            YoullLearn = "Variables, control flow, basic data structures, and more.",
                            DateOfPublication = DateTime.Now,
                            DifficultyLevel = DifficultyLevel.Beginner,
                            VideoUrl = "https://www.youtube.com/watch?v=Z5JS36NlJiU",
                            Language = "English",
                            Category = context.Categories.FirstOrDefault(e=> e.Name.Equals("C#")),
                            Hourpassed = "9:45",
                            Image = "/courseImages/cNews.png" 
                        },
                        new Course
                        {
                            Name = "Advanced C# Programming",
                            AboutCourse = "This course covers advanced topics in C# programming.",
                            Prerequisites = "Familiarity with basic C# programming concepts.",
                            YoullLearn = "Advanced topics like LINQ, asynchronous programming, etc.",
                            DateOfPublication = DateTime.Now,
                            DifficultyLevel = DifficultyLevel.Advanced,
                            VideoUrl = "https://www.youtube.com/watch?v=3cfVmcAkR2w",
                            Language = "English",
                            Category = context.Categories.FirstOrDefault(e => e.Name.Equals("C#")),
                            Hourpassed = "9:45",
                            Image = "/courseImages/CAdvanced.png",
                            Price = 15
                        },
                        new Course
                        {
                            Name = "Web Development Basics",
                            AboutCourse = "Introduction to web development technologies.",
                            Prerequisites = "Basic understanding of HTML and CSS.",
                            YoullLearn = "HTML, CSS, and basic JavaScript.",
                            DateOfPublication = DateTime.Now,
                            DifficultyLevel = DifficultyLevel.Beginner,
                            VideoUrl = "https://www.youtube.com/watch?v=HGTJBPNC-Gw",
                            Language = "English",
                            Category = context.Categories.FirstOrDefault(e => e.Name.Equals("Web dev")),
                            Hourpassed = "6:00",
                            Image = "/courseImages/htmlcssBeg.png",
                        },
                        new Course
                        {
                            Name = "Full Stack Web Development",
                            AboutCourse = "Comprehensive course on full-stack web development.",
                            Prerequisites = "Basic understanding of web development concepts.",
                            YoullLearn = "Frontend and backend development using popular frameworks.",
                            DateOfPublication = DateTime.Now,
                            DifficultyLevel = DifficultyLevel.Middle,
                            VideoUrl = "https://www.youtube.com/watch?v=iG2jotQo9NI",
                            Language = "English",
                            Category = context.Categories.FirstOrDefault(e => e.Name.Equals("Web dev")),
                            Hourpassed = "11:00",
                            Image = "/courseImages/csshtmlMd.png",
                            Price = 20,
                            PriceId = "price_1OxrMG02pbliTYxGHlFhYTGu",
                            Discount = 8,
                            DaysDiscount = 15
                        },
                        new Course
                        {
                            Name = "JavaScript Fundamentals",
                            AboutCourse = "Learn the fundamentals of JavaScript programming language.",
                            Prerequisites = "No prior JavaScript knowledge required.",
                            YoullLearn = "JavaScript syntax, data types, functions, and more.",
                            DateOfPublication = DateTime.Now,
                            DifficultyLevel = DifficultyLevel.Beginner,
                            VideoUrl = "https://www.youtube.com/watch?v=PkZNo7MFNFg",
                            Category = context.Categories.FirstOrDefault(e => e.Name.Equals("JS")),
                            Language = "English",
                            Hourpassed = "8:00",
                            Image = "/courseImages/jsBeg.jpg"
                        }
                    );
                await context.SaveChangesAsync();
            }

            if (!await context.Lessons.AnyAsync())
            {
                await context.Lessons.AddAsync
                    (
                        new Lesson
                        {
                            Title = "Lesson 1: Program Structure. Variables. Input-Output. Transformations",
                            Information = "There are more than 300 thousand different words in the English language. So how can the C# language cope with having " +
                        "only a hundred keywords? Furthermore, why is C# so difficult to learn if it only contains 0.0416% of the words compared to the number" +
                        " in English? One of the key differences between human language and programming language is that developers must be able to define " +
                        "new \"words\" with new meanings. In addition to the 104 keywords in C#, in the lessons I'll teach you some of the hundreds of thousands " +
                        "of \"words\" defined by other developers, but you'll also learn how to define your own. ",
                            LessonContent = "The fact that today software products are integrated into all spheres of human activity is a recognized fact. Technologies " +
                        "of production process automation are being implemented not only at industrial facilities, but, for example, in farming.\r\n\r\nDue to such a" +
                        " rapid growth of demand for software in various spheres of life, the very technology of creating this software is developing. Which consists, " +
                        "on the one hand, in the improvement of programming languages allowing to realize more and more complex and large-scale projects, and on the other hand" +
                        " - in the development of technologies that are either based on these languages or allow to realize the possibility of creating and using complex " +
                        "high-level programming languages. \r\n\r\nWe will consider the development of programming languages on the example of the \"C\" language family.",
                            VideoUrl = "https://www.youtube.com/watch?time_continue=1&v=KyFWqbRfWIA&source_ve_path=Mjg2NjY&feature=emb_logo",
                            Course = context.Courses.FirstOrDefault(e => e.Name.Equals("C# Programming for Beginners")),
                            MaterialsUrl = "https://visualstudio.microsoft.com/ru/msdn-platforms/",
                            FileLessonContent = "/lessonMaterials/bffec2e8-63b9-40d2-be73-c25e4f5c4126first.docx"
                        }
                    );
                await context.SaveChangesAsync();
            }

            if (!await context.Tests.AnyAsync())
            {
                var test1 = new Test
                {
                    Title = "Test 1",
                    Description = "Description for Test 1",
                    Course = context.Courses.FirstOrDefault(e => e.Name.Equals("C# Programming for Beginners")),
                    TimeToPass = new TimeOnly(0,15),
                    Questions = new List<Question>
                    {
                         new Question
                            {
                                Content = "Question 1 for Test 1",
                                Answers = new List<Answer>
                                {
                                    new Answer { Content = "Answer 1 for Question 1 correct", IsCorrect = true },
                                    new Answer { Content = "Answer 2 for Question 1", IsCorrect = false },
                                    new Answer { Content = "Answer 3 for Question 1", IsCorrect = false },
                                    new Answer { Content = "Answer 4 for Question 1", IsCorrect = false }
                                }
                            },
                          new Question
                            {
                                Content = "Question 2 for Test 1",
                                Answers = new List<Answer>
                                {
                                    new Answer { Content = "Answer 1 for Question 2", IsCorrect = false },
                                    new Answer { Content = "Answer 2 for Question 2 correct", IsCorrect = true },
                                    new Answer { Content = "Answer 3 for Question 2", IsCorrect = false },
                                    new Answer { Content = "Answer 4 for Question 1", IsCorrect = false }
                                }
                            }
                    }
                };
                var test2 = new Test
                {
                    Title = "Test 2",
                    Description = "Description for Test 2",
                    Course = context.Courses.FirstOrDefault(e => e.Name.Equals("C# Programming for Beginners")),
                    TimeToPass = new TimeOnly(0, 18),
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            Content = "Question 1 for Test 2",
                            Answers = new List<Answer>
                            {
                                new Answer { Content = "Answer 1 for Question 1 correct", IsCorrect = true },
                                new Answer { Content = "Answer 2 for Question 1", IsCorrect = false },
                                new Answer { Content = "Answer 3 for Question 1", IsCorrect = false },
                                new Answer { Content = "Answer 4 for Question 1", IsCorrect = false }
                            }
                        },
                        new Question
                        {
                            Content = "Question 2 for Test 2",
                            Answers = new List<Answer>
                            {
                                new Answer { Content = "Answer 1 for Question 2", IsCorrect = false },
                                new Answer { Content = "Answer 2 for Question 2", IsCorrect = false },
                                new Answer { Content = "Answer 3 for Question 2 correct", IsCorrect = true },
                                new Answer { Content = "Answer 4 for Question 1", IsCorrect = false }
                            }
                        }
                    }
                };

                await context.Tests.AddRangeAsync(test1, test2);
                await context.SaveChangesAsync();
            }
            if (!await context.Blogs.AnyAsync()) { }
            {
                var blog1 = new Blog
                {
                    Name = "New basic C# video course",
                    MiniDescription = "Welcome to our new basic C# video course! " +
                    "This course is designed for beginners who want to learn the fundamentals of C# programming. " +
                    "Through a series of engaging and easy-to-follow video tutorials, you'll gain a solid understanding " +
                    "of C# syntax, data types, control structures, and more. ",
                    Content = "Welcome to our new basic C# video course! In this course, you'll embark on a journey to master the fundamentals of C# programming." +
                    " Each lesson is designed with beginners in mind, ensuring that complex concepts are broken down into easy-to-understand segments. " +
                    "By the end of the course, you'll have a strong grasp of C# syntax, data types, and control structures." +
                    "Our video tutorials are engaging and interactive, allowing you to learn at your own pace. " +
                    "You'll find practical examples and hands-on exercises to reinforce your understanding. " +
                    "Whether you're starting from scratch or enhancing your programming skills, " +
                    "this course will equip you with the knowledge to build your own C# applications confidently. " +
                    "Join us and start your programming journey today!",
                    Image = "/blogImages/3e16f420-1d35-4ebc-bb7b-3ecb3efee3e2cNews.png"
                };
                var blog2 = new Blog
                {
                    Name = "Web Development Basics",
                    MiniDescription = "Welcome to our Web Development Basics course! This course is designed for beginners who want to learn the " +
                    "fundamentals of web development. You will explore essential technologies like HTML, CSS, and JavaScript, and gain the skills needed to" +
                    " build your own websites from scratch.",
                    Content = "In this Web Development Basics course, you will start by learning the core languages of the web: HTML, CSS, and JavaScript. " +
                    "HTML is used to structure your web content, CSS to style it, and JavaScript to make it interactive. By mastering these technologies," +
                    " you will be able to create visually appealing and functional websites.As you progress through the course, you will work on" +
                    " various projects to reinforce your learning. You will build simple web pages, style them with CSS, and add interactivity with JavaScript. " +
                    "By the end of the course, you will have a strong foundation in web development, enabling you to create your own web projects and " +
                    "further explore advanced topics.",
                    Image = "/blogImages/c9c18ec8-e9b9-4082-ad17-e59818c2b1dcunityNews.jpg"
                };
                var blog3 = new Blog
                {
                    Name = "Create telegram bot on C#",
                    MiniDescription = "In this course, Create a Telegram Bot with C#, you will learn how to develop a functional and interactive bot" +
                    " using the Telegram Bot API and C#. We will guide you through setting up your development environment, creating your bot, and implementing" +
                    " various features. By the end of the course, you will have the skills to build and deploy your own Telegram bot from scratch.",
                    Content = "In the first part of this course, you will learn the basics of setting up your development environment and creating a simple" +
                    " Telegram bot using C#. We will start by registering your bot with Telegram and obtaining the necessary API token. Then, we will cover " +
                    "the fundamental concepts of interacting with the Telegram Bot API, including sending and receiving messages.The second part of the course " +
                    "will focus on enhancing your bot with more advanced features.You will learn how to handle different types of user inputs," +
                    " integrate with external APIs, and manage bot commands.By the end of the course, you will be able to deploy your bot to a live server," +
                    " making it accessible to users worldwide.",
                    Image = "/blogImages/ea58a698-524f-4ac6-9fee-d2fb095fd772languagesNews.png"
                };
                await context.Blogs.AddRangeAsync(blog1, blog2, blog3);
                await context.SaveChangesAsync();
            }
        }

    }
}
