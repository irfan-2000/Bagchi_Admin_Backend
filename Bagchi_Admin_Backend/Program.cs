using Bagchi_Admin_Backend.Models;
using Bagchi_Admin_Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
 
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ILiveStreamingService, LiveStreamingService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));
    

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Keep property names as-is (PascalCase)
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
}); ;
builder.Services.AddHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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
