using Microsoft.AspNetCore.Authentication.Cookies;
using Radzen;
using SilverBotDS.Objects;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
if (Debugger.IsAttached && !Environment.CurrentDirectory.EndsWith("bin\\Debug\\net6.0"))
{
    Environment.CurrentDirectory += Environment.OSVersion.Platform == PlatformID.Win32NT
        ? "\\bin\\Debug\\net6.0"
        : "/bin/Debug/net6.0";
}
await SilverBotDS.Program.MainAsync(args,true);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMvc().AddXmlSerializerFormatters();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/api/unauthorized";
    //options.LogoutPath = "/logout";
    options.ExpireTimeSpan = new TimeSpan(7, 0, 0, 0);
}).AddDiscord(options => {
    options.ClientId = SilverBotDS.Program._config.LoginPageDiscordClientId.ToString();
    options.ClientSecret = SilverBotDS.Program._config.LoginPageDiscordClientSecret;

});
foreach (var service in SilverBotDS.Program.services)
{
    builder.Services.Add(service);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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
    endpoints.MapBlazorHub();
    endpoints.MapControllers();
    endpoints.MapFallbackToPage("/_Host");
});

app.Run();