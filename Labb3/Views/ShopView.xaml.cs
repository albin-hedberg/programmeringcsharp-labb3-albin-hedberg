using System;
using System.Linq;
using System.Windows.Controls;
using Labb3ProgTemplate.DataModels.Products;
using Labb3ProgTemplate.Enums;
using Labb3ProgTemplate.Managers;

namespace Labb3ProgTemplate.Views
{
    /// <summary>
    /// Interaction logic for ShopView.xaml
    /// </summary>
    public partial class ShopView : UserControl
    {
        public ShopView()
        {
            InitializeComponent();

            UserManager.CurrentUserChanged += UserManager_CurrentUserChanged;
            UserManager.UserCartChanged += UserManager_UserCartChanged;
            ProductManager.ProductListChanged += ProductManager_ProductListChanged;
            CurrencyManager.CurrentCurrencyChanged += CurrencyManager_CurrentCurrencyChanged;

            PopulateSortingList();
            PopulateCurrencyList();
            PopulateProductsList();
        }

        private void PopulateSortingList()
        {
            SortProductList.Items.Add("All");

            foreach (var productType in Enum.GetValues(typeof(ProductTypes)))
            {
                SortProductList.Items.Add(productType);
            }

            SortProductList.SelectedIndex = 0;
        }

        private void PopulateCurrencyList()
        {
            foreach (var currency in CurrencyManager.Currencies.Keys)
            {
                CurrencyList.Items.Add(currency);
            }

            CurrencyList.SelectedIndex = 0;
        }

        private void PopulateProductsList()
        {
            foreach (var product in ProductManager.Products.OrderBy(p => p.Name))
            {
                ProdList.Items.Add(product);
            }
        }

        private void PopulateCartList()
        {
            foreach (var product in UserManager.CurrentUser.Cart)
            {
                CartList.Items.Add(product);
            }
        }

        private void UserManager_CurrentUserChanged()
        {
            if (UserManager.CurrentUser.LoggedIn == false)
            {
                return;
            }

            UserName.Text = UserManager.CurrentUser.Name;

            SortProductList.SelectedIndex = 0;
            CurrencyList.SelectedIndex = 0;

            PopulateCartList();
            UpdateProductList();
            UpdateCartList();
        }

        private void UserManager_UserCartChanged()
        {
            UpdateCartList();
        }

        private void ProductManager_ProductListChanged()
        {
            UpdateProductList();
        }

        private void CurrencyManager_CurrentCurrencyChanged()
        {
            UpdateProductList();
        }

        private void CurrencyList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrencyManager.UpdateAllProductPricesToCurrentCurrency(CurrencyList.SelectedItem.ToString());
        }

        private void UpdateProductList()
        {
            ProdList.Items.Clear();

            if (SortProductList.SelectedIndex == 0)
            {
                PopulateProductsList();
                return;
            }

            var productType = Enum.Parse<ProductTypes>(SortProductList.SelectedItem.ToString());

            foreach (var product in ProductManager.Products.Where(p => p.Type == productType).OrderBy(p => p.Name))
            {
                ProdList.Items.Add(product);
            }
        }

        private void UpdateCartList()
        {
            UpdateTotalPrice();
            CartList.Items.Clear();
            PopulateCartList();
        }

        private void UpdateTotalPrice()
        {
            TotalPrice.Content = $"{UserManager.CartTotal()} {CurrencyManager.CurrentCurrency}";
        }

        private void RemoveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CartList.SelectedItem is Product selectedItem)
            {
                UserManager.RemoveFromCart(selectedItem);
            }
        }

        private void AddBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ProdList.SelectedItem is Product selectedItem)
            {
                UserManager.AddToCart(selectedItem);
            }
        }

        private void LogoutBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserManager.LogOut();
        }

        private void CheckoutBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserManager.Checkout();
        }

        private void SortProductList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProductList();
        }
    }
}
