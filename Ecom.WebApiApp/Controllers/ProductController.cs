using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecom.WebApiApp.Data;
using Ecom.WebApiApp.Models.DTOMapper.cs;
using Ecom.WebApiApp.DTOMapper;
using Ecom.WebApiApp.Models;

namespace Ecom.WebApiApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet("/api/All-Products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            return await _context.Products.Select(P => ProductDtoMapper.MapToDTO(P)).ToListAsync();
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(string id)
        {
            Product? product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return ProductDtoMapper.MapToDTO(product);
        }

        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(string id, ProductPatchDto dto)
        {

            Product? product = await _context.Products.FindAsync(id);

            if (id != product.Id)
            {
                return BadRequest();
            }

            if (dto.Name is not null) product.Name = dto.Name;

            if (dto.Price is not null) product.Price = dto.Price.Value;

            if (dto.IsOnline is bool Online) product.IsOnline = Online;

            if (dto.IsSearchable is bool Searchable) product.IsOnline = Searchable;

            if (dto.FabricCode is not null) product.FabricCode = dto.FabricCode;

            product.LastModifiedTime = DateTime.Now;

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto productDTO)
        {

            _context.Products.Add(ProductDtoMapper.MapDtoToModel(productDTO));
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(productDTO.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProduct", new { id = productDTO.Id }, productDTO);
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(string id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
