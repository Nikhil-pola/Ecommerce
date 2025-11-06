using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ProductRepository : IproductRepository
{
    private readonly StoreContext context;
    public ProductRepository(StoreContext context)
    {
        this.context = context;
    }
    public void AddProduct(Product product)
    {
        context.Products.Add(product);
    }

    public void DeleteProduct(int id)
    {
        var product = context.Products.Find(id);
        if (product != null)
        {
            context.Products.Remove(product);
        }
    }

    public async Task<IReadOnlyList<string>> GetBrandAsync()
    {
        return await context.Products.Select(c => c.Brand).Distinct().ToListAsync();
    }

    public bool GetProductById(int id)
    {
        return context.Products.Any(p => p.Id == id);
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort)
    {
        var query = context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(x => x.Brand == brand);
        }
        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(x => x.Type == type);
        }
        query = sort switch
            {
                "priceAsc" => query.OrderBy(x => x.Price),
                "priceDesc" => query.OrderByDescending(x => x.Price),
                 _ => query.OrderBy(x => x.Name)
            };
        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync()
    {
        return await context.Products.Select(c => c.Type).Distinct().ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void UpdateProduct(Product product)
    {
        context.Entry(product).State = EntityState.Modified;
    }
}
