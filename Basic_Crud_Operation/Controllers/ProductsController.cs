using Basic_Crud_Operation.Dtos;
using Basic_Crud_Operation.Models;
using Basic_Crud_Operation.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basic_Crud_Operation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository repo , ILogger<ProductsController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // GET api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            try
            {
                var products = await _repo.GetAllAsync();
                return Ok(products.Select(ToDto));
            }
            catch (Exception ex)
            {
                return ServerError(ex, "retrieving products");
            }
        }


        // GET api/products/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            if (id <= 0) return InvalidId(id);

            try
            {
                var product = await _repo.GetByIdAsync(id);
                if (product is null) return ProductNotFound(id);

                return Ok(ToDto(product));
            }
            catch (Exception ex)
            {
                return ServerError(ex, $"retrieving product {id}");
            }
        }

        // POST api/products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(ProductCreateDto dto)
        {
            var name = dto.Name.Trim();

            try
            {
                if (await _repo.NameExistsAsync(name)) return DuplicateName(name);

                var product = new Product
                {
                    Name = name,
                    Price = dto.Price,
                    Quantity = dto.Quantity
                };

                var created = await _repo.AddAsync(product);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
            }
            catch (Exception ex)
            {
                return ServerError(ex, "creating a product");
            }
        }


        // PUT api/products/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
        {
            if (id <= 0) return InvalidId(id);

            var name = dto.Name.Trim();

            try
            {
                if (!await _repo.ExistsAsync(id)) return ProductNotFound(id);

                // excludeId = id, so the product does not clash with itself
                if (await _repo.NameExistsAsync(name, excludeId: id)) return DuplicateName(name);

                var updated = new Product { Name = name, Price = dto.Price, Quantity = dto.Quantity };
                var success = await _repo.UpdateAsync(id, updated);

                // Product could have been deleted by another request in between
                if (!success) return ProductNotFound(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return ServerError(ex, $"updating product {id}");
            }
        }




        // DELETE api/products/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return InvalidId(id);

            try
            {
                if (!await _repo.ExistsAsync(id)) return ProductNotFound(id);

                var success = await _repo.DeleteAsync(id);
                if (!success) return ProductNotFound(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return ServerError(ex, $"deleting product {id}");
            }
        }



        // ---------- Helpers: one place for every error response ----------

        private ObjectResult InvalidId(int id) => Problem(
            title: "Invalid product Id.",
            detail: $"Id must be greater than 0, but received {id}.",
            statusCode: StatusCodes.Status400BadRequest);

        private ObjectResult ProductNotFound(int id) => Problem(
            title: "Product not found.",
            detail: $"No product exists with Id {id}.",
            statusCode: StatusCodes.Status404NotFound);

        private ObjectResult DuplicateName(string name) => Problem(
            title: "Duplicate product name.",
            detail: $"A product named '{name}' already exists.",
            statusCode: StatusCodes.Status409Conflict);

        private ObjectResult ServerError(Exception ex, string operation)
        {
            _logger.LogError(ex, "Unhandled error while {Operation}", operation);

            return Problem(
                title: "An unexpected error occurred.",
                detail: $"Something went wrong while {operation}. Please try again later.",
                statusCode: StatusCodes.Status500InternalServerError);
        }




        private static ProductDto ToDto(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Quantity = p.Quantity,
            CreatedAt = p.CreatedAt
        };

    }
}
