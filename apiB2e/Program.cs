using Microsoft.EntityFrameworkCore;
using apiB2e.Entities;
using apiB2e.Interfaces;
using apiB2e.Infrastructure;
using apiB2e.Infrastructure.Repositories;
using apiB2e.Endpoints;
using apiB2e.Services;
using ClosedXML.Excel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ExcelExportService>();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    await ConfigureDevelopmentEnvironment(app);
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Seed database
await DataSeeder.SeedProductsAsync(app.Services);

// Map endpoints
app.MapAuthEndpoints();
app.MapProductEndpoints();

app.Run();

static async Task ConfigureDevelopmentEnvironment(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    
    app.UseSwagger();
    app.UseSwaggerUI();
    
    await DataSeeder.SeedProductsAsync(app.Services);
}

// DTO for login request
public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}