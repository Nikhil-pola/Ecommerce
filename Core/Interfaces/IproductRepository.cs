using System;
using Core.Entities;
namespace Core.Interfaces;

public interface IproductRepository
{
    Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort);
    Task<Product?> GetProductByIdAsync(int id);

    Task<IReadOnlyList<String>> GetBrandAsync();
    Task<IReadOnlyList<String>> GetTypesAsync();
    void AddProduct(Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(int id);
    bool GetProductById(int id);
    Task<bool> SaveAllAsync();

}
