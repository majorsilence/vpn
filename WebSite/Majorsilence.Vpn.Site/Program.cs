using System;
using System.Security.Policy;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Logic.Helpers;
using Majorsilence.Vpn.Logic.Payments;
using Majorsilence.Vpn.Site;
using Majorsilence.Vpn.Site.Helpers;
using Majorsilence.Vpn.Site.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".majorsilence.vpn.site.session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddLocalization(options => options.ResourcesPath = "App_GlobalResources");

builder.Services.AddControllersWithViews();

builder.Services.AddTransient(_ =>
    new MySqlConnection(builder.Configuration["ConnectionStrings:LocalMySqlServer"]));

builder.Services.AddScoped<Majorsilence.Vpn.Logic.AppSettings.Settings>(i =>
{
    return builder.Configuration.GetSection("Settings").Get<Majorsilence.Vpn.Logic.AppSettings.Settings>();
});
builder.Services.AddScoped<IEmail>(i =>
{
    var s = i.GetService<Majorsilence.Vpn.Logic.AppSettings.Settings>().Smtp;
    return new LiveEmail(s.FromAddress, s.Username, s.Password, s.Host, s.Port);
});
builder.Services.AddScoped<IPaypalSettings>(i => i.GetService<Majorsilence.Vpn.Logic.AppSettings.Settings>().Paypal);
builder.Services.AddScoped<IEncryptionKeysSettings>(i => i.GetService<Majorsilence.Vpn.Logic.AppSettings.Settings>().EncryptionKeys);
builder.Services.AddScoped<ISessionVariables, SessionVariables>();

builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var app = builder.Build();
if (builder.Environment.IsDevelopment())
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

app.Run();