using Microsoft.EntityFrameworkCore;
using apiB2e.Entities;
using apiB2e.Infrastructure;
using apiB2e.Infrastructure.Repositories;

namespace apiB2e.Infrastructure
{
    public static class DataSeeder
    {
        public static async Task SeedProductsAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure the database is created
            await context.Database.MigrateAsync();

            // Seed some sample products if needed
            if (!context.Products.Any())
            {
                var products = new[]
                {
                    new Product { Nome = "Produto 1", Valor = 19.99M, DataInclusao = DateTime.UtcNow },
                    new Product { Nome = "Produto 2", Valor = 29.99M, DataInclusao = DateTime.UtcNow },
                    new Product { Nome = "Produto 3", Valor = 39.99M, DataInclusao = DateTime.UtcNow },
                    new Product { Nome = "Produto 4", Valor = 49.99M, DataInclusao = DateTime.UtcNow },
                    new Product { Nome = "Produto 5", Valor = 59.99M, DataInclusao = DateTime.UtcNow }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}