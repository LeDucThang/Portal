using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Auth.Models;
using Auth.Helpers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddControllersWithViews();

            services.AddDbContext<DataContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DataContext")));

            StaticParams.SecretKey = Configuration["Config:SecretKey"];
            StaticParams.ExpiredTime = long.TryParse(Configuration["Config:ExpiredTime"], out long time) ? time : 0;
            StaticParams.AdminPassword = Configuration["Config:AdminPassword"];
            StaticParams.GoogleClientId = Configuration["Google:ClientId"];
            StaticParams.GoogleClientSecret = Configuration["Google:ClientSecret"];
            StaticParams.GoogleRedirectUri = Configuration["Google:RedirectUri"];

            var key = Encoding.ASCII.GetBytes(StaticParams.SecretKey);

            
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
            }) 
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = "998310595831-dceeoaikv8ce1qls0v35h1fbd3uskiel.apps.googleusercontent.com";
                googleOptions.ClientSecret = "7Aur4sJBSIOXWv4gJdlDsu_B";
            });
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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
            // Session phải đặt bên trên app.UseEndpoints
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


            //new Setup(Configuration);
        }
    }
}
