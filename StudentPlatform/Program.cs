using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentPlatform.Data;

namespace StudentPlatform
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. إضافة الـ DbContext وقاعدة البيانات (SQLite)
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("Data Source=studentplatform.db"));

            // 2. إضافة الـ Identity ودعم الـ Roles
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // 3. ضبط مسار صفحة تسجيل الدخول الخاص بنا
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // 4. كود إنشاء الـ Roles وحساب المدرس أوتوماتيكياً عند تشغيل التطبيق
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync("Teacher"))
                    await roleManager.CreateAsync(new IdentityRole("Teacher"));

                if (!await roleManager.RoleExistsAsync("Student"))
                    await roleManager.CreateAsync(new IdentityRole("Student"));

                string teacherEmail = "teacher@platform.com";
                var teacherCheck = await userManager.FindByEmailAsync(teacherEmail);
                if (teacherCheck == null)
                {
                    var newTeacher = new IdentityUser { UserName = teacherEmail, Email = teacherEmail, EmailConfirmed = true };
                    var result = await userManager.CreateAsync(newTeacher, "Teacher@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newTeacher, "Teacher");
                    }
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

         

            app.Run();
        }
    }
}