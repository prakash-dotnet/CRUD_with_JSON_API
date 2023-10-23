using CRUD_with_JSON_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
 
 

namespace CRUD_with_JSON_API.Controllers
{
    
        [Route("api/products")]
        [ApiController]
        public class ProductsController : ControllerBase
        {
            private readonly MySettings _settings;

            public ProductsController(IOptions<MySettings> settings)
            {
                _settings = settings.Value;
            }

            [HttpGet]
            public IActionResult GetProducts()
            {
                var products = ReadProductsFromJsonFile();
                return Ok(products);
            }

            [HttpGet("{id}")]
            public IActionResult GetProduct(int id)
            {
                var products = ReadProductsFromJsonFile();
                var product = products.FirstOrDefault(p => p.Id == id);

                if (product == null)
                {
                    return NotFound(); // 404 Not Found
                }

                return Ok(product);
            }

            [HttpPost]
            public IActionResult CreateProduct([FromBody] Product product)
            {
                if (product == null)
                {
                    return BadRequest(); // 400 Bad Request
                }

                var products = ReadProductsFromJsonFile();
                product.Id = GetNextProductId(products);
                products.Add(product);
                WriteProductsToJsonFile(products);

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }

            [HttpPut("{id}")]
            public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
            {
                if (updatedProduct == null)
                {
                    return BadRequest(); // 400 Bad Request
                }

                var products = ReadProductsFromJsonFile();
                var existingProduct = products.FirstOrDefault(p => p.Id == id);

                if (existingProduct == null)
                {
                    return NotFound(); // 404 Not Found
                }

                existingProduct.Name = updatedProduct.Name;
                existingProduct.Price = updatedProduct.Price;
                WriteProductsToJsonFile(products);

                return NoContent(); // 204 No Content
            }

            [HttpDelete("{id}")]
            public IActionResult DeleteProduct(int id)
            {
                var products = ReadProductsFromJsonFile();
                var productToDelete = products.FirstOrDefault(p => p.Id == id);

                if (productToDelete == null)
                {
                    return NotFound(); // 404 Not Found
                }

                products.Remove(productToDelete);
                WriteProductsToJsonFile(products);

                return NoContent(); // 204 No Content
            }

            private List<Product> ReadProductsFromJsonFile()
            {
                var jsonFilePath = _settings.JsonFilePath;
                if (System.IO.File.Exists(jsonFilePath))
                {
                    string json = System.IO.File.ReadAllText(jsonFilePath);
                    return JsonConvert.DeserializeObject<List<Product>>(json);
                }
                return new List<Product>();
            }

            private void WriteProductsToJsonFile(List<Product> products)
            {
                var jsonFilePath = _settings.JsonFilePath;
                string json = JsonConvert.SerializeObject(products, Formatting.Indented);
                System.IO.File.WriteAllText(jsonFilePath, json);
            }

            private int GetNextProductId(List<Product> products)
            {
                return products.Any() ? products.Max(p => p.Id) + 1 : 1;
            }
        }
    
}
