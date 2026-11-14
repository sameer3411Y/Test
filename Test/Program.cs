using Microsoft.EntityFrameworkCore;
using Test.Data;

var builder = WebApplication.CreateBuilder(args);

// EF Core - SQL Server with Windows Authentication
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC Controllers
builder.Services.AddControllers();

// Swagger / Swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title   = "Student API",
        Version = "v1",
        Description = "CRUD API for managing students"
    });
});

var app = builder.Build();

// Always show Swagger (move inside if-block to restrict to Development only)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API v1");
    c.RoutePrefix = string.Empty; // Swagger UI at root: https://localhost:<port>/
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
