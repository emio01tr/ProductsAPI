using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Dto;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    //localhost/api/products
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsContext _context;
        public ProductsController(ProductsContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet("Get-AllProducts")]
        
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.Select(x => ProductToDto(x)).ToListAsync();

            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productEntity = await _context.Products.FirstOrDefaultAsync(x => x.id == id);

            if (productEntity == null)
            {
                return NotFound(new {message = $"{id} Id'li ürün bulunamadý."});
            }
            var ProductDto = ProductToDto(productEntity);

            return Ok(ProductDto);
        }

        [HttpPost("Create-Product")]
        [Authorize]
        public async Task<IActionResult> CreateProduct(Product model)
        {
            if(model.ProductName == "")
            {
                return BadRequest("Urunun ismi bos gecilemez, lutfen yanlis parametreyi duzeltin.");
            }

            await _context.Products.AddAsync(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = model.id }, model);
        }

        [HttpPut("Update-Product")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(Product model)
        {
            var product = await _context.Products.Select(x => ProductToDto(x)).FirstOrDefaultAsync(x => x.id == model.id);

            if (product == null)
            {
                return NotFound();
            }

            product.ProductName = model.ProductName;
            product.Price = model.Price;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpDelete("Delete-ProductBy-{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var product = await _context.Products.FirstOrDefaultAsync(x => x.id == id);
            var deletedProduct = product;
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok(deletedProduct);
        }

        private static ProductDto ProductToDto(Product model)
        {
            return new ProductDto
            {
                id = model.id,
                Price = model.Price,
                ProductName = model.ProductName
            };
        }
    }

    }