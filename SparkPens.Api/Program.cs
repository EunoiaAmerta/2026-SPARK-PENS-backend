using Microsoft.EntityFrameworkCore;
using SparkPens.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Tambahkan ini:
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Pindahkan ini ke paling atas pipeline agar selalu bisa diakses
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    // Ini penting! Memberitahu Swagger UI untuk mencari endpoint default
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "swagger"; // Menjamin akses lewat http://localhost:5151/swagger
});

if (app.Environment.IsDevelopment())
{
   // app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseHttpsRedirection();
app.UseAuthorization(); // Tambahkan ini sebelum MapControllers jika ada
app.MapControllers();
app.Run();


