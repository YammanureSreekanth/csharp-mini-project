using Ecom.WebApiApp.Data;
using Ecom.WebApiApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecom.WebApiApp.Repos;

public class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<bool> AddAsync(Product product)
    { 
        await _context.Products.AddAsync(product);
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            if (await ProductExists(product.Id))
            {
                return false;
            }
            else
            {
                throw;
            }
        }

    }

    public async Task<bool> UpdateAsync(Product product)
    {
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ProductExists(product.Id))
            {
                return false;
            }
            else
            {
                throw;
            }
        }
    }

    public async Task<Product> GetByIdAsync(string id)
    {
        Product? product = await _context.Products.FindAsync(id);

        return product;
    }

    public async Task<bool> DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
       try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ProductExists(product.Id))
            {
                return false;
            }
            else
            {
                throw;
            }
        }
    }

    private async Task<bool> ProductExists(string id)
    {
        return await _context.Products.AnyAsync(e => e.Id == id);
    }
}