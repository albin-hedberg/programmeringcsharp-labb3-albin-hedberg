using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Labb3ProgTemplate.DataModels.Products;
using Labb3ProgTemplate.Enums;

namespace Labb3ProgTemplate.DataModels.Users;

public abstract class User : INotifyPropertyChanged
{
    public string Name { get; }

    public string Password { get; }

    public abstract UserTypes Type { get; }

    public bool LoggedIn { get; set; }

    private List<Product> _cart;
    public List<Product> Cart
    {
        get => _cart;
        set
        {
            _cart = value; 
            OnPropertyChanged();
        }
    }

    protected User(string name, string password)
    {
        Name = name;
        Password = password;
        LoggedIn = false;
        _cart = new List<Product>();
    }

    public bool Authenticate(string password)
    {
        return Password.Equals(password);
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
