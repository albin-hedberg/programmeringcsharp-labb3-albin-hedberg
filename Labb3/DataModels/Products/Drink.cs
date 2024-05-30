using Labb3ProgTemplate.Enums;

namespace Labb3ProgTemplate.DataModels.Products;

public class Drink : Product
{
    public override ProductTypes Type { get; set; } = ProductTypes.Drink;

    public Drink(string name, double price, int quantity, string imageUrl) : base(name, price, quantity, imageUrl)
    {
        ImageUrl = imageUrl.Length < 1 ? "https://i.imgur.com/EexVSNz.png" : imageUrl;
    }
}
