using Ecom.WebApiApp.DTOMapper;
using Ecom.WebApiApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.WebApiApp.Repos;
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> GetByIdAsync(string id);
    Task<bool> UpdateAsync(Product product);
    Task<bool> AddAsync(Product product);
    Task<bool> DeleteAsync(Product product);
}