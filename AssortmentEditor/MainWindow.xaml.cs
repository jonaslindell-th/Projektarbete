using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using Projektarbete;

namespace AssortmentEditor
{
    public partial class MainWindow : Window
    {
        List<Product> productList;
        List<Coupon> couponList;

        Grid editGrid;

        Grid buttonClickGrid;

        Brush listBoxBrush;
        Brush textBoxBrush;

        ListBox editProductListBox;

        TextBox nameBox;
        TextBox descriptionBox;
        TextBox priceBox;
        TextBox categoryBox;
        TextBox pathBox;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            if (!Directory.Exists($@"C:\Windows\Temp\Sebastian_Jonas"))
            {
                Directory.CreateDirectory(@"C:\Windows\Temp\Sebastian_Jonas");
                Product.CreateProductFile();
                Coupon.CreateCouponFile();
            }

            productList = ShopUtils.DeserializeProducts(ShopUtils.GetFilePath("Products.json"));
            couponList = Coupon.DeserializeCoupons();
            #region Custom brushes
            // declare a brushconverter to convert a hex color code string to a Brush color
            BrushConverter brushConverter = new System.Windows.Media.BrushConverter();
            Brush backgroundBrush = (Brush)brushConverter.ConvertFromString("#2F3136");
            listBoxBrush = (Brush)brushConverter.ConvertFromString("#36393F");
            textBoxBrush = (Brush)brushConverter.ConvertFromString("#40444B");
            Brush expanderBrush = (Brush)brushConverter.ConvertFromString("#202225");
            #endregion

            // Window options
            Title = "Sortiment hanteraren";
            Width = 900;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            // Changes the Window icon
            Uri iconUri = new Uri("Images/Ica.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            Grid selectionGrid = new Grid();
            root.Content = selectionGrid;
            //mainGrid.Margin = new Thickness(5);
            selectionGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            //selectionGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            selectionGrid.RowDefinitions.Add(new RowDefinition());
            selectionGrid.ColumnDefinitions.Add(new ColumnDefinition());
            selectionGrid.Background = backgroundBrush;

            TextBlock headingTextBlock = Projektarbete.ShopUtils.CreateTextBlock("Sortiment hanteraren", 18, TextAlignment.Center);
            selectionGrid.Children.Add(headingTextBlock);
            Grid.SetRow(headingTextBlock, 0);
            Grid.SetColumnSpan(headingTextBlock, 3);

            editGrid = new Grid();
            Grid.SetRow(editGrid, 2);
            editGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            editGrid.ColumnDefinitions.Add(new ColumnDefinition());
            editGrid.RowDefinitions.Add(new RowDefinition());
            selectionGrid.Children.Add(editGrid);

            StackPanel buttonPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            editGrid.Children.Add(buttonPanel);
            Grid.SetColumn(buttonPanel, 0);

            Button editProducts = new Button
            {
                Content = "Ändra produkter",
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Padding = new Thickness(10)
            };
            buttonPanel.Children.Add(editProducts);
            editProducts.Click += CreateEditProductGrid;

            Button addProduct = new Button
            {
                Content = "Lägg till produkt",
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Padding = new Thickness(10)
            };
            buttonPanel.Children.Add(addProduct);
            addProduct.Click += CreateAddProductGrid;

            Button editCoupons = new Button
            {
                Content = "Ändra kuponger",
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Padding = new Thickness(10)
            };
            buttonPanel.Children.Add(editCoupons);

            Button addCoupon = new Button
            {
                Content = "Lägg till kupong",
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Padding = new Thickness(10)
            };
            buttonPanel.Children.Add(addCoupon);
            editProducts.Click += CreateEditProductGrid;

            buttonClickGrid = new Grid();
            editGrid.Children.Add(buttonClickGrid);
            Grid.SetRow(buttonClickGrid, 1);
            Grid.SetColumn(buttonClickGrid, 1);
        }

        private void CreateAddProductGrid(object sender, RoutedEventArgs e)
        {
            buttonClickGrid.Children.Clear();

            Grid addProductGrid = new Grid();
            addProductGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            addProductGrid.RowDefinitions.Add(new RowDefinition());
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition());
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition());

            buttonClickGrid.Children.Add(addProductGrid);

            TextBlock changeProductsHeader = Projektarbete.ShopUtils.CreateTextBlock("Lägg till produkt", 16, TextAlignment.Center);
            addProductGrid.Children.Add(changeProductsHeader);
            Grid.SetRow(changeProductsHeader, 0);
            Grid.SetColumnSpan(changeProductsHeader, 2);

            Grid productPropertiesGrid = new Grid();
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            productPropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition());
            addProductGrid.Children.Add(productPropertiesGrid);
            Grid.SetRow(productPropertiesGrid, 1);
            Grid.SetColumn(productPropertiesGrid, 0);

            TextBlock editProductHeader = Projektarbete.ShopUtils.CreateTextBlock("Produktens egenskaper", 14, TextAlignment.Center);
            productPropertiesGrid.Children.Add(editProductHeader);
            Grid.SetColumnSpan(editProductHeader, 2);

            TextBlock editNameText = Projektarbete.ShopUtils.CreateTextBlock("Namn", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editNameText);
            Grid.SetRow(editNameText, 1);
            Grid.SetColumn(editNameText, 0);

            nameBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            productPropertiesGrid.Children.Add(nameBox);
            nameBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(nameBox, 1);
            Grid.SetColumn(nameBox, 1);

            TextBlock editDescriptionText = Projektarbete.ShopUtils.CreateTextBlock("Beskrivning", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editDescriptionText);
            Grid.SetRow(editDescriptionText, 2);
            Grid.SetColumn(editDescriptionText, 0);

            descriptionBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            productPropertiesGrid.Children.Add(descriptionBox);
            descriptionBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(descriptionBox, 2);
            Grid.SetColumn(descriptionBox, 1);

            TextBlock editPriceText = Projektarbete.ShopUtils.CreateTextBlock("Pris", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editPriceText);
            Grid.SetRow(editPriceText, 3);
            Grid.SetColumn(editPriceText, 0);

            priceBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            productPropertiesGrid.Children.Add(priceBox);
            priceBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(priceBox, 3);
            Grid.SetColumn(priceBox, 1);

            TextBlock editCategoryText = Projektarbete.ShopUtils.CreateTextBlock("Kategori", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editCategoryText);
            Grid.SetRow(editCategoryText, 4);
            Grid.SetColumn(editCategoryText, 0);

            categoryBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            productPropertiesGrid.Children.Add(categoryBox);
            categoryBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(categoryBox, 4);
            Grid.SetColumn(categoryBox, 1);

            TextBlock editPathText = Projektarbete.ShopUtils.CreateTextBlock("Bild", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editPathText);
            Grid.SetRow(editPathText, 5);
            Grid.SetColumn(editPathText, 0);

            pathBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            productPropertiesGrid.Children.Add(pathBox);
            pathBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(pathBox, 5);
            Grid.SetColumn(pathBox, 1);

            Button addProductButton = new Button
            {
                Content = "Lägg till produkt",
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                MaxWidth = 120
            };
            productPropertiesGrid.Children.Add(addProductButton);
            Grid.SetRow(addProductButton, 6);
            Grid.SetColumnSpan(addProductButton, 2);
            addProductButton.Click += AddProductClick;
        }

        private void AddProductClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists("Images/" + pathBox.Text))
                {
                    throw new FileNotFoundException();
                }
                Projektarbete.Product product = new Projektarbete.Product
                {
                    Title = nameBox.Text,
                    Description = descriptionBox.Text,
                    Price = decimal.Parse(priceBox.Text),
                    ProductImage = pathBox.Text,
                    Category = categoryBox.Text
                };
                productList.Add(product);
                nameBox.Clear();
                descriptionBox.Clear();
                priceBox.Clear();
                pathBox.Clear();
                categoryBox.Clear();
                productList.Serialize(ShopUtils.GetFilePath("Products.json"));
                MessageBox.Show("Produkt tillagd");
            }
            catch (FormatException)
            {
                MessageBox.Show("Felaktig inmatning för pris");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Bilden kunde inte hittas");
            }
        }

        private void CreateEditProductGrid(object sender, RoutedEventArgs e)
        {

            buttonClickGrid.Children.Clear();

            Grid editProductGrid = new Grid();
            editProductGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            editProductGrid.RowDefinitions.Add(new RowDefinition());
            editProductGrid.ColumnDefinitions.Add(new ColumnDefinition());
            editProductGrid.ColumnDefinitions.Add(new ColumnDefinition());

            buttonClickGrid.Children.Add(editProductGrid);

            TextBlock changeProductsHeader = Projektarbete.ShopUtils.CreateTextBlock("Ändra produkter", 16, TextAlignment.Center);
            editProductGrid.Children.Add(changeProductsHeader);
            Grid.SetRow(changeProductsHeader, 0);
            Grid.SetColumnSpan(changeProductsHeader, 2);

            Grid productGrid = new Grid();
            productGrid.RowDefinitions.Add(new RowDefinition ());
            productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            editProductGrid.Children.Add(productGrid);
            Grid.SetRow(productGrid, 1);
            Grid.SetColumn(productGrid, 0);
            productGrid.ShowGridLines = true;

            Button removeProductButton = new Button
            {
                Content = "Ta bort vald produkt",
                Margin = new Thickness(5, 5, 5, 100),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                MaxWidth = 120,
                VerticalAlignment = VerticalAlignment.Top
            };
            productGrid.Children.Add(removeProductButton);
            Grid.SetRow(removeProductButton, 1);
            Grid.SetColumn(removeProductButton, 0);
            removeProductButton.Click += RemoveProductClick;

            editProductListBox = new ListBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MaxHeight = 425,
                MaxWidth = 300,
                Background = listBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            productGrid.Children.Add(editProductListBox);
            Grid.SetRow(editProductListBox, 0);
            Grid.SetColumn(editProductListBox, 0);
            UpdateProductListBox();
            editProductListBox.SelectionChanged += SetBoxText;

            Grid productPropertiesGrid = new Grid();
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productPropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            productPropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition());
            editProductGrid.Children.Add(productPropertiesGrid);
            Grid.SetRow(productPropertiesGrid, 1);
            Grid.SetColumn(productPropertiesGrid, 1);

            TextBlock editProductHeader = Projektarbete.ShopUtils.CreateTextBlock("Produktens egenskaper", 14, TextAlignment.Center);
            productPropertiesGrid.Children.Add(editProductHeader);
            Grid.SetColumnSpan(editProductHeader, 2);

            TextBlock editNameText = Projektarbete.ShopUtils.CreateTextBlock("Namn", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editNameText);
            Grid.SetRow(editNameText, 1);
            Grid.SetColumn(editNameText, 0);

            nameBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            productPropertiesGrid.Children.Add(nameBox);
            nameBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(nameBox, 1);
            Grid.SetColumn(nameBox, 1);

            TextBlock editDescriptionText = Projektarbete.ShopUtils.CreateTextBlock("Beskrivning", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editDescriptionText);
            Grid.SetRow(editDescriptionText, 2);
            Grid.SetColumn(editDescriptionText, 0);

            descriptionBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            productPropertiesGrid.Children.Add(descriptionBox);
            descriptionBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(descriptionBox, 2);
            Grid.SetColumn(descriptionBox, 1);

            TextBlock editPriceText = Projektarbete.ShopUtils.CreateTextBlock("Pris", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editPriceText);
            Grid.SetRow(editPriceText, 3);
            Grid.SetColumn(editPriceText, 0);

            priceBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            productPropertiesGrid.Children.Add(priceBox);
            priceBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(priceBox, 3);
            Grid.SetColumn(priceBox, 1);

            TextBlock editCategoryText = Projektarbete.ShopUtils.CreateTextBlock("Kategori", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editCategoryText);
            Grid.SetRow(editCategoryText, 4);
            Grid.SetColumn(editCategoryText, 0);

            categoryBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            productPropertiesGrid.Children.Add(categoryBox);
            categoryBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(categoryBox, 4);
            Grid.SetColumn(categoryBox, 1);

            TextBlock editPathText = Projektarbete.ShopUtils.CreateTextBlock("Bild", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editPathText);
            Grid.SetRow(editPathText, 5);
            Grid.SetColumn(editPathText, 0);

            pathBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            productPropertiesGrid.Children.Add(pathBox);
            pathBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(pathBox, 5);
            Grid.SetColumn(pathBox, 1);

            Button saveProductChanges = new Button
            {
                Content = "Spara ändringar",
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                MaxWidth = 120
            };
            productPropertiesGrid.Children.Add(saveProductChanges);
            Grid.SetRow(saveProductChanges, 6);
            Grid.SetColumnSpan(saveProductChanges, 2);
            saveProductChanges.Click += SaveProductChangesClick;
        }

        private void RemoveProductClick(object sender, RoutedEventArgs e)
        {
            if (editProductListBox.SelectedIndex != -1)
            {
                productList.RemoveAt(editProductListBox.SelectedIndex);
                UpdateProductListBox();
                nameBox.Clear();
                descriptionBox.Clear();
                priceBox.Clear();
                pathBox.Clear();
                categoryBox.Clear();
                MessageBox.Show("Produkt borttagen");
            }
            else
            {
                MessageBox.Show("Ingen produkt vald");
            }
        }

        private void SaveProductChangesClick(object sender, RoutedEventArgs e)
        {
            try
            {
                productList[editProductListBox.SelectedIndex].Title = nameBox.Text;
                productList[editProductListBox.SelectedIndex].Description = descriptionBox.Text;
                productList[editProductListBox.SelectedIndex].Price = decimal.Parse(priceBox.Text);
                productList[editProductListBox.SelectedIndex].Category = categoryBox.Text;
                productList[editProductListBox.SelectedIndex].ProductImage = pathBox.Text;
                productList.Serialize(ShopUtils.GetFilePath("Products.json"));
                UpdateProductListBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Felaktig inmatning: " + ex.Message);
            }
        }

        private void SetBoxText(object sender, SelectionChangedEventArgs e)
        {
            if (editProductListBox.SelectedIndex != -1)
            {
                nameBox.Text = productList[editProductListBox.SelectedIndex].Title;
                descriptionBox.Text = productList[editProductListBox.SelectedIndex].Description;
                priceBox.Text = productList[editProductListBox.SelectedIndex].Price.ToString();
                categoryBox.Text = productList[editProductListBox.SelectedIndex].Category;
                pathBox.Text = productList[editProductListBox.SelectedIndex].ProductImage;
            }
        }

        private void UpdateProductListBox()
        {
            editProductListBox.Items.Clear();
            foreach (Projektarbete.Product product in productList)
            {
                editProductListBox.Items.Add(product.Title + " (" + product.Price + ") kr");
            }
        }
    }
}