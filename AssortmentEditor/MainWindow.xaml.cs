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
        List<Product> productList = ShopUtils.DeserializeProducts(ShopUtils.GetFilePath("Products.json"));
        List<Coupon> couponList = Coupon.DeserializeCoupons();

        Grid buttonClickGrid;

        Brush listBoxBrush, textBoxBrush;

        ListBox editProductListBox, pictureListBox, editCouponListBox;

        TextBox nameBox, descriptionBox, priceBox, categoryBox, pathBox, codeBox, discountBox;

        Grid imageGrid;

        string[] imageArray = new[] { "Dairy.jpg", "Fish.jpg", "Meat.jpg", "Shelf.jpg", "Veg.jpg" };

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

            #region Custom brushes
            // declare a brushconverter to convert a hex color code string to a Brush color
            BrushConverter brushConverter = new System.Windows.Media.BrushConverter();
            Brush backgroundBrush = (Brush)brushConverter.ConvertFromString("#2F3136");
            listBoxBrush = (Brush)brushConverter.ConvertFromString("#36393F");
            textBoxBrush = (Brush)brushConverter.ConvertFromString("#40444B");
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
            Grid mainGrid = new Grid();
            root.Content = mainGrid;
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.Background = backgroundBrush;

            TextBlock headingTextBlock = ShopUtils.CreateTextBlock("Sortiment hanteraren", 18, TextAlignment.Center);
            mainGrid.Children.Add(headingTextBlock);
            Grid.SetRow(headingTextBlock, 0);
            Grid.SetColumnSpan(headingTextBlock, 3);

            // first column contains buttons which determines what grid to create, the second column contains a class field grid "buttonClickGrid" which is the parent grid for the eventhandlers nested grids
            Grid selectionGrid = new Grid();
            Grid.SetRow(selectionGrid, 2);
            selectionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            selectionGrid.ColumnDefinitions.Add(new ColumnDefinition());
            selectionGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.Children.Add(selectionGrid);

            buttonClickGrid = new Grid();
            selectionGrid.Children.Add(buttonClickGrid);
            Grid.SetRow(buttonClickGrid, 1);
            Grid.SetColumn(buttonClickGrid, 1);

            // using a stackpanel to save some space defining a grid
            StackPanel buttonPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            selectionGrid.Children.Add(buttonPanel);
            Grid.SetColumn(buttonPanel, 0);

            Button editProductButton = AssortmentUtils.CreateButton("Ändra produkter");
            editProductButton.Padding = new Thickness(10);
            buttonPanel.Children.Add(editProductButton);
            editProductButton.Click += CreateEditProductGrid;

            Button addProductButton = AssortmentUtils.CreateButton("Lägg till produkt");
            addProductButton.Padding = new Thickness(10);
            buttonPanel.Children.Add(addProductButton);
            addProductButton.Click += CreateAddProductGrid;

            Button EditCouponsButton = AssortmentUtils.CreateButton("Lägg till/Ändra kuponger");
            EditCouponsButton.Padding = new Thickness(10);
            buttonPanel.Children.Add(EditCouponsButton);
            EditCouponsButton.Click += CreateEditCouponsGrid;

        }

        private void CreateEditCouponsGrid(object sender, RoutedEventArgs e)
        {
            buttonClickGrid.Children.Clear();

            Grid editCouponGrid = new Grid();
            editCouponGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            editCouponGrid.RowDefinitions.Add(new RowDefinition());
            editCouponGrid.ColumnDefinitions.Add(new ColumnDefinition());
            editCouponGrid.ColumnDefinitions.Add(new ColumnDefinition());

            buttonClickGrid.Children.Add(editCouponGrid);

            TextBlock changeCouponHeader = ShopUtils.CreateTextBlock("Ändra kuponger", 16, TextAlignment.Center);
            editCouponGrid.Children.Add(changeCouponHeader);
            Grid.SetRow(changeCouponHeader, 0);
            Grid.SetColumnSpan(changeCouponHeader, 2);

            Grid couponGrid = new Grid();
            couponGrid.RowDefinitions.Add(new RowDefinition());
            couponGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            couponGrid.ColumnDefinitions.Add(new ColumnDefinition());
            editCouponGrid.Children.Add(couponGrid);
            Grid.SetRow(couponGrid, 1);
            Grid.SetColumn(couponGrid, 0);

            Button removeCouponButton = new Button
            {
                Content = "Ta bort vald kupong",
                Margin = new Thickness(5, 5, 5, 100),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                MaxWidth = 120,
                VerticalAlignment = VerticalAlignment.Top
            };
            couponGrid.Children.Add(removeCouponButton);
            Grid.SetRow(removeCouponButton, 1);
            Grid.SetColumn(removeCouponButton, 0);
            removeCouponButton.Click += RemoveCouponClick;

            editCouponListBox = new ListBox
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
            couponGrid.Children.Add(editCouponListBox);
            Grid.SetRow(editCouponListBox, 0);
            Grid.SetColumn(editCouponListBox, 0);
            UpdateCouponListBox();
            editCouponListBox.SelectionChanged += AddSelectedCouponToTextBox;

            Grid couponPropertiesGrid = new Grid();
            couponPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            couponPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            couponPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            couponPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            couponPropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            couponPropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition());
            editCouponGrid.Children.Add(couponPropertiesGrid);
            Grid.SetRow(couponPropertiesGrid, 1);
            Grid.SetColumn(couponPropertiesGrid, 1);

            TextBlock editCouponHeader = Projektarbete.ShopUtils.CreateTextBlock("Kupongens egenskaper", 14, TextAlignment.Center);
            couponPropertiesGrid.Children.Add(editCouponHeader);
            Grid.SetColumnSpan(editCouponHeader, 2);

            TextBlock editCodeText = Projektarbete.ShopUtils.CreateTextBlock("Kod", 10, TextAlignment.Left);
            couponPropertiesGrid.Children.Add(editCodeText);
            Grid.SetRow(editCodeText, 1);
            Grid.SetColumn(editCodeText, 0);

            codeBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            couponPropertiesGrid.Children.Add(codeBox);
            codeBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(codeBox, 1);
            Grid.SetColumn(codeBox, 1);

            TextBlock editDiscountText = Projektarbete.ShopUtils.CreateTextBlock("Rabatt", 10, TextAlignment.Left);
            couponPropertiesGrid.Children.Add(editDiscountText);
            Grid.SetRow(editDiscountText, 2);
            Grid.SetColumn(editDiscountText, 0);

            discountBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            couponPropertiesGrid.Children.Add(discountBox);
            discountBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(discountBox, 2);
            Grid.SetColumn(discountBox, 2);

            Button addNewCoupon = new Button
            {
                Content = "Lägg till ny kupong",
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                MaxWidth = 120,
                Padding = new Thickness(5, 2, 5, 2)
            };
            couponPropertiesGrid.Children.Add(addNewCoupon);
            Grid.SetRow(addNewCoupon, 3);
            addNewCoupon.Click += AddNewCouponClick;

            Button saveCouponChanges = new Button
            {
                Content = "Spara ändringar",
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                MaxWidth = 120,
                Padding = new Thickness(5, 2, 5, 2)
            };
            couponPropertiesGrid.Children.Add(saveCouponChanges);
            Grid.SetRow(saveCouponChanges, 3);
            Grid.SetColumn(saveCouponChanges, 1);
            saveCouponChanges.Click += SaveCouponChangesClick;
        }

        private void AddNewCouponClick(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveCouponClick(object sender, RoutedEventArgs e)
        {

        }

        private void SaveCouponChangesClick(object sender, RoutedEventArgs e)
        {

        }

        private void AddSelectedCouponToTextBox(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UpdateCouponListBox()
        {

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

            TextBlock addProductHeader = Projektarbete.ShopUtils.CreateTextBlock("Urval för bilder", 14, TextAlignment.Center);
            addProductGrid.Children.Add(addProductHeader);
            Grid.SetRow(addProductHeader, 0);
            Grid.SetColumn(addProductHeader, 1);

            Grid choosePictureGrid = new Grid();
            choosePictureGrid.RowDefinitions.Add(new RowDefinition());
            choosePictureGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            choosePictureGrid.RowDefinitions.Add(new RowDefinition());
            choosePictureGrid.ColumnDefinitions.Add(new ColumnDefinition());
            addProductGrid.Children.Add(choosePictureGrid);
            Grid.SetRow(choosePictureGrid, 1);
            Grid.SetColumn(choosePictureGrid, 1);
            choosePictureGrid.ShowGridLines = true;


            imageGrid = new Grid();
            choosePictureGrid.Children.Add(imageGrid);
            Grid.SetRow(imageGrid, 0);
            Grid.SetColumn(imageGrid, 0);

            Button addToImageBox = new Button
            {
                Content = "Lägg till sökväg",
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                MaxWidth = 120
            };
            choosePictureGrid.Children.Add(addToImageBox);
            Grid.SetRow(addToImageBox, 1);
            addToImageBox.Click += AddToImageBox;

            pictureListBox = new ListBox
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
            choosePictureGrid.Children.Add(pictureListBox);
            Grid.SetRow(pictureListBox, 2);
            Grid.SetColumn(pictureListBox, 0);
            pictureListBox.SelectionChanged += DisplayPicture;

            for (int i = 0; i < imageArray.Length; i++)
            {
                pictureListBox.Items.Add("Bildförslag " + (i + 1));
            }

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

        private void AddToImageBox(object sender, RoutedEventArgs e)
        {
            if (pictureListBox.SelectedIndex != -1)
            {
                pathBox.Text = imageArray[pictureListBox.SelectedIndex];
            }
        }

        private void DisplayPicture(object sender, SelectionChangedEventArgs e)
        {
            if (pictureListBox.SelectedIndex != -1)
            {
                imageGrid.Children.Clear();
                Image currentImage = CreateImage("SampleImages/" + imageArray[pictureListBox.SelectedIndex]);
                imageGrid.Children.Add(currentImage);
            }
        }

        private void AddProductClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists("Images/" + pathBox.Text))
                {
                    throw new FileNotFoundException();
                }
                Product product = new Product
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
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            editProductGrid.Children.Add(productGrid);
            Grid.SetRow(productGrid, 1);
            Grid.SetColumn(productGrid, 0);

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
            editProductListBox.SelectionChanged += AddSelectedProductToTextBox;

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

            TextBlock editProductHeader = ShopUtils.CreateTextBlock("Produktens egenskaper", 14, TextAlignment.Center);
            productPropertiesGrid.Children.Add(editProductHeader);

            TextBlock editNameText = ShopUtils.CreateTextBlock("Namn", 10, TextAlignment.Left);
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

            TextBlock editDescriptionText = ShopUtils.CreateTextBlock("Beskrivning", 10, TextAlignment.Left);
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

            TextBlock editPriceText = ShopUtils.CreateTextBlock("Pris", 10, TextAlignment.Left);
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

            TextBlock editCategoryText = ShopUtils.CreateTextBlock("Kategori", 10, TextAlignment.Left);
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

            TextBlock editPathText = ShopUtils.CreateTextBlock("Bild", 10, TextAlignment.Left);
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
                MaxWidth = 120,
                Padding = new Thickness(5, 1, 5, 1)
            };
            productPropertiesGrid.Children.Add(saveProductChanges);
            Grid.SetRow(saveProductChanges, 6);
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
                productList.Serialize(ShopUtils.GetFilePath("Products.json"));
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
                //productList.Serialize(ShopUtils.GetFilePath("Products.json"));
                UpdateProductListBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Felaktig inmatning: " + ex.Message);
            }
        }

        private void AddSelectedProductToTextBox(object sender, SelectionChangedEventArgs e)
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

        private Image CreateImage(string filePath)
        {
            ImageSource source = new BitmapImage(new Uri(filePath, UriKind.Relative));
            Image image = new Image
            {
                Source = source,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                MaxHeight = 300,
                MaxWidth = 300
            };
            image.Stretch = Stretch.UniformToFill;
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            return image;
        }
    }
}