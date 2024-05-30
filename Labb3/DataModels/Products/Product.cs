using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Labb3ProgTemplate.Enums;

namespace Labb3ProgTemplate.DataModels.Products;

public abstract class Product : INotifyPropertyChanged
{
    #region Properties

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    private double _price;
    public double Price
    {
        get => _price;
        set
        {
            _price = value;
            OnPropertyChanged();
        }
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            OnPropertyChanged();
        }
    }

    private string _imageUrl;
    public string ImageUrl
    {
        get => _imageUrl;
        set
        {
            _imageUrl = value;
            OnPropertyChanged();
        }
    }

    public abstract ProductTypes Type { get; set; }

    #endregion

    protected Product(string name, double price, int quantity, string imageUrl)
    {
        _name = name;
        _price = price;
        _quantity = quantity;
        _imageUrl = imageUrl;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
