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
using System.Diagnostics;

namespace Projektarbete
{
    public partial class MainWindow : Window
    {
        private List<Product> productList = Product.DeserializeProducts();
        private List<Product> shoppingCart = new List<Product>();
        private List<Product> searchTermList = new List<Product>();
        private List<string> categoryList = new List<string>();
        private List<Coupon> couponList = Coupon.CouponCodes();

        private ListBox cartListBox;
        private ListBox productListBox;

        ComboBox categoryBox;
        ComboBox couponComboBox;

        private TextBlock sumTextBlock;
        private TextBlock productHeading;
        private TextBlock productDescriptionHeading;
        private TextBlock productDescription;

        private TextBox searchBox;
        private TextBox couponTextBox;

        Expander cartExpander;

        private Image currentImage;

        private bool hasDiscount;
        private Coupon currentCoupon;

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
            Width = 800;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid definition
            Grid mainGrid = new Grid();
            root.Content = mainGrid;
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());// First column contains the shoppingcart grid inside an expander on the first row, the remaining rows displays all available products in a listbox with related TextBlock and add to cart button
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());// The second column contains the StackPanel which displays the current selected product image, description and heading using SelectionChanged from the ListBox in the first grid
            Uri iconUri = new Uri("Images/Ica.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);// Changes the window icon

            // #### custom brushes ####
            var brushConverter = new System.Windows.Media.BrushConverter();
            var backgroundBrush = (Brush)brushConverter.ConvertFromString("#2F3136");
            mainGrid.Background = backgroundBrush;
            var listBoxBrush = (Brush)brushConverter.ConvertFromString("#36393F");
            var textBoxBrush = (Brush)brushConverter.ConvertFromString("#40444B");
            var expanderBrush = (Brush)brushConverter.ConvertFromString("#202225");

            // ##### expanderCartGrid definition #####
            Grid expanderCartGrid = new Grid();
            expanderCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            expanderCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            expanderCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            expanderCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            expanderCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            expanderCartGrid.RowDefinitions.Add(new RowDefinition());
            expanderCartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            expanderCartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            expanderCartGrid.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock cartTextBlock = CreateTextBlock("Varukorg", 18, TextAlignment.Center, expanderCartGrid, 0, 0, 2);
            TextBlock discountTextBlock = CreateTextBlock("Mata in rabattkod nedan", 12, TextAlignment.Center, expanderCartGrid, 1, 0, 1);

            couponComboBox = new ComboBox
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Height = 18,
                Margin = new Thickness(0, 5, 5, 5),
                BorderThickness = new Thickness(0),
                VerticalContentAlignment = VerticalAlignment.Top,
                Padding = new Thickness(5, 1, 5, 0),
            };
            expanderCartGrid.Children.Add(couponComboBox);
            Grid.SetRow(couponComboBox, 1);
            Grid.SetColumn(couponComboBox, 1);
            couponComboBox.Items.Add("Dina rabattkoder");
            couponComboBox.SelectedIndex = 0;
            couponComboBox.SelectionChanged += AddToCouponTextBox;
            foreach (var coupon in couponList)
            {
                couponComboBox.Items.Add(coupon.Code + " " +  (100 - Math.Round(coupon.Discount * 100, 0)) + "%");
            }

            couponTextBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            expanderCartGrid.Children.Add(couponTextBox);
            Grid.SetRow(couponTextBox, 2);
            Grid.SetColumnSpan(couponTextBox, 1);

            CreateButton("Använd rabattkod", expanderCartGrid, 2, 2, 1, ValidateCoupon);
            CreateButton("Rensa varukorg", expanderCartGrid, row: 3, column: 0, columnspan: 2, ClearCartClick);
            CreateButton("Ta bort en vald produkt", expanderCartGrid, row: 4, column: 0, columnspan: 1, RemoveProductClick);
            CreateButton("Ta bort varje vald produkt", expanderCartGrid, 4, 1, 1, RemoveAllProductsClick);

            cartListBox = new ListBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MaxHeight = 200,
                MinHeight = 200,
                Background = listBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            expanderCartGrid.Children.Add(cartListBox);
            Grid.SetRow(cartListBox, 5);
            Grid.SetColumnSpan(cartListBox, 2);

            sumTextBlock = CreateTextBlock("Varukorgens summa: 0 kr", 12, TextAlignment.Left, expanderCartGrid, 6, 0, 2);

            // #### End of expanderCartGrid definition ####

            // ##### left Grid definition ####
            Grid leftGrid = new Grid();
            mainGrid.Children.Add(leftGrid);
            leftGrid.ColumnDefinitions.Add(new ColumnDefinition());
            leftGrid.ColumnDefinitions.Add(new ColumnDefinition());
            leftGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            leftGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            leftGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            leftGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            leftGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            leftGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            leftGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(leftGrid, 0);
            Grid.SetColumn(leftGrid, 0);

            cartExpander = new Expander
            {
                Content = expanderCartGrid,
                Header = "Din varukorg 0 kr",
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.White,
                Background = expanderBrush
            };
            leftGrid.Children.Add(cartExpander);
            Grid.SetRow(cartExpander, 0);
            Grid.SetColumn(cartExpander, 0);
            cartExpander.Collapsed += DecreaseCartRowSpan;
            cartExpander.Expanded += IncreaseCartRowSpan;

            TextBlock products = CreateTextBlock("Produkter", 18, TextAlignment.Center, leftGrid, 1, 0, 2);

            TextBlock searchHeading = CreateTextBlock("Sök efter produkt", 12, TextAlignment.Center, leftGrid, 2, 0, 2);

            searchBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            leftGrid.Children.Add(searchBox);
            Grid.SetRow(searchBox, 3);
            Grid.SetColumnSpan(searchBox, 2);
            searchBox.TextChanged += ShowSearch;

            categoryBox = new ComboBox
            {
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Height = 22,
            };
            leftGrid.Children.Add(categoryBox);
            Grid.SetRow(categoryBox, 4);
            Grid.SetColumn(categoryBox, 0);
            Grid.SetColumnSpan(categoryBox, 2);
            categoryBox.Items.Add("Välj kategori");
            categoryBox.SelectedIndex = 0;
            GenerateCategories();
            foreach (string category in categoryList)
            {
                categoryBox.Items.Add(category);
            }
            categoryBox.SelectionChanged += ShowCategory;

            CreateButton("Lägg till vald produkt", leftGrid, row:5, column:0, columnspan:2, AddProductToCart);

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
            leftGrid.Children.Add(productListBox);
            Grid.SetRow(productListBox, 6);
            Grid.SetColumn(productListBox, 0);
            Grid.SetColumnSpan(productListBox, 2);
            UpdateProductListBox("");
            productListBox.SelectionChanged += DisplaySelectedProduct;
            // #### End of left Grid definition ####

            // ##### Right StackPanel definition ####
            StackPanel rightStackPanel = new StackPanel();
            mainGrid.Children.Add(rightStackPanel);
            Grid.SetRow(rightStackPanel, 0);
            Grid.SetColumn(rightStackPanel, 1);

            productHeading = CreateTextBlock("Välj produkt", 18, TextAlignment.Center, rightStackPanel);
            productHeading.Margin = new Thickness(5, 30, 5, 32);

            imageGrid = new Grid();
            rightStackPanel.Children.Add(imageGrid);

            currentImage = CreateImage("Images/Ica.png");
            imageGrid.Children.Add(currentImage);

            productDescriptionHeading = CreateTextBlock("", 16, TextAlignment.Center, rightStackPanel);
            productDescription = CreateTextBlock("", 12, TextAlignment.Left, rightStackPanel);
            productDescription.FontWeight = FontWeights.Thin;
            productDescription.Margin = new Thickness(30, 5, 30, 5);
            // ##### End of right StackPanel definition ####
        }

        private void AddToCouponTextBox(object sender, SelectionChangedEventArgs e)
        {
            if (couponComboBox.SelectedIndex != 0)
            {
                couponTextBox.Text = couponList[couponComboBox.SelectedIndex - 1].Code;
            }
            else
            {
                couponTextBox.Text = "";
            }
        }

        private void IncreaseCartRowSpan(object sender, RoutedEventArgs e)
        {
            Grid.SetColumnSpan(cartExpander, 2);
        }

        private void DecreaseCartRowSpan(object sender, RoutedEventArgs e)
        {
            Grid.SetColumnSpan(cartExpander, 1);
        }

        private void ShowCategory(object sender, SelectionChangedEventArgs e)
        {
            searchTermList.Clear();
            if (categoryBox.SelectedIndex != 0 && categoryBox != null)
            {
                string category = categoryList[categoryBox.SelectedIndex - 1];
                var searchTermProducts = productList.Where(product => product.Category.Contains(category));
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

        private void ShowSearch(object sender, TextChangedEventArgs e)
        {
            string searchTerm = searchBox.Text;
            UpdateProductListBox(searchTerm);
        }

        private void ValidateCoupon(object sender, RoutedEventArgs e)
        {
            string input = couponTextBox.Text;
            bool validCoupon = Coupon.IsValid(input);

            //This will update our coupon if the discount will leave the user with a better price than before
            if(validCoupon && currentCoupon != null)
            {
                Coupon compare = Coupon.CouponCodes().First(x => x.Code == input);
                if(compare.Discount < currentCoupon.Discount)
                {
                    currentCoupon = compare;
                }
                else
                {
                    MessageBox.Show("Din nuvarande rabattkod har en bättre rabatt än den du nyligen matade in!");
                }

            }

            currentCoupon = validCoupon ? Coupon.CouponCodes().First(x => x.Code == input) : null;

            hasDiscount = currentCoupon != null;

            if(currentCoupon == null)
            {
                MessageBox.Show("Den inmatade rabattkoden är ej giltig, försök igen.");
                return;
            }
            UpdateShoppingCart();
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
                Margin = new Thickness(5),
                MaxHeight = 330
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
            else if (cartListBox.SelectedIndex != -1 && shoppingCart[cartListBox.SelectedIndex].Count == 1)
            {
                shoppingCart.RemoveAt(cartListBox.SelectedIndex);
                UpdateShoppingCart();
                return;
            }
            MessageBox.Show("Välj en produkt att ta bort");
        }

        private void RemoveAllProductsClick(object sender, RoutedEventArgs e)
        {
            if (cartListBox.SelectedIndex != -1)
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

            if (hasDiscount) sum *= currentCoupon.Discount;

            sumTextBlock.Text = "Varukorgens summa: " + (Convert.ToString(Math.Round(sum, 1))) + " kr";
            cartExpander.Header = "Din varukorg " + (Convert.ToString(Math.Round(sum, 1))) + " kr";
        }

        private void UpdateProductListBox(string searchTerm)
        {
            searchTermList.Clear();
            if (searchTerm != "")
            {
                var searchTermProducts = productList.Where(product => product.Title.ToLower().Contains(searchTerm.ToLower()));
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

        private void GenerateCategories()
        {
            foreach (var product in productList)
            {
                if (!categoryList.Contains(product.Category))
                {
                    categoryList.Add(product.Category);
                }
            }
        }

        private void OnPaymentClick(object sender, RoutedEventArgs e)
        {
            decimal sum = shoppingCart.Sum(x => x.Price);
            decimal saved = sum - (sum * (currentCoupon != null ? currentCoupon.Discount : 0));

            string items = string.Join("\n", shoppingCart.Select(x => $"{x.Count}x {x.Title} {x.Price * x.Count}"));
            string payment = $"Du betalade: {sum}:- " + currentCoupon != null ? $"och sparade: {saved}:-" : "";

            MessageBox.Show($"{items}\n{payment}");
        }
    }
}