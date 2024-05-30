using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Labb3ProgTemplate.DataModels.Products;
using Labb3ProgTemplate.DataModels.Users;
using Labb3ProgTemplate.Enums;

namespace Labb3ProgTemplate.Managers;

public static class UserManager
{
    private static readonly string _directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Labb3");
    private static readonly string _usersPath = Path.Combine(_directory, "Users.json");

    private static readonly IEnumerable<User>? _users = new List<User>();
    private static User _currentUser;

    public static IEnumerable<User>? Users => _users;

    public static readonly List<User>? UsersModifier = Users as List<User>;

    public static User CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            CurrentUserChanged?.Invoke();
        }
    }

    public static double CartTotalPrice { get; set; }

    public static event Action CurrentUserChanged;

    // Skicka detta efter att användarlistan ändrats eller lästs in
    public static event Action UserListChanged;

    public static event Action UserAlreadyExist;

    public static event Action UserDoesntExist;

    public static event Action UserAuthenticationFailed;

    public static event Action UserCartChanged;

    public static bool IsAdminLoggedIn => CurrentUser.Type is UserTypes.Admin;

    public static void ChangeCurrentUser(string name, string password, UserTypes type)
    {
        switch (type)
        {
            case UserTypes.Admin:
                CurrentUser = new Admin(name, password) { LoggedIn = true };
                break;
            case UserTypes.Customer:
                CurrentUser = new Customer(name, password) { LoggedIn = true };
                break;
        }
    }

    public static void Login(string name, string password)
    {
        if (!CheckIfUserExist(name))
        {
            UserDoesntExist.Invoke();
            return;
        }

        int userIndex = UsersModifier.FindIndex(u => u.Name == name);
        var user = UsersModifier[userIndex];

        if (user.Authenticate(password))
        {
            ChangeCurrentUser(user.Name, user.Password, user.Type);
        }
        else
        {
            UserAuthenticationFailed.Invoke();
        }
    }

    public static void LogOut()
    {
        CurrentUser.LoggedIn = false;
        CurrentUserChanged?.Invoke();
    }

    private static bool CheckIfUserExist(string name)
    {
        return Users is not null && Users.Any(u => u.Name == name);
    }

    public static void RegisterNewUser(string name, string password, UserTypes type)
    {
        if (CheckIfUserExist(name))
        {
            UserAlreadyExist.Invoke();
            return;
        }

        switch (type)
        {
            case UserTypes.Customer:
                var newCustomer = new Customer(name, password);
                UsersModifier.Add(newCustomer);
                ChangeCurrentUser(newCustomer.Name, newCustomer.Password, newCustomer.Type);
                UserListChanged?.Invoke();
                break;
            case UserTypes.Admin:
                var newAdmin = new Admin(name, password);
                UsersModifier.Add(newAdmin);
                ChangeCurrentUser(newAdmin.Name, newAdmin.Password, newAdmin.Type);
                UserListChanged?.Invoke();
                break;
        }
    }

    public static async Task SaveUsersToFile()
    {
        Directory.CreateDirectory(_directory);

        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(Users, jsonOptions);

        await using var sw = new StreamWriter(_usersPath);
        await sw.WriteAsync(json);
    }

    public static async Task LoadUsersFromFile()
    {
        if (!File.Exists(_usersPath))
        {
            return;
        }

        var text = string.Empty;

        using var sr = new StreamReader(_usersPath);
        text = await sr.ReadToEndAsync();

        var deserializedUsers = new List<User>();

        using (var jsonDoc = JsonDocument.Parse(text))
        {
            if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var jsonElement in jsonDoc.RootElement.EnumerateArray())
                {
                    User u;
                    switch (jsonElement.GetProperty("Type").GetByte())
                    {
                        case 0:
                            u = jsonElement.Deserialize<Admin>();
                            deserializedUsers.Add(u);
                            break;
                        case 1:
                            u = jsonElement.Deserialize<Customer>();
                            deserializedUsers.Add(u);
                            break;
                    }
                }
            }
        }

        if (!deserializedUsers.Any())
        {
            return;
        }

        UsersModifier.AddRange(deserializedUsers);
    }

    #region Cart_Functions
    public static void AddToCart(Product product)
    {
        if (CurrentUser.Cart.Contains(product))
        {
            CurrentUser.Cart[CurrentUser.Cart.IndexOf(product)].Quantity++;
        }
        else
        {
            CurrentUser.Cart.Add(product);
        }

        UserCartChanged.Invoke();
    }

    public static void RemoveFromCart(Product product)
    {
        if (!CurrentUser.Cart.Contains(product))
        {
            return;
        }

        int productIndex = CurrentUser.Cart.IndexOf(product);

        if (CurrentUser.Cart[productIndex].Quantity > 1)
        {
            CurrentUser.Cart[productIndex].Quantity--;
        }
        else
        {
            CurrentUser.Cart.Remove(product);
        }

        UserCartChanged.Invoke();
    }

    public static double CartTotal()
    {
        if (!CurrentUser.Cart.Any())
        {
            return 0.0;
        }

        CartTotalPrice = 0.0;

        foreach (var product in CurrentUser.Cart)
        {
            CartTotalPrice += product.Price;
        }

        return CartTotalPrice;
    }

    public static void Checkout()
    {
        foreach (var product in CurrentUser.Cart)
        {
            product.Quantity = 1;
        }

        CurrentUser.Cart.Clear();
        UserCartChanged.Invoke();
    }
    #endregion
}
