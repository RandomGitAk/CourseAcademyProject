using CourseAcademyProject.Data;
using CourseAcademyProject.Data.Helpers;
using CourseAcademyProject.Interfaces;
using CourseAcademyProject.Models;
using CourseAcademyProject.Models.Email;
using CourseAcademyProject.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

IConfigurationRoot _confString = new ConfigurationBuilder().
   SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();

builder.Services.AddDbContext<ApplicationContext>(options =>
options.UseSqlServer(_confString.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
}).AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

//Email 
var emailConfig = builder.Configuration.GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig!);

//mail delivery service
builder.Services.AddSingleton<EmailSender>();

//Time of token existence, for password recovery - 1 hour
builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(1));

builder.Services.AddTransient<ICategory, CategoryRepository>();
builder.Services.AddTransient<ICourse, CourseRepository>();
builder.Services.AddTransient<ILesson, LessonRepository>();
builder.Services.AddTransient<IComment, CommentRepository>();
builder.Services.AddTransient<ITest, TestRepository>(); 
builder.Services.AddTransient<IUserAnswer, UserAnswerRepository>();
builder.Services.AddTransient<IUserCourse, UserCourseRepository>();
builder.Services.AddTransient<IBlog, BlogRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await DbInit.InitializeAsync(userManager, rolesManager);

        ApplicationContext applicationContext = services.GetRequiredService<ApplicationContext>();
        await DbInit.InitializeContentAsync(applicationContext);
        await DbInit.CreateProcedureAllCountForAdminAsync(applicationContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Use(async (context, next) =>
{
    await next.Invoke();

    if (context.Response.StatusCode == 400)
        context.Response.Redirect("/Index");
    if (context.Response.StatusCode == 401)
        context.Response.Redirect("/Login");

    if (context.Response.StatusCode == 403)
        context.Response.Redirect("/forbitten");
    if (context.Response.StatusCode == 404)
        context.Response.Redirect("/notFound");
});

app.Run();
