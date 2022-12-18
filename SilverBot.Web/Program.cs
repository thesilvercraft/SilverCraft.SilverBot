/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using Microsoft.AspNetCore.Authentication.Cookies;
using Radzen;
using SilverBotDS.Objects;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
if (Debugger.IsAttached && !Environment.CurrentDirectory.EndsWith("bin\\Debug\\net7.0"))
{
    Environment.CurrentDirectory += Environment.OSVersion.Platform == PlatformID.Win32NT
        ? "\\bin\\Debug\\net7.0"
        : "/bin/Debug/net7.0";
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