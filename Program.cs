using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using STAWeb;

var builder = WebApplication.CreateBuilder(args);
var sessionTimeout = builder.Configuration.GetValue<int>(
    "Appsettings:SessionSettings:SessionTimeoutMinutes");

var authTimeout = builder.Configuration.GetValue<int>(
    "Appsettings:SessionSettings:AuthCookieTimeoutMinutes");

// ---------------- MVC ----------------

// 1. Add services to the container
builder.Services.AddDistributedMemoryCache(); // Required for Session
builder.Services.AddSession(options =>
{
    //options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;                // Security: prevent JS access
    options.Cookie.IsEssential = true;             // Required for GDPR/compliance
    //options.Cookie.Name = ".STA.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(sessionTimeout);
    options.Cookie.Name = "CUMS.Session";

});
builder.Services.AddControllersWithViews()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<UserManagementModelValidator>();
    });

builder.Services.AddHttpContextAccessor();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login";
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // localhost
        options.AccessDeniedPath = "/Login/Login";
        //options.AccessDeniedPath = "/Login/Denied";
        //options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(authTimeout);
        options.Cookie.Name = "CUMS.Auth";
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<UserProvider>();

var app = builder.Build();

// ---------------- PIPELINE ----------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.UseMiddleware<ApiTokenRefreshMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();



//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using STARegistration.Middleware;
//using FluentValidation;
//using FluentValidation.AspNetCore;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using STAWeb;

//public class Program
//{
//	public static void Main(string[] args)
//	{
//		CreateHostBuilder(args).Build().Run();
//	}

//	public static IHostBuilder CreateHostBuilder(string[] args) =>
//		Host.CreateDefaultBuilder(args)
//			.ConfigureWebHostDefaults(webBuilder =>
//			{
//				webBuilder.ConfigureServices(services =>
//				{
//                    services.AddControllersWithViews()
//                            .AddFluentValidation(fv =>
//                            {
//                                fv.RegisterValidatorsFromAssemblyContaining<UserManagementModelValidator>();
//                            });
//                    services.AddHttpContextAccessor();
//                    // Add session services
//                    services.AddSession(options =>
//                    {
//                        options.IdleTimeout = TimeSpan.FromMinutes(20); // ⏱️ Set timeout here
//                        options.Cookie.HttpOnly = true;
//                        options.Cookie.IsEssential = true;
//                    });
//                })
//				.Configure(app =>
//				{
//					app.UseHttpsRedirection();
//					app.UseStaticFiles();
//					app.UseRouting();
//					app.UseAuthorization();
//					app.UseSession(); // Use session middleware
//                    app.UseMiddleware<SessionCheckMiddleware>();
//                    app.UseEndpoints(endpoints =>
//					{
//						endpoints.MapControllerRoute(
//							name: "default",
//							pattern: "{controller=Login}/{action=Login}/{id?}");
//					});
//				});
//			});
//}

