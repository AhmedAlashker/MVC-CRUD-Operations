using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Context;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.MappingProfiles;
using Demo.PL.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL
{
    public class Program
    {
        // Entry Point
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            var Builder = WebApplication.CreateBuilder(args);
            #region Configure Services That Allow Dependancy Injection

            Builder.Services.AddControllersWithViews();

            ///
            Builder.Services.AddDbContext<MvcAppDbContext>(options =>
            {
                options.UseSqlServer(Builder.Configuration.GetConnectionString("DefaultConnection"));
            }/*, ServiceLifetime.Scoped*/); // Allow Dependancy Injection

            //services.AddScoped<IDepartmentRepository, DepartmentRepository>(); // Allow Dependancy Injection for Class ' DepartmentRepository '
            //services.AddScoped<IEmployeeRepository, EmployeeRepository>(); // Allow Dependancy Injection for Class ' EmployeeRepository '

            //services.AddAutoMapper(M => M.AddProfile(new EmployeeProfile()));
            //services.AddAutoMapper(M => M.AddProfile(new UserProfile()));
            Builder.Services.AddAutoMapper(M => M.AddProfiles(new List<Profile>() { new EmployeeProfile(), new UserProfile(), new RoleProfile() }));

            Builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            //services.AddScoped<UserManager<ApplicationUser>>();

            Builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Options =>
            {
                Options.Password.RequireNonAlphanumeric = true; // @ #
                Options.Password.RequireDigit = true; // 1254
                Options.Password.RequireLowercase = true; // aa
                Options.Password.RequireUppercase = true; // AA 
            })
                .AddEntityFrameworkStores<MvcAppDbContext>()
                .AddDefaultTokenProviders();

            Builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(Options =>
                {
                    Options.LoginPath = "Account/Login";
                    Options.AccessDeniedPath = "Home/Error";
                });

            //// Mailkit
            Builder.Services.Configure<MailSettings>(Builder.Configuration.GetSection("MailSettings"));
            Builder.Services.AddTransient<IMailSettings, EmailSettings>();

            //// Twilio
            Builder.Services.Configure<TwilioSettings>(Builder.Configuration.GetSection("Twilio"));
            Builder.Services.AddTransient<ISMSService, SMSService>();

            //// External Login with Google
            Builder.Services.AddAuthentication(O =>
            {
                O.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                O.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }).AddGoogle(O =>
            {
                IConfiguration GoogleAuthSection = Builder.Configuration.GetSection("Authentication:Google");
                O.ClientId = GoogleAuthSection["ClientId"];
                O.ClientSecret = GoogleAuthSection["ClientSecret"];

            });


			#endregion

			var app = Builder.Build(); // This IS Cestrel
            #region Configure Http Request Pipelines
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
            #endregion

            app.Run();

        }

        /// Method That Create Cestrel
        ///public static IHostBuilder CreateHostBuilder(string[] args) =>
        ///    Host.CreateDefaultBuilder(args)
        ///        .ConfigureWebHostDefaults(webBuilder =>
        ///        {
        ///            webBuilder.UseStartup<Startup>();
        ///        });
        ///        


    }
}
