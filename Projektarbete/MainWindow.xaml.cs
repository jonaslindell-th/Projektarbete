﻿using System;
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
        //private List<Product> productList = Product.DeserializeProducts();
        private List<Product> productList = Product.DeserializeProducts();
        private List<Product> shoppingCart = new List<Product>();
        private List<Product> searchTermList = new List<Product>();
        private static List<string> categoryList = new List<string>();

        private ListBox cartListBox;
        private ListBox productListBox;

        private TextBlock sumTextBlock;
        private TextBlock productHeading;
        private TextBlock productDescriptionHeading;
        private TextBlock productDescription;

        private TextBox searchBox;

        private Image currentImage;

        Grid imageGrid;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "Butiken";
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
            //mainGrid.Margin = new Thickness(5);
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());// First column contains a separate left grid, the left grid displays products added to the shoppingCart list using ListBox. The left grid also contains buttons and TextBlocks related to the shopping cart.
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());// Second column contains the middle grid which displays all available products in a ListBox with a related TextBlock and a add to cart button
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());// Third column contains the right grid which displays the current selected product image, description and heading using SelectionChanged from the ListBox in the middle grid
            Uri iconUri = new Uri("Images/Ica.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);// Changes the window icon

            // #### custom brushes ####
            var brushConverter = new System.Windows.Media.BrushConverter();
            var backgroundBrush = (Brush)brushConverter.ConvertFromString("#2F3136");
            mainGrid.Background = backgroundBrush;

            var listBoxBrush = (Brush)brushConverter.ConvertFromString("#36393F");

            var textBoxBrush = (Brush)brushConverter.ConvertFromString("#40444B");

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

            TextBlock cartTextBlock = CreateTextBlock("Varukorg", 18, TextAlignment.Center, leftGrid, 0, 0, 2);
            TextBlock discountTextBlock = CreateTextBlock("Mata in rabattkod nedan", 12, TextAlignment.Center, leftGrid, 1, 0, 2);

            TextBox discountTextBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            leftGrid.Children.Add(discountTextBox);
            Grid.SetRow(discountTextBox, 2);
            Grid.SetColumnSpan(discountTextBox, 1);

            CreateButton("Använd rabattkod", leftGrid, 2, 2, 1, ValidateCoupon);
            CreateButton("Rensa varukorg", leftGrid, row: 3, column: 0, columnspan: 2, ClearCartClick);
            CreateButton("Ta bort vald produkt", leftGrid, row: 4, column: 0, columnspan: 2, RemoveProductClick);

            cartListBox = new ListBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MaxHeight = 370,
                Background = listBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            leftGrid.Children.Add(cartListBox);
            Grid.SetRow(cartListBox, 5);
            Grid.SetColumnSpan(cartListBox, 2);

            sumTextBlock = CreateTextBlock("Varukorgens summa: 0 kr", 12, TextAlignment.Left, leftGrid, 6, 0, 2);

            // #### End of left grid definition ####

            // ##### Middle grid definition ####
            Grid middleGrid = new Grid();
            mainGrid.Children.Add(middleGrid);
            middleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            middleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            middleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            middleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            middleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            middleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            middleGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(middleGrid, 0);
            Grid.SetColumn(middleGrid, 1);

            TextBlock products = CreateTextBlock("Produkter", 18, TextAlignment.Center, middleGrid, 0, 0, 1);

            TextBlock searchHeading = CreateTextBlock("Sök efter produkt", 12, TextAlignment.Center, middleGrid, 1, 0, 1);

            searchBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            middleGrid.Children.Add(searchBox);
            Grid.SetRow(searchBox, 2);
            searchBox.TextChanged += ShowSearch;

            ComboBox categoryBox = new ComboBox
            {
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Height = 21,
            };
            middleGrid.Children.Add(categoryBox);
            Grid.SetRow(categoryBox, 3);
            categoryBox.Items.Add("Välj kategori");
            categoryBox.SelectedIndex = 0;
            foreach(string category in categoryList)
            {
                categoryBox.Items.Add(category);
            }
            categoryBox.SelectionChanged += ShowCategory;

            CreateButton("Lägg till vald produkt", middleGrid, 4, 0, 1, AddProductToCart);

            productListBox = new ListBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MaxHeight = 425,
                Background = listBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            middleGrid.Children.Add(productListBox);
            Grid.SetRow(productListBox, 5);
            Grid.SetColumn(productListBox, 0);
            Grid.SetColumnSpan(productListBox, 2);
            UpdateProductListBox("");
            productListBox.SelectionChanged += DisplaySelectedProduct;
            // #### End of middle grid definition ####

            // ##### Right grid definition ####
            StackPanel rightGrid = new StackPanel();
            mainGrid.Children.Add(rightGrid);
            Grid.SetRow(rightGrid, 0);
            Grid.SetColumn(rightGrid, 2);

            productHeading = CreateTextBlock("Välj produkt", 18, TextAlignment.Center, rightGrid);

            imageGrid = new Grid();
            rightGrid.Children.Add(imageGrid);

            currentImage = CreateImage("Images/Ica.png");
            //currentImage.Stretch = Stretch.Uniform;
            imageGrid.Children.Add(currentImage);

            productDescriptionHeading = CreateTextBlock("", 16, TextAlignment.Center, rightGrid);
            productDescription = CreateTextBlock("", 12, TextAlignment.Left, rightGrid);
            // ##### End of right grid definition ####
        }

        private void ShowCategory(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ShowSearch(object sender, TextChangedEventArgs e)
        {
            string searchTerm = searchBox.Text;
            UpdateProductListBox(searchTerm);
        }

        private void ValidateCoupon(object sender, RoutedEventArgs e)
        {

        }

        private void CreateButton(string content, Grid grid, int row, int column, int columnspan, RoutedEventHandler onClick)
        {
            Button button = new Button
            {
                Content = content,
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            grid.Children.Add(button);
            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            Grid.SetColumnSpan(button, columnspan);
            button.Click += onClick;
        }

        private TextBlock CreateTextBlock(string text, int fontSize, TextAlignment alignment, Grid grid, int row, int column, int columnSpan)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = fontSize,
                TextAlignment = alignment,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
            };
            grid.Children.Add(textBlock);
            Grid.SetRow(textBlock, row);
            Grid.SetColumn(textBlock, column);
            Grid.SetColumnSpan(textBlock, columnSpan);
            return textBlock;
        }

        private TextBlock CreateTextBlock(string text, int fontSize, TextAlignment alignment, StackPanel grid)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Arial"),
                FontSize = fontSize,
                TextAlignment = alignment,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
            };
            grid.Children.Add(textBlock);
            return textBlock;
        }

        private void DisplaySelectedProduct(object sender, SelectionChangedEventArgs e)
        {
            if (productListBox.SelectedIndex != -1)
            {
                productHeading.Text = searchTermList[productListBox.SelectedIndex].Title;
                imageGrid.Children.Clear();
                currentImage = CreateImage("Images/" + searchTermList[productListBox.SelectedIndex].ProductImage);
                imageGrid.Children.Add(currentImage);
                productDescriptionHeading.Text = "Produktbeskrivning";
                productDescription.Text = searchTermList[productListBox.SelectedIndex].Description;
            }
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
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            return image;
        }

        private void RemoveProductClick(object sender, RoutedEventArgs e)
        {
            if (cartListBox.SelectedIndex != -1 && shoppingCart[cartListBox.SelectedIndex].Count > 1)
            {
                shoppingCart[cartListBox.SelectedIndex].Count--;
                UpdateShoppingCart();
                return;
            }
            else if(cartListBox.SelectedIndex != -1 && shoppingCart[cartListBox.SelectedIndex].Count == 1)
            {
                shoppingCart.RemoveAt(cartListBox.SelectedIndex);
                UpdateShoppingCart();
                return;
            }
            MessageBox.Show("Välj en produkt att ta bort");
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
                if (shoppingCart.Contains(searchTermList[productListBox.SelectedIndex]))
                {
                    int index = shoppingCart.IndexOf(searchTermList[productListBox.SelectedIndex]);
                    shoppingCart[index].Count += 1;
                }
                else
                {
                    shoppingCart.Add(searchTermList[productListBox.SelectedIndex]);
                    int index = shoppingCart.IndexOf(searchTermList[productListBox.SelectedIndex]);
                    shoppingCart[index].Count = 1;
                }
                UpdateShoppingCart();
                return;
            }
            MessageBox.Show("Ingen produkt vald");
        }

        private void UpdateShoppingCart()
        {
            cartListBox.Items.Clear();
            foreach (Product product in shoppingCart)
            {
                cartListBox.Items.Add(product.Title + " (" + product.Price + ") kr" + " (" + product.Count + ")");
            }
            decimal sum = 0;
            foreach (Product product in shoppingCart)
            {
                sum += product.Price * product.Count;
            }
            sumTextBlock.Text = "Varukorgens summa: " + (Convert.ToString(Math.Round(sum, 1))) + " kr";
        }

        private void UpdateProductListBox(string searchTerm)
        {
            searchTermList.Clear();
            if (searchTerm != "")
            {
                var searchTermProducts = productList.Where(product => product.Title.Contains(searchTerm));
                foreach (Product product in searchTermProducts)
                {
                    searchTermList.Add(product);
                }
                productListBox.Items.Clear();
                foreach (Product product in searchTermProducts)
                {
                    productListBox.Items.Add(product.Title + " (" + product.Price + ") kr");
                }
            }
            else
            {
                foreach (Product product in productList)
                {
                    searchTermList.Add(product);
                }
                productListBox.Items.Clear();
                foreach (Product product in searchTermList)
                {
                    productListBox.Items.Add(product.Title + " (" + product.Price + ") kr");
                }
            }
        }

        private static List<Product> GenerateProducts()
        {
            List<Product> generateProducts = new List<Product>();
            var csvFile = File.ReadAllLines("Products.csv", Encoding.GetEncoding("iso-8859-1"));

            foreach (var productLine in csvFile)
            {
                string[] productInformation = productLine.Split(";");
                Product product = new Product
                {
                    Title = productInformation[0],
                    Category = productInformation[1],
                    Description = productInformation[2],
                    Price = decimal.Parse(productInformation[3]),
                    ProductImage = productInformation[4]
                };
                generateProducts.Add(product);
                if (!categoryList.Contains(product.Category))
                {
                    categoryList.Add(product.Category);
                }
            }
            return generateProducts;
        }
    }
}