using System;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Logic.Payments;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using VpnSite.Models;

namespace Majorsilence.Vpn.Site;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        var vpnConnectionString = Configuration.GetConnectionString("MySqlVpn");
        var sessionConnectionString = Configuration.GetConnectionString("MySqlSessions");

        var s = Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
        var email = new LiveEmail(s.FromAddress, s.Username, s.Password, s.Host, s.Port);
        var setup = new InitializeSettings(vpnConnectionString, sessionConnectionString, email, false);

        try
        {
            Retry.Do(() => { setup.Execute(); }, TimeSpan.FromSeconds(2), 5);
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            email.SendMail_BackgroundThread("It appears the server setup failed: " + ex.Message,
                "MajorsilnceVPN setup failure on application_start", SiteInfo.AdminEmail, false);
        }
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.Cookie.Name = ".majorsilence.vpn.site.session";
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddLocalization(options => options.ResourcesPath = "App_GlobalResources");

        services.AddControllersWithViews();

        services.AddTransient(_ =>
            new MySqlConnection(Configuration["ConnectionStrings:LocalMySqlServer"]));

        services.AddScoped<IEmail>(i =>
        {
            var s = Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
            return new LiveEmail(s.FromAddress, s.Username, s.Password, s.Host, s.Port);
        });
        services.AddScoped<IPaypalSettings>(i => Configuration.GetSection("Paypal").Get<PaypalSettings>());
        services.AddScoped<ISessionVariables, SessionVariables>();

        services.AddMvc()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
        services.AddControllersWithViews().AddRazorRuntimeCompilation();
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
            endpoints.MapRazorPages();
            endpoints.MapControllerRoute(
                "areas",
                "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );
            endpoints.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}");
        });
    }
}