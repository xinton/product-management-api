using Microsoft.EntityFrameworkCore;
using apiB2e.Entities;
using apiB2e.Interfaces;
using apiB2e.Infrastructure;

namespace apiB2e.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(int page, int pageSize, string sortOrder)
        {
            IQueryable<Product> query = _context.Products;

            // Apply sorting
            query = sortOrder.ToLower() == "desc" 
                ? query.OrderByDescending(p => p.Nome)
                : query.OrderBy(p => p.Nome);

            // Apply pagination
            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            product.DataInclusao = System.DateTime.UtcNow;
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.IdProduto);
            
            if (existingProduct == null)
                return product;
                
            existingProduct.Nome = product.Nome;
            existingProduct.Valor = product.Valor;
            
            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();
            
            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            
            if (product == null)
                return false;
                
            _context.Products.Remove(product);
            var result = await _context.SaveChangesAsync();
            
            return result > 0;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Products.CountAsync();
        }
    }
}