using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Site.Helpers;
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
using VpnSite.Models;

namespace Majorsilence.Vpn.Site
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var mySqlInstance = System.Configuration.ConfigurationManager.AppSettings["MySqlInstance"].ToString();
            var mySqlDatabase = System.Configuration.ConfigurationManager.AppSettings["MySqlDatabase"].ToString();

            var s = Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
            var email = new Majorsilence.Vpn.Logic.Email.LiveEmail(s.FromAddress, s.Username, s.Password, s.Host, s.Port);
            var setup = new Majorsilence.Vpn.Logic.Setup(mySqlInstance, mySqlDatabase, email, false);

            try
            {
                setup.Execute();
            }
            catch (Exception ex)
            {
                Majorsilence.Vpn.Logic.Helpers.Logging.Log(ex);
                email.SendMail_BackgroundThread("It appears the server setup failed: " + ex.Message,
                    "MajorsilnceVPN setup failure on application_start", Majorsilence.Vpn.Logic.Helpers.SiteInfo.AdminEmail, false, null);
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.Name = ".majorsilence.vpn.site.session";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            services.AddControllersWithViews();

            services.AddTransient<MySqlConnection>(_ => new MySqlConnection(Configuration["ConnectionStrings:LocalMySqlServer"]));

            services.AddScoped<IEmail>(i =>
            {
                var s = Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
                return new Majorsilence.Vpn.Logic.Email.LiveEmail(s.FromAddress, s.Username, s.Password, s.Host, s.Port);
            });
            services.AddScoped<ISessionVariables, SessionVariables>();
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
            app.UseSession();

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
