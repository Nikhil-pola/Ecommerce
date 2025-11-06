using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IproductRepository repo;
    public ProductsController(IproductRepository repo)
    {
        this.repo = repo;
    }
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
    {
        return Ok(await repo.GetProductsAsync(brand,type,sort));
    }

    [HttpGet("{id:int}")] //api/prodcuts/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);

        if (product == null) return NotFound();
        return product;
    }
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repo.AddProduct(product);

        if(await repo.SaveAllAsync())
        {
            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product);
        }
        return BadRequest("Failed to create product");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !GetProductById(id))
        {
            return BadRequest(" Cannot update the product");
        }
        repo.UpdateProduct(product);
        if(await repo.SaveAllAsync())
        {
            return NoContent();
        }
        return BadRequest("Failed to update the product");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);

        if (product == null) return NotFound();

        repo.DeleteProduct(id);
        if (await repo.SaveAllAsync())
        {
            return NoContent();
        }
        return BadRequest("Failed to delete the product");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        return Ok(await repo.GetBrandAsync());
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        return Ok(await repo.GetTypesAsync());
    }


    private bool GetProductById(int id)
    {
        return repo.GetProductById(id);
    }
    
    
}
