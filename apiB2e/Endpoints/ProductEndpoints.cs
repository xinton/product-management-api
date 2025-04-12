using apiB2e.Interfaces;
using apiB2e.Services;
using apiB2e.Entities;

namespace apiB2e.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
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

        app.MapGet("/api/produtos/export", HandleExportProducts)
          .WithName("ExportProducts")
          .WithOpenApi();
    }

    private static async Task<IResult> HandleExportProducts(
        IProductRepository productRepository,
        ExcelExportService excelService)
    {
        var products = await productRepository.GetAllProductsAsync();
        var excelBytes = excelService.ExportProductsToExcel(products);

        return Results.File(
            excelBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "produtos.xlsx"
        );
    }
}