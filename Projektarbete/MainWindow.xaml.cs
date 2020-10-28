using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Projektarbete
{
    public partial class MainWindow : Window
    {
        private List<Product> productList = GenerateProducts();
        private List<Product> shoppingCart = new List<Product>();

        private ListBox cartListBox;
        private ListBox productListBox;

        private TextBlock sumTextBlock;
        private TextBlock productHeading;
        private TextBlock productDescription;

        private Image uniformImage;

        Grid imageGrid;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "Marketstore";
            Width = 900;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid definition
            Grid mainGrid = new Grid();
            root.Content = mainGrid;
            mainGrid.Margin = new Thickness(5);
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());// First column displays the products added to the shoppingCart list using ListBox and contains related buttons and TextBlocks
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());// Second column displays all available products in a ListBox
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());// Third column displays product the product image, description and changes the heading to the title of the selected product using SelectionChanged in the ListBox in the second column
            Uri iconUri = new Uri("Images/Windowicon.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);// Changes the window icon

            // ##### Left grid definition #####
            Grid leftGrid = new Grid();
            mainGrid.Children.Add(leftGrid);
            leftGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            leftGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            leftGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            leftGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            leftGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            leftGrid.RowDefinitions.Add(new RowDefinition());
            leftGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            leftGrid.ColumnDefinitions.Add(new ColumnDefinition());
            leftGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetRow(leftGrid, 0);
            Grid.SetColumn(leftGrid, 0);

            TextBlock cartTextBlock = new TextBlock
            {
                Text = "Your shopping cart",
                Margin = new Thickness(5),
                //FontFamily = new FontFamily("Arial"),
                FontSize = 18,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            leftGrid.Children.Add(cartTextBlock);
            Grid.SetRow(cartTextBlock, 0);
            //Grid.SetColumn(cartTextBlock, 0);
            Grid.SetColumnSpan(cartTextBlock, 2);

            TextBlock discountTextBlock = new TextBlock
            {
                Text = "Enter discount coupon below",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 12,
                Margin = new Thickness(5)
            };
            leftGrid.Children.Add(discountTextBlock);
            Grid.SetRow(discountTextBlock, 1);
            //Grid.SetColumn(discountTextBlock, 0);
            Grid.SetColumnSpan(discountTextBlock, 2);

            TextBox discountTextBox = new TextBox
            {
                Margin = new Thickness(5)
            };
            leftGrid.Children.Add(discountTextBox);
            Grid.SetRow(discountTextBox, 2);
            //Grid.SetColumn(discountTextBox, 0);
            Grid.SetColumnSpan(discountTextBox, 2);

            Button clearCart = new Button
            {
                Content = "Clear shopping cart",
                Margin = new Thickness(5),
                Background = Brushes.PaleVioletRed
            };
            leftGrid.Children.Add(clearCart);
            Grid.SetRow(clearCart, 3);
            //Grid.SetColumn(clearCart, 0);
            Grid.SetColumnSpan(clearCart, 2);
            clearCart.Click += ClearCartClick;

            Button removeProduct = new Button
            {
                Content = "Remove selected product",
                Margin = new Thickness(5),
                Background = Brushes.PaleVioletRed
            };
            leftGrid.Children.Add(removeProduct);
            Grid.SetRow(removeProduct, 4);
            //Grid.SetColumn(removeProduct, 0);
            Grid.SetColumnSpan(removeProduct, 2);
            removeProduct.Click += RemoveProductClick;

            cartListBox = new ListBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MaxHeight = 380
            };
            leftGrid.Children.Add(cartListBox);
            Grid.SetRow(cartListBox, 5);
            //Grid.SetColumn(cartListBox, 0);
            Grid.SetColumnSpan(cartListBox, 2);

            sumTextBlock = new TextBlock
            {
                Text = "Shopping cart sum: 0 kr",
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Bottom
            };
            leftGrid.Children.Add(sumTextBlock);
            Grid.SetRow(sumTextBlock, 6);
            //Grid.SetColumn(sumTextBlock, 0);
            Grid.SetColumnSpan(sumTextBlock, 2);

            // #### End of left grid definition ####

            // ##### Middle grid definition ####
            Grid middleGrid = new Grid();
            mainGrid.Children.Add(middleGrid);
            middleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            middleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            middleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            middleGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(middleGrid, 0);
            Grid.SetColumn(middleGrid, 1);

            TextBlock products = new TextBlock
            {
                Text = "Products",
                Margin = new Thickness(5),
                //FontFamily = new FontFamily("Arial"),
                FontSize = 18,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            middleGrid.Children.Add(products);
            Grid.SetRow(products, 0);
            //Grid.SetColumn(products, 0);
            Grid.SetColumnSpan(products, 2);

            Button addToCartButton = new Button
            {
                Content = "Add selected product to cart",
                Margin = new Thickness(5),
                Background = Brushes.LightGreen
            };
            middleGrid.Children.Add(addToCartButton);
            Grid.SetRow(addToCartButton, 1);
            //Grid.SetColumn(addToCartButton, 0);
            addToCartButton.Click += AddProductToCart;

            productListBox = new ListBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            middleGrid.Children.Add(productListBox);
            Grid.SetRow(productListBox, 2);
            Grid.SetColumn(productListBox, 0);
            Grid.SetColumnSpan(productListBox, 2);
            DisplayProductsInListBox();
            productListBox.SelectionChanged += DisplaySelectedProduct;

            // #### End of middle grid definition ####

            // ##### Right grid definition ####
            Grid rightGrid = new Grid();
            mainGrid.Children.Add(rightGrid);
            rightGrid.ColumnDefinitions.Add(new ColumnDefinition());
            rightGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rightGrid.RowDefinitions.Add(new RowDefinition());
            rightGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(rightGrid, 0);
            Grid.SetColumn(rightGrid, 2);

            productHeading = new TextBlock
            {
                Text = "Select product",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Arial"),
                FontSize = 18,
                TextAlignment = TextAlignment.Center
            };
            rightGrid.Children.Add(productHeading);
            Grid.SetRow(productHeading, 0);
            Grid.SetColumn(productHeading, 0);

            imageGrid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Top,
                MaxHeight = 600,
                MaxWidth = 600,
                Margin = new Thickness(5)
            };
            rightGrid.Children.Add(imageGrid);
            imageGrid.RowDefinitions.Add(new RowDefinition());
            imageGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetRow(imageGrid, 1);
            Grid.SetColumn(imageGrid, 0);

            uniformImage = CreateImage("Images/Store.jpg");
            uniformImage.Stretch = Stretch.Uniform;
            imageGrid.Children.Add(uniformImage);
            Grid.SetRow(uniformImage, 0);
            Grid.SetColumn(uniformImage, 0);

            productDescription = new TextBlock
            {
                Text = "Product description: ",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                //FontFamily = new FontFamily("Arial"),
                FontSize = 14,
                TextAlignment = TextAlignment.Left
            };
            rightGrid.Children.Add(productDescription);
            Grid.SetRow(productDescription, 2);
            //Grid.SetColumn(productDescription, 0);

            // ##### End of right grid definition ####
        }

        private void DisplaySelectedProduct(object sender, SelectionChangedEventArgs e)
        {
            int index = productListBox.SelectedIndex;
            productHeading.Text = productList[index].Title;

            uniformImage = CreateImage("Images/" + productList[index].ProductImage);
            imageGrid.Children.Clear();
            imageGrid.Children.Add(uniformImage);

            productDescription.Text = "Product description: " + productList[index].Description;
        }

        private Image CreateImage(string filePath)
        {
            ImageSource source = new BitmapImage(new Uri(filePath, UriKind.Relative));
            Image image = new Image
            {
                Source = source,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            // A small rendering tweak to ensure maximum visual appeal.
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            return image;
        }

        private void RemoveProductClick(object sender, RoutedEventArgs e)
        {
            if (cartListBox.SelectedIndex != -1)
            {
                shoppingCart.RemoveAt(cartListBox.SelectedIndex);
                UpdateShoppingCart();
                return;
            }
            MessageBox.Show("Select a product to remove");
        }

        private void ClearCartClick(object sender, RoutedEventArgs e)
        {
            shoppingCart.Clear();
            UpdateShoppingCart();
        }

        private void AddProductToCart(object sender, RoutedEventArgs e)
        {
            if (productListBox.SelectedIndex != -1)
            {
                shoppingCart.Add(productList[productListBox.SelectedIndex]);
                UpdateShoppingCart();
                return;
            }
            MessageBox.Show("No product selected");
        }

        private void UpdateShoppingCart()
        {
            cartListBox.Items.Clear();
            foreach (Product product in shoppingCart)
            {
                cartListBox.Items.Add(product.Title);
            }
            decimal sum = 0;
            foreach (Product product in shoppingCart)
            {
                sum += product.Price;
            }
            sumTextBlock.Text = "Shopping cart sum: " + Convert.ToString(sum) + " kr";
        }

        private void DisplayProductsInListBox()
        {
            foreach (Product product in productList)
            {
                productListBox.Items.Add(product.Title + " (" + product.Price + ")kr");
            }
        }

        private static List<Product> GenerateProducts()
        {
            List<Product> generateProducts = new List<Product>();
            //var csvFile = File.ReadAllLines("C:/Windows/Temp/Projectarbete/Products.csv");
            var csvFile = File.ReadAllLines("Products.csv");

            foreach (var productLine in csvFile)
            {
                string[] productInformation = productLine.Split(";");
                Product product = new Product
                {
                    Title = productInformation[0],
                    Description = productInformation[1],
                    Price = decimal.Parse(productInformation[2]),
                    ProductImage = productInformation[3]
                };
                generateProducts.Add(product);
            }
            return generateProducts;
        }
    }
}
