using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Labb3ProgTemplate.DataModels.Products;
using Labb3ProgTemplate.Enums;

namespace Labb3ProgTemplate.Managers;

public static class ProductManager
{
    private static readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Labb3");
    private static readonly string _productsPath = Path.Combine(_directory, "Products.json");

    private static readonly IEnumerable<Product>? _products = new List<Product>();
    public static IEnumerable<Product>? Products => _products;

    public static readonly List<Product>? ProductsModifier = Products as List<Product>;

    // Skicka detta efter att produktlistan ändrats eller lästs in
    public static event Action ProductListChanged;

    public static void AddProduct(Product product)
    {
        ProductsModifier.Add(product);
        ProductListChanged.Invoke();
    }

    public static void UpdateProduct(Product product)
    {
        int productIndex = ProductsModifier.IndexOf(product);

        ProductsModifier[productIndex].Name = product.Name;
        ProductsModifier[productIndex].Price = product.Price;
        ProductsModifier[productIndex].ImageUrl = product.ImageUrl;

        ProductListChanged.Invoke();
    }

    public static void RemoveProduct(Product product)
    {
        if (!ProductsModifier.Contains(product))
        {
            return;
        }

        ProductsModifier.Remove(product);
        ProductListChanged.Invoke();
    }

    public static async Task SaveProductsToFile()
    {
        if (CurrencyManager.CurrentCurrency != ":- SEK")
        {
            CurrencyManager.UpdateAllProductPricesToCurrentCurrency(":- SEK");
        }

        foreach (var product in ProductsModifier)
        {
            product.Quantity = 1;
        }

        Directory.CreateDirectory(_directory);

        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(Products, jsonOptions);

        await using var sw = new StreamWriter(_productsPath);
        await sw.WriteAsync(json);
    }

    public static async Task LoadProductsFromFile()
    {
        if (!File.Exists(_productsPath))
        {
            return;
        }

        var text = string.Empty;

        using var sr = new StreamReader(_productsPath);
        text = await sr.ReadToEndAsync();

        var deserializedProducts = new List<Product>();

        using (var jsonDoc = JsonDocument.Parse(text))
        {
            if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var jsonElement in jsonDoc.RootElement.EnumerateArray())
                {
                    Product p;
                    switch (jsonElement.GetProperty("Type").GetByte())
                    {
                        case (int)ProductTypes.Food:
                            p = jsonElement.Deserialize<Food>();
                            deserializedProducts.Add(p);
                            break;
                        case (int)ProductTypes.Drink:
                            p = jsonElement.Deserialize<Drink>();
                            deserializedProducts.Add(p);
                            break;
                    }
                }
            }
        }

        if (!deserializedProducts.Any())
        {
            return;
        }

        ProductsModifier.AddRange(deserializedProducts);
    }
}
