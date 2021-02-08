using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Majorsilence.Vpn.Site
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var mySqlInstance = System.Configuration.ConfigurationManager.AppSettings["MySqlInstance"].ToString();
            var mySqlDatabase = System.Configuration.ConfigurationManager.AppSettings["MySqlDatabase"].ToString();
            var email = new LibLogic.Email.LiveEmail();
            var setup = new LibLogic.Setup(mySqlInstance, mySqlDatabase, email, false);

            try
            {
                setup.Execute();
            }
            catch (Exception ex)
            {
                LibLogic.Helpers.Logging.Log(ex);
                email.SendMail_BackgroundThread("It appears the server setup failed: " + ex.Message,
                    "MajorsilnceVPN setup failure on application_start", LibLogic.Helpers.SiteInfo.AdminEmail, false, null);
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddTransient<MySqlConnection>(_ => new MySqlConnection(Configuration["ConnectionStrings:LocalMySqlServer"]));
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
