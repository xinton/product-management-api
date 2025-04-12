using Microsoft.EntityFrameworkCore;
using apiB2e.Entities;
using apiB2e.Interfaces;
using apiB2e.Infrastructure;
using apiB2e.Infrastructure.Repositories;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate(); // await db.Database.MigrateAsync();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Seed database
await DataSeeder.SeedProductsAsync(app.Services);

// Authentication endpoint
app.MapPost("/api/auth/login", async (LoginRequest request, IUserRepository userRepository) =>
{
    var isValid = await userRepository.ValidateUserAsync(request.Login, request.Password);
    
    if (!isValid)
    {
        return Results.Unauthorized();
    }
    
    // TODO generate a JWT token
    return Results.Ok(new { message = "Login successful"});
})
.WithName("Login")
.WithOpenApi();

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

// Export products to Excel
app.MapGet("/api/produtos/export", async (IProductRepository productRepository) =>
{
    var products = await productRepository.GetAllProductsAsync();
    
    using var workbook = new XLWorkbook();
    var worksheet = workbook.Worksheets.Add("Produtos");
    
    // Add headers
    worksheet.Cell(1, 1).Value = "ID";
    worksheet.Cell(1, 2).Value = "Nome";
    worksheet.Cell(1, 3).Value = "Valor (R$)";
    worksheet.Cell(1, 4).Value = "Data de Inclusão";
    
    // Add data
    int row = 2;
    foreach (var product in products)
    {
        worksheet.Cell(row, 1).Value = product.IdProduto;
        worksheet.Cell(row, 2).Value = product.Nome;
        worksheet.Cell(row, 3).Value = product.Valor;
        worksheet.Cell(row, 4).Value = product.DataInclusao.ToString("dd/MM/yyyy HH:mm:ss");
        row++;
    }
    
    // Format header and autofit columns
    var headerRow = worksheet.Row(1);
    headerRow.Style.Font.Bold = true;
    worksheet.Columns().AdjustToContents();
    
    using var stream = new MemoryStream();
    workbook.SaveAs(stream);
    stream.Position = 0;
    
    return Results.File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "produtos.xlsx");
})
.WithName("ExportProducts")
.WithOpenApi();

app.Run();

// DTO for login request
public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}