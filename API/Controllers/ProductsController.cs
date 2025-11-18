using System;
using API.ResquestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specification;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class ProductsController : BaseAPIController
{
    private readonly IGenericRepository<Product> repo;
    public ProductsController(IGenericRepository<Product> repo)
    {
        this.repo = repo;
    }
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecPrarams specPrarams)
    {
        var spec = new ProductSpecification(specPrarams);
        
        return await CreatePagedResult(repo, spec, specPrarams.PageIndex, specPrarams.PageSize);
    }

    [HttpGet("{id:int}")] //api/prodcuts/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);

        if (product == null) return NotFound();
        return product;
    }
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repo.Add(product);

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
        repo.Update(product);
        if(await repo.SaveAllAsync())
        {
            return NoContent();
        }
        return BadRequest("Failed to update the product");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);

        if (product == null) return NotFound();

        repo.Remove(product);
        if (await repo.SaveAllAsync())
        {
            return NoContent();
        }
        return BadRequest("Failed to delete the product");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var spec = new BrandListSpecification();
        return Ok(await repo.ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeListSpecification();
        return Ok(await repo.ListAsync(spec));
    }


    private bool GetProductById(int id)
    {
        return repo.Exists(id);
    }
    
    
}
