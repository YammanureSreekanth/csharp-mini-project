using Microsoft.AspNetCore.Mvc;
using Ecom.WebApiApp.Models.DTOMapper;
using Ecom.WebApiApp.DTOMapper;
using Ecom.WebApiApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Ecom.WebApiApp.Repos;

namespace Ecom.WebApiApp.Controllers
{
    /// <summary>
    /// This is Product API Group.
    /// Here you can perform GET, POST, PUT, DELETE
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductRepository repository) : ControllerBase
    {
        private readonly IProductRepository _repository = repository;

        /// <summary>Retrieves All products.</summary>
        /// <response code="200">Returns all products</response>
        [ProducesResponseType(typeof(ActionResult<IEnumerable<ProductDto>>), StatusCodes.Status200OK)]
        [HttpGet("/api/All-Products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {   
            IEnumerable<Product> dbResults = await _repository.GetAllAsync();  
            IEnumerable<ProductDto> productDtos = dbResults.Select(ProductDtoMapper.MapToDTO);
            return Ok(productDtos);
        }

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
            Product? product = await _repository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(ProductDtoMapper.MapToDTO(product));
        }

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

            Product? product = await _repository.GetByIdAsync(id);

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

            try
            {
                if (await _repository.UpdateAsync(product))
                {
                    return NoContent();
                } else
                {
                    return NotFound();
                } 
            } catch (Exception)
            {
                // @TODO: Send better Status
                return NotFound();
            }

        }

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

            bool status = await _repository.AddAsync(ProductDtoMapper.MapDtoToModel(productDTO));
            
            try
            {
                if (!status)
                {
                    return Conflict();
                }
            } catch (Exception)
            {
                return Conflict();
            }

            return CreatedAtAction("GetProduct", new { id = productDTO.Id }, productDTO);
        }

        /// <summary>
        /// To Delete the product
        /// </summary>
        /// <param name="id">The primary key of product</param>
        [ProducesResponseType(typeof(NoContent), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFound), StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            Product? product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(product);

            return NoContent();
        }

    }
}
