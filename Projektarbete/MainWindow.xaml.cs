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
        //private List<Product> productList = Product.DeserializeProducts();
        private List<Product> productList = GenerateProducts();
        private List<Product> shoppingCart = new List<Product>();

        private ListBox cartListBox;
        private ListBox productListBox;

        private TextBlock sumTextBlock;
        private TextBlock productHeading;
        private TextBlock productDescription;

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
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());// First column contains a separate left grid, the left grid displays products added to the shoppingCart list using ListBox. The left grid also contains buttons and TextBlocks related to the shopping cart.
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());// Second column contains the middle grid which displays all available products in a ListBox with a related TextBlock and a add to cart button
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());// Third column contains the right grid which displays the current selected product image, description and heading using SelectionChanged from the ListBox in the middle grid
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

            TextBlock cartTextBlock = CreateTextBlock("Your shopping cart", 18, TextAlignment.Center, leftGrid, 0, 0, 2);
            TextBlock discountTextBlock = CreateTextBlock("Enter discount code below", 12, TextAlignment.Center, leftGrid, 1, 0, 2);

            TextBox discountTextBox = new TextBox { Margin = new Thickness(5) };
            leftGrid.Children.Add(discountTextBox);
            Grid.SetRow(discountTextBox, 2);
            Grid.SetColumnSpan(discountTextBox, 1);

            CreateButton("Validate coupon", leftGrid, 2, 2, 1, ValidateCoupon);

            CreateButton("Clear shopping cart", Brushes.PaleVioletRed, leftGrid, row: 3, column: 0, columnspan: 2, ClearCartClick);
            CreateButton("Remove selected product", Brushes.PaleVioletRed, leftGrid, row: 4, column: 0, columnspan: 2, RemoveProductClick);

            cartListBox = new ListBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MaxHeight = 370
            };
            leftGrid.Children.Add(cartListBox);
            Grid.SetRow(cartListBox, 5);
            Grid.SetColumnSpan(cartListBox, 2);

            sumTextBlock = new TextBlock
            {
                Text = "Shopping cart sum: 0 kr",
                Margin = new Thickness(5),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            leftGrid.Children.Add(sumTextBlock);
            Grid.SetRow(sumTextBlock, 6);
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

            TextBlock products = CreateTextBlock("Products", 18, TextAlignment.Center, middleGrid, 0, 0, 2);

            CreateButton("Add selected product to cart", Brushes.LightGreen, middleGrid, 1, 0, 1, AddProductToCart);

            productListBox = new ListBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MaxHeight = 425
            };
            middleGrid.Children.Add(productListBox);
            Grid.SetRow(productListBox, 2);
            Grid.SetColumn(productListBox, 0);
            Grid.SetColumnSpan(productListBox, 2);
            DisplayProductsInListBox();
            productListBox.SelectionChanged += DisplaySelectedProduct;
            // #### End of middle grid definition ####

            // ##### Right grid definition ####
            StackPanel rightGrid = new StackPanel();
            mainGrid.Children.Add(rightGrid);
            Grid.SetRow(rightGrid, 0);
            Grid.SetColumn(rightGrid, 2);

            productHeading = CreateTextBlock("Select product", 18, TextAlignment.Center, rightGrid);

            imageGrid = new Grid();
            rightGrid.Children.Add(imageGrid);
            imageGrid.RowDefinitions.Add(new RowDefinition());
            imageGrid.ColumnDefinitions.Add(new ColumnDefinition());

            currentImage = CreateImage("Images/Store.jpg");
            currentImage.Stretch = Stretch.Uniform;
            imageGrid.Children.Add(currentImage);
            Grid.SetRow(currentImage, 0);
            Grid.SetColumn(currentImage, 0);

            productDescription = CreateTextBlock("Product description: ", 14, TextAlignment.Left, rightGrid);
            // ##### End of right grid definition ####
        }

        private void ValidateCoupon(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Creates and adds a button based on passed parameters
        /// </summary>
        /// <param name="content"></param>
        /// <param name="color"></param>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="columnspan"></param>
        /// <param name="onClick"></param>
        private void CreateButton(string content, Brush color, Grid grid, int row, int column, int columnspan, RoutedEventHandler onClick)
        {
            Button button = new Button
            {
                Content = content,
                Margin = new Thickness(5),
                Background = color
            };
            grid.Children.Add(button);
            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            Grid.SetColumnSpan(button, columnspan);
            button.Click += onClick;
        }

        private void CreateButton(string content, Grid grid, int row, int column, int columnspan, RoutedEventHandler onClick)
        {
            Button button = new Button
            {
                Content = content,
                Margin = new Thickness(5)
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
                FontFamily = new FontFamily("Arial"),
                FontSize = fontSize,
                TextAlignment = alignment
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
                TextAlignment = alignment
            };
            grid.Children.Add(textBlock);
            return textBlock;
        }

        private void DisplaySelectedProduct(object sender, SelectionChangedEventArgs e)
        {
            int index = productListBox.SelectedIndex;
            productHeading.Text = productList[index].Title;

            currentImage = CreateImage("Images/" + productList[index].ProductImage);
            imageGrid.Children.Clear();
            imageGrid.Children.Add(currentImage);

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
                if (shoppingCart.Contains(productList[productListBox.SelectedIndex]))
                {
                    int index = shoppingCart.IndexOf(productList[productListBox.SelectedIndex]);
                    shoppingCart[index].Count += 1;
                }
                else
                {
                    shoppingCart.Add(productList[productListBox.SelectedIndex]);
                    int index = shoppingCart.IndexOf(productList[productListBox.SelectedIndex]);
                    shoppingCart[index].Count = 1;
                }
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
                cartListBox.Items.Add(product.Title + " (" + product.Price + ") kr" + " (" + product.Count + ")");
            }
            decimal sum = 0;
            foreach (Product product in shoppingCart)
            {
                sum += product.Price * product.Count;
            }
            sumTextBlock.Text = "Shopping cart sum: " + Convert.ToString(sum) + " kr";
        }

        private void DisplayProductsInListBox()
        {
            foreach (Product product in productList)
            {
                productListBox.Items.Add(product.Title + " (" + product.Price + ") kr");
            }
        }

        private static List<Product> GenerateProducts()
        {
            List<Product> generateProducts = new List<Product>();
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