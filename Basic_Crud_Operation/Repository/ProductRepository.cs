using Basic_Crud_Operation.Data;
using Basic_Crud_Operation.Models;
using Microsoft.EntityFrameworkCore;

namespace Basic_Crud_Operation.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(AppDbContext context, ILogger<ProductRepository> logger) 
        { 
            _context = context;
            _logger = logger;

        }

        public async Task<List<Product>> GetAllAsync() 
        {
            try
            {
                return await _context.Products.AsNoTracking().ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while fetching all products");
                throw; // Rethrow the exception to be handled by the caller
                
            }

        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
            try
            {
                return await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while fetching product {ProductId}", id);
                throw;
            }

        }

        public async Task<Product> AddAsync(Product product)
        {
            ArgumentNullException.ThrowIfNull(product);
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();   // runs the INSERT
                return product; // Id and CreatedAt are now filled in

            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating product {ProductName}", product.Name);
                throw;
            }
        }

        // UPDATE
        public async Task<bool> UpdateAsync(int id, Product updated)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
            ArgumentNullException.ThrowIfNull(updated);

            try
            {
                var existing = await _context.Products.FindAsync(id);
                if (existing is null) return false;

                existing.Name = updated.Name;
                existing.Price = updated.Price;
                existing.Quantity = updated.Quantity;

                await _context.SaveChangesAsync();   // EF detects changes and runs UPDATE
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating product {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            try
            {
                var existing = await _context.Products.FindAsync(id);
                if (existing is null) return false;

                _context.Products.Remove(existing);
                await _context.SaveChangesAsync();   // runs the DELETE
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting product {ProductId}", id);
                throw;
            }
        }


        // New validation helpers

        public async Task<bool> ExistsAsync(int id)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            try
            {
                return await _context.Products.AnyAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while checking product {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            try
            {
                return await _context.Products.AnyAsync(p =>
                    p.Name == name && (excludeId == null || p.Id != excludeId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while checking product name {ProductName}", name);
                throw;
            }
        }


    }
}
