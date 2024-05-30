using System;
using System.Collections.Generic;
using System.Linq;

namespace Labb3ProgTemplate.Managers;

public static class CurrencyManager
{
    public static readonly Dictionary<string, double> Currencies = new()
    {
        {":- SEK", 1.00},
        {"$ USD", 0.091},
        {"€ EUR", 0.085},
        {"¥ YEN", 13.90}
    };

    public static string CurrentCurrency { get; set; } = Currencies.Keys.First();

    public static event Action CurrentCurrencyChanged;

    public static void UpdateAllProductPricesToCurrentCurrency(string newCurrency)
    {
        foreach (var product in ProductManager.Products)
        {
            // Stötte på fett störande problem med double division, casta till decimal funkade fint
            decimal updatedPrice = (decimal)product.Price;
            updatedPrice /= (decimal)Currencies[CurrentCurrency];
            updatedPrice *= (decimal)Currencies[newCurrency];
            product.Price = (double)updatedPrice;
        }

        CurrentCurrency = newCurrency;
        CurrentCurrencyChanged.Invoke();
    }

    public static double ConvertPriceToSEK(double price)
    {
        decimal updatedPrice = (decimal)price;
        updatedPrice /= (decimal)Currencies[CurrentCurrency];
        updatedPrice *= (decimal)Currencies[Currencies.Keys.First()];

        return (double)updatedPrice;
    }
}
