using Microsoft.AspNetCore.Authentication.Cookies;
using CMCS.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace CMCS;
public class Program
{
    public static System.Threading.Tasks.Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

       
        builder.Services.AddSingleton<IClaimRepository, InMemoryClaimRepository>();

      
        builder.Services.AddSingleton<IUserService, InMemoryUserService>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}");

       
        return app.RunAsync();
    }
}
