using System;
using Majorsilence.Vpn.Logic;
using Majorsilence.Vpn.Logic.AppSettings;
using Majorsilence.Vpn.Logic.Email;
using Majorsilence.Vpn.Site;
using Majorsilence.Vpn.Site.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

builder.Services.AddScoped(i => { return builder.Configuration.GetSection("Settings").Get<Settings>(); });
builder.Services.AddScoped<IEmail>(i =>
{
    var s = i.GetService<Settings>().Smtp;
    return new EmailWorkQueue(i.GetService<DatabaseSettings>());
});
builder.Services.AddScoped(i => i.GetService<Settings>().Paypal);
builder.Services.AddScoped<IEncryptionKeysSettings>(i => i.GetService<Settings>().EncryptionKeys);
builder.Services.AddScoped<ISessionVariables, SessionVariables>();
builder.Services.AddScoped(i => new DatabaseSettings(builder.Configuration["ConnectionStrings:MySqlVpn"],
    builder.Configuration["ConnectionStrings:MySqlSessions"],
    false,
    i.GetService<ILogger>()
));
builder.Services.AddScoped<ActionLog>();

builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddHostedService<StartupWorker>();

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