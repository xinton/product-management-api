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

            // Check if users already exist
            if (!context.Users.Any())
            {
                // Add default admin user
                var adminUser = new User
                {
                    Login = "admin",
                    Senha = UserRepository.HashPassword("admin123"),  // Hashed password
                    DataInclusao = DateTime.UtcNow
                };

                // Add test user
                var testUser = new User
                {
                    Login = "teste",
                    Senha = UserRepository.HashPassword("teste123"),  // Hashed password
                    DataInclusao = DateTime.UtcNow
                };

                context.Users.AddRange(adminUser, testUser);
                await context.SaveChangesAsync();
            }

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