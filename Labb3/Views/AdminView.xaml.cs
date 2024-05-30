using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Labb3ProgTemplate.DataModels.Products;
using Labb3ProgTemplate.Enums;
using Labb3ProgTemplate.Managers;

namespace Labb3ProgTemplate.Views
{
    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView : UserControl
    {
        public AdminView()
        {
            InitializeComponent();

            UserManager.CurrentUserChanged += UserManager_CurrentUserChanged;
            ProductManager.ProductListChanged += ProductManager_ProductListChanged;
            CurrencyManager.CurrentCurrencyChanged += CurrencyManager_CurrentCurrencyChanged;

            PopulateSortingList();
            PopulateCurrencyList();
            PopulateProductsList();
            PopulateProductTypeList();
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

        private void PopulateProductTypeList()
        {
            ProdType.ItemsSource = Enum.GetValues(typeof(ProductTypes));
        }

        private void UserManager_CurrentUserChanged()
        {
            SortProductList.SelectedIndex = 0;
            CurrencyList.SelectedIndex = 0;
            ErrorMessage.Text = string.Empty;

            UpdateProductList();
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
            ProdPriceCurrency.Content = CurrencyManager.CurrentCurrency;
            ProdList.Items.Clear();
            PopulateProductsList();
        }

        private void ProdList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProdList.SelectedItem is Product selectedItem)
            {
                ProdName.Text = selectedItem.Name;
                ProdPrice.Text = selectedItem.Price.ToString();
                ProdImgUrl.Text = selectedItem.ImageUrl;
                ProdType.SelectedItem = selectedItem.Type;
                ProdType.Visibility = Visibility.Hidden;
                ProdTypeLabel.Visibility = Visibility.Hidden;
                ErrorMessage.Text = "";
            }
            else
            {
                ProdName.Text = "";
                ProdPrice.Text = "";
                ProdImgUrl.Text = "";
                ProdType.SelectedItem = null;
                ProdType.Visibility = Visibility.Visible;
                ProdTypeLabel.Visibility = Visibility.Visible;
                ErrorMessage.Text = "";
            }
        }

        private bool CheckUserInputForErrors()
        {
            if (ProdName.Text.Length < 1 || ProdPrice.Text.Length < 1)
            {
                ErrorMessage.Text = "All fields aren't filled in.";
                return true;
            }

            if (!double.TryParse(ProdPrice.Text, out _))
            {
                ErrorMessage.Text = "Price isn't a valid number.";
                return true;
            }

            if (ProdType.SelectedItem is null)
            {
                ErrorMessage.Text = "You must choose a product type.";
                return true;
            }

            return false;
        }

        private void SaveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CheckUserInputForErrors())
            {
                return;
            }

            if (ProdList.SelectedItem is Product selectedProduct)
            {
                selectedProduct.Name = ProdName.Text;
                selectedProduct.Price = double.Parse(ProdPrice.Text);
                selectedProduct.ImageUrl = ProdImgUrl.Text;

                ProductManager.UpdateProduct(selectedProduct);

                return;
            }

            var price = double.Parse(ProdPrice.Text);

            switch (ProdType.SelectedItem)
            {
                case ProductTypes.Food:
                    ProductManager.AddProduct(new Food(ProdName.Text, CurrencyManager.ConvertPriceToSEK(price), 1, ProdImgUrl.Text));
                    break;
                case ProductTypes.Drink:
                    ProductManager.AddProduct(new Drink(ProdName.Text, CurrencyManager.ConvertPriceToSEK(price), 1, ProdImgUrl.Text));
                    break;
            }
        }

        private void RemoveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ProdList.SelectedItem is Product selectedItem)
            {
                ProductManager.RemoveProduct(selectedItem);
            }
        }

        private void LogoutBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserManager.LogOut();
        }

        private void SortProductList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
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
    }
}
