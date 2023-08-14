
using BqsClinoTag;
using BqsClinoTag.Grool;
using BqsClinoTag.Hubs;
using BqsClinoTag.Models;
using BqsClinoTag.Services;
using BqsClinoTag.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Variables de session
//builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
var policyName = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName,
                      builder =>
                      {
                          builder
                            .AllowAnyOrigin().AllowAnyMethod()// Add the methods you need
                            .AllowAnyHeader();
                      });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    // Adding Jwt Bearer  
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
            TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["Demo-ClinoTag-Access-Token"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CLINOTAGBQSContext>();

builder.Services.AddControllersWithViews()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddTransient<IMailService, MailService>();

var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();
builder.Services.Configure<MailSettings>(config.GetSection("MailSettings"));


////if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
builder.Services.AddSingleton<ScheduleService>();
builder.Services.AddHostedService<ScheduleService>();

builder.Services.AddSignalR();

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePages(async context =>
{
    var request = context.HttpContext.Request;
    var response = context.HttpContext.Response;

    if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
        response.Redirect("/account/redirection?returnurl=" + request.Path);

    if (response.StatusCode == (int)HttpStatusCode.Forbidden)
        response.Redirect($"{Network.GetBaseUrl(context.HttpContext)}/Home/Interdit");

    if (response.StatusCode == (int)HttpStatusCode.NotFound)
        response.Redirect(Network.GetBaseUrl(context.HttpContext) + "/Home/Erreur?erreur=Fonctionnalité introuvable.&urlRetour=" + Network.GetBaseUrl(context.HttpContext));

});

// Commenté car la redirection est faite par l'Apache de Square (WIKI)
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(policyName);
app.UseSession();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

app.MapRazorPages();

app.Run();