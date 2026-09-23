using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecom.WebApiApp.Data;
using Ecom.WebApiApp.Models.DTOMapper.cs;
using Ecom.WebApiApp.DTOMapper;
using Ecom.WebApiApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecom.WebApiApp.Controllers
{
    /// <summary>
    /// This is Product API Group.
    /// Here you can perform GET, POST, PUT, DELETE
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        // GET: api/Products
        /// <summary>Retrieves All products.</summary>
        /// <response code="200">Returns all products</response>
        [ProducesResponseType(typeof(ActionResult<IEnumerable<ProductDto>>), StatusCodes.Status200OK)]
        [HttpGet("/api/All-Products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            ActionResult<IEnumerable<ProductDto>> products = await _context.Products.Select(P => ProductDtoMapper.MapToDTO(P)).ToListAsync();
            return products;
        }

        // GET: api/Product/5
        /// <summary>Retrieves a single product by its ID.</summary>
        /// <param name="id">The product's primary key.</param>
        /// <response code="200">Product exists with the given ID.</response>
        /// <response code="404">No product exists with the given ID.</response>
        /// <response code="400">Bad request.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> GetProduct(string id)
        {
            Product? product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(ProductDtoMapper.MapToDTO(product));
        }

        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Update the product by using ID and Product Patch Object
        /// </summary>
        /// <param name="id">The product's primary key.</param>
        /// <param name="dto">The Product Patch Object</param>
        /// <response code="200">Product is successfully updated.</response>
        /// <response code="400">Product is not exists with the given ID.</response>
        [ProducesResponseType(typeof(NoContent), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(string id, ProductPatchDto dto)
        {

            Product? product = await _context.Products.FindAsync(id);

            if (id != product?.Id)
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
        /// <summary>
        /// To create new product
        /// </summary>
        /// <param name="productDTO">Enter product details</param>
        /// <response code="200">Product is successfully added.</response>
        [ProducesResponseType(typeof(NoContent), StatusCodes.Status200OK)]
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
        /// <summary>
        /// To Delete the product
        /// </summary>
        /// <param name="id">The primary key of product</param>
        [ProducesResponseType(typeof(NoContent), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFound), StatusCodes.Status404NotFound)]
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
