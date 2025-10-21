using Bagchi_Admin_Backend.Models;
using Bagchi_Admin_Backend.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
 
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ILiveStreamingService, LiveStreamingService>();
 

builder.Services.AddScoped<IQuizService, QuizService>();
 
builder.Services.AddScoped<ICourseService, CourseService>();
 

builder.Services.AddScoped<IStudentService, StudentService>();
 
builder.Services.AddScoped<IShayariService, shayservice>();
 

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ShayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShayConnection")));

// Add services to the container.
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));
 
int port = builder.Configuration.GetValue<int>("KestrelSettings:Port");


//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.ListenAnyIP(port, listenOptions =>
//    {
//        listenOptions.UseHttps("/etc/ssl/private/bagadmin_api_cert.pfx", "myPfxPassword"); // empty password
//    });
//});




builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Keep property names as-is (PascalCase)
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
}); ;
builder.Services.AddHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".avif"] = "image/avif"; // Add AVIF support
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(@"D:/Dr.Bagchi_Media"),
        RequestPath = "/media"
    });
    //app.UseStaticFiles(new StaticFileOptions
    //{
    //    FileProvider = new PhysicalFileProvider(@"E:\.Net Core Practice\My project\Udemy\PatientManagementMedia"),
    //    RequestPath = "/media"
    //});



}
else
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider("/var/www/PatientManagement/PatientManagementMedia"),
        RequestPath = "/media"
    });
}
app.MapControllers();

app.Run();
