using Labb3ProgTemplate.Enums;

namespace Labb3ProgTemplate.DataModels.Products;

public class Food : Product
{
    public override ProductTypes Type { get; set; } = ProductTypes.Food;

    public Food(string name, double price, int quantity, string imageUrl) : base(name, price, quantity, imageUrl)
    {
        ImageUrl = imageUrl.Length < 1 ? "https://i.imgur.com/CXRa38r.png" : imageUrl;
    }
}
