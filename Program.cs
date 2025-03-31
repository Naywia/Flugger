using Flugger.Components;
using Flugger.Data;
using Microsoft.AspNetCore.Identity;

namespace Flugger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddIdentityCore<ApplicationUser>()
                .AddUserStore<CustomUserStore>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication()
            .AddIdentityCookies();
            builder.Services.AddAuthorization();

            builder.Services.AddHttpContextAccessor();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
