using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Resort.Application.Repository;
using Resort.Application.Services;
using Resort.Application.Services.Implementation;
using Resort.Application.Services.Interface;
using Resort.Domain.Models;
using Resort.Infrastructure.Data;
using Resort.Infrastructure.Emails;
using Resort.Infrastructure.Implementation;
using Resort.Web.ChatHub;
using Stripe;
using Syncfusion.Licensing;

namespace Resort.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // add signalR package
            builder.Services.AddSignalR();

            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            builder.Services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("_cn")
                ));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


           //  builder.Services.AddScoped<IVillaRepository, VillaRepository>();
          
           builder.Services.AddScoped<IVillaService, VillaService>();
           builder.Services.AddScoped<IVillaNumberService, VillaNumberService>();
           builder.Services.AddScoped<IAmenityService, AmenityService>();
           builder.Services.AddScoped<IBookingService, BookingService>();
           builder.Services.AddScoped<IDashboardService, DashboardService>();
           builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
           builder.Services.AddScoped<IDbInitializer,DbInitializer>();
           builder.Services.AddScoped<IEmailService, EmailService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            // add signalR Route Here 
            app.MapHub<ChatHubs>("/ChatPage");
            // stripe configuration
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
           
            // syncfusion package (pdf,word,ppt) configuration
            SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetSection("Syncfusion:Licensekey").Get<string>());
        
            app.UseAuthorization();

            SeedDataDefault();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();

            void SeedDataDefault()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    dbInitializer.Initialize();
                }
            }
        }


    }
}
