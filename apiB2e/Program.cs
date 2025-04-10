using Microsoft.EntityFrameworkCore;
using apiB2e.Entities;
using apiB2e.Interfaces;
using apiB2e.Infrastructure;
using apiB2e.Infrastructure.Repositories;

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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Seed database
await DataSeeder.SeedProductsAsync(app.Services);

// Product endpoints
app.MapGet("/api/produtos", async (
    int page, 
    int pageSize, 
    string sortOrder, 
    IProductRepository productRepository) =>
{
    if (page <= 0) page = 1;
    if (pageSize <= 0) pageSize = 10;
    if (string.IsNullOrEmpty(sortOrder)) sortOrder = "asc";

    var products = await productRepository.GetAllProductsAsync(page, pageSize, sortOrder);
    var totalCount = await productRepository.GetTotalCountAsync();

    return Results.Ok(new
    {
        items = products,
        totalCount,
        page,
        pageSize,
        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
    });
})
.WithName("GetProducts")
.WithOpenApi();

app.MapGet("/api/produtos/{id}", async (int id, IProductRepository productRepository) =>
{
    var product = await productRepository.GetProductByIdAsync(id);
    
    if (product == null)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(product);
})
.WithName("GetProductById")
.WithOpenApi();

app.MapPost("/api/produtos", async (Product product, IProductRepository productRepository) =>
{
    var result = await productRepository.AddProductAsync(product);
    return Results.Created($"/api/produtos/{result.IdProduto}", result);
})
.WithName("CreateProduct")
.WithOpenApi();

app.MapPut("/api/produtos/{id}", async (int id, Product product, IProductRepository productRepository) =>
{
    if (id != product.IdProduto)
    {
        return Results.BadRequest();
    }
    
    var existingProduct = await productRepository.GetProductByIdAsync(id);
    
    if (existingProduct == null)
    {
        return Results.NotFound();
    }
    
    var result = await productRepository.UpdateProductAsync(product);
    return Results.Ok(result);
})
.WithName("UpdateProduct")
.WithOpenApi();

app.MapDelete("/api/produtos/{id}", async (int id, IProductRepository productRepository) =>
{
    var success = await productRepository.DeleteProductAsync(id);
    
    if (!success)
    {
        return Results.NotFound();
    }
    
    return Results.NoContent();
})
.WithName("DeleteProduct")
.WithOpenApi();

app.Run();

// DTO for login request
public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}