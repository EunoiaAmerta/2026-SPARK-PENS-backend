using Microsoft.EntityFrameworkCore;
using SparkPens.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. CORS Configuration (Penting untuk Frontend!)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin() // Di produksi nanti ini harus spesifik URL Frontend
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Swagger Setup
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "swagger"; 
});

// 4. Middleware Pipeline
app.UseHttpsRedirection();

// Gunakan Policy CORS yang sudah dibuat di atas
app.UseCors("AllowAll"); 

app.UseAuthorization();

app.MapControllers();

// Railway inject environment variable PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

app.Urls.Add($"http://0.0.0.0:{port}");

app.MapGet("/", () => "SparkPens API Running 🚀");


app.Run();