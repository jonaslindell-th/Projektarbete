using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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

            ShopUtils.CreateFiles();

            productList = ShopUtils.DeserializeProducts(ShopUtils.GetFilePath("Products.json"));
            couponList = Coupon.DeserializeCoupons();

            #region Custom brushes
            // declare a brushconverter to convert a hex color code string to a Brush color
            BrushConverter brushConverter = new System.Windows.Media.BrushConverter();
            Brush backgroundBrush = (Brush)brushConverter.ConvertFromString("#2F3136");
            listBoxBrush = (Brush)brushConverter.ConvertFromString("#36393F");
            textBoxBrush = (Brush)brushConverter.ConvertFromString("#40444B");
            #endregion

            // Window options
            Title = "Sortiment hanteraren";
            Width = 1100;
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
            // first column contains a stackpanel with buttons which determines what grid to create, the second column contains a class field grid "buttonClickGrid" which is the parent grid for the eventhandlers nested grids
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.Background = backgroundBrush;

            // Window heading, always visible
            TextBlock headingTextBlock = ShopUtils.CreateTextBlock("Sortiment hanteraren", 18, TextAlignment.Center);
            mainGrid.Children.Add(headingTextBlock);
            Grid.SetRow(headingTextBlock, 0);
            Grid.SetColumnSpan(headingTextBlock, 2);

            StackPanel buttonPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Top };
            mainGrid.Children.Add(buttonPanel);
            Grid.SetColumn(buttonPanel, 0);
            Grid.SetRow(buttonPanel, 1);

            // using a empty class field grid to be able to add nested grids from the eventhandlers
            buttonClickGrid = new Grid();
            mainGrid.Children.Add(buttonClickGrid);
            Grid.SetRow(buttonClickGrid, 1);
            Grid.SetColumn(buttonClickGrid, 1);

            // each button declares a new grid which is added as buttonClickGrid's only child to display a menu to add/edit/remove coupons and products
            Button editProductButton = ShopUtils.CreateButton("Ändra produkter");
            editProductButton.Padding = new Thickness(10);
            buttonPanel.Children.Add(editProductButton);
            editProductButton.Click += CreateEditProductGrid;

            Button addProductButton = ShopUtils.CreateButton("Lägg till produkt");
            addProductButton.Padding = new Thickness(10);
            buttonPanel.Children.Add(addProductButton);
            addProductButton.Click += CreateAddProductGrid;

            Button EditCouponsButton = ShopUtils.CreateButton("Lägg till/Ändra kuponger");
            EditCouponsButton.Padding = new Thickness(10);
            buttonPanel.Children.Add(EditCouponsButton);
            EditCouponsButton.Click += CreateEditCouponsGrid;

        }

        private void CreateEditCouponsGrid(object sender, RoutedEventArgs e)
        {
            // always clear the buttonClickGrid to avoid stacking grids on top of each other
            buttonClickGrid.Children.Clear();

            //Main grid for the coupon grid
            Grid editCouponGrid = new Grid();
            editCouponGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            editCouponGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            editCouponGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            editCouponGrid.ColumnDefinitions.Add(new ColumnDefinition());
            editCouponGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonClickGrid.Children.Add(editCouponGrid);

            TextBlock changeCouponHeader = ShopUtils.CreateTextBlock("Ändra kuponger", 16, TextAlignment.Center);
            editCouponGrid.Children.Add(changeCouponHeader);
            Grid.SetRow(changeCouponHeader, 0);
            Grid.SetColumnSpan(changeCouponHeader, 2);

            Button removeCouponButton = ShopUtils.CreateButton("Ta bort vald kupong");
            removeCouponButton.MaxWidth = 120;
            editCouponGrid.Children.Add(removeCouponButton);
            Grid.SetRow(removeCouponButton, 2);
            Grid.SetColumn(removeCouponButton, 0);
            removeCouponButton.Click += RemoveCouponClick;

            // listbox to display all editable coupons
            editCouponListBox = new ListBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MaxHeight = 300,
                MaxWidth = 300,
                MinHeight = 150,
                Background = listBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            editCouponGrid.Children.Add(editCouponListBox);
            Grid.SetRow(editCouponListBox, 1);
            Grid.SetColumn(editCouponListBox, 0);
            // fills the listbox with items from couponList
            UpdateCouponListBox();
            editCouponListBox.SelectionChanged += AddSelectedCouponToTextBox;

            // using a nested grid to set coupon properties in order for the textboxes and listbox to stay leveled at the same height
            Grid couponPropertiesGrid = new Grid();
            couponPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            couponPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            couponPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            couponPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            couponPropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            couponPropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            editCouponGrid.Children.Add(couponPropertiesGrid);
            Grid.SetRow(couponPropertiesGrid, 1);
            Grid.SetColumn(couponPropertiesGrid, 1);

            // add children to the couponPropertiesGrid
            TextBlock editCouponHeader = ShopUtils.CreateTextBlock("Kupongens egenskaper", 14, TextAlignment.Center);
            couponPropertiesGrid.Children.Add(editCouponHeader);
            Grid.SetColumnSpan(editCouponHeader, 2);

            TextBlock editCodeText = ShopUtils.CreateTextBlock("Kod", 10, TextAlignment.Left);
            couponPropertiesGrid.Children.Add(editCodeText);
            Grid.SetRow(editCodeText, 1);
            Grid.SetColumn(editCodeText, 0);

            codeBox = ShopUtils.CreateTextBox(textBoxBrush);
            couponPropertiesGrid.Children.Add(codeBox);
            codeBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(codeBox, 1);
            Grid.SetColumn(codeBox, 1);

            TextBlock editDiscountText = ShopUtils.CreateTextBlock("Rabatt", 10, TextAlignment.Left);
            couponPropertiesGrid.Children.Add(editDiscountText);
            Grid.SetRow(editDiscountText, 2);
            Grid.SetColumn(editDiscountText, 0);

            discountBox = ShopUtils.CreateTextBox(textBoxBrush);
            couponPropertiesGrid.Children.Add(discountBox);
            discountBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(discountBox, 2);
            Grid.SetColumn(discountBox, 2);

            Button addNewCoupon = ShopUtils.CreateButton("Lägg till ny kupong");
            addNewCoupon.Padding = new Thickness(5, 2, 5, 2);
            couponPropertiesGrid.Children.Add(addNewCoupon);
            Grid.SetRow(addNewCoupon, 3);
            addNewCoupon.Click += AddNewCouponClick;

            Button saveCouponChanges = ShopUtils.CreateButton("Spara ändringar");
            saveCouponChanges.Padding = new Thickness(5, 2, 5, 2);
            couponPropertiesGrid.Children.Add(saveCouponChanges);
            Grid.SetRow(saveCouponChanges, 3);
            Grid.SetColumn(saveCouponChanges, 1);
            saveCouponChanges.Click += SaveCouponChangesClick;
        }

        private void AddNewCouponClick(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(discountBox.Text, out decimal discount))
            {
                decimal converted = 1 - (discount / 100);

                // Regex.IsMatch checks if codeBox.Text matches alphabetic characters between a-z and A-Z and numbers 0-9
                if (codeBox.Text.Length < 21 && codeBox.Text.Length > 2 && Regex.IsMatch(codeBox.Text, "^[a-zA-Z0-9]*$"))
                {
                    Coupon coupon = new Coupon(codeBox.Text, converted);
                    couponList.Add(coupon);
                    discountBox.Clear();
                    codeBox.Clear();
                    UpdateCouponListBox();
                }
                else
                {
                    MessageBox.Show("Koden måste vara minst 3 tecken eller max 20 och endast innehålla engelska bokstäver eller siffror.");
                }
            }
            else
            {
                MessageBox.Show("Rabattvärdet du angav är inte ett giltigt nummer.");
            }
        }

        private void RemoveCouponClick(object sender, RoutedEventArgs e)
        {
            int index = editCouponListBox.SelectedIndex;
            if (index != -1)
            {
                couponList.RemoveAt(index);
                UpdateCouponListBox();
                codeBox.Clear();
                discountBox.Clear();
                MessageBox.Show("Kupong borttagen.");
            }
            else
            {
                MessageBox.Show("Ingen kupong vald.");
            }
        }

        private void SaveCouponChangesClick(object sender, RoutedEventArgs e)
        {
            int index = editCouponListBox.SelectedIndex;
            try
            {
                if (index != -1)
                {
                    // Regex.IsMatch checks if codeBox.Text matches alphabetic characters between a-z and A-Z and numbers 0-9 
                    if (codeBox.Text.Length < 21 && codeBox.Text.Length > 2 && Regex.IsMatch(codeBox.Text, "^[a-zA-Z0-9]*$"))
                    {
                        couponList[index].Code = codeBox.Text;
                        couponList[index].Discount = 1 - (decimal.Parse(discountBox.Text) / 100);
                        UpdateCouponListBox();
                    }
                    else
                    {
                        MessageBox.Show("Ogiltiga rabattkod");
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Felaktig inmatning!");
            }
        }

        private void AddSelectedCouponToTextBox(object sender, SelectionChangedEventArgs e)
        {
            int index = editCouponListBox.SelectedIndex;
            if (index != -1)
            {
                int discount = (int)((1 - couponList[index].Discount) * 100);
                codeBox.Text = couponList[index].Code;
                discountBox.Text = discount.ToString();
            }
        }

        private void UpdateCouponListBox()
        {
            editCouponListBox.Items.Clear();
            foreach (Coupon c in couponList)
            {
                int percent = (int)((1 - c.Discount) * 100);
                editCouponListBox.Items.Add($"{c.Code} ({percent}%)");
            }
            couponList.Serialize(ShopUtils.GetFilePath("Coupons.json"));
        }

        private void CreateAddProductGrid(object sender, RoutedEventArgs e)
        {
            buttonClickGrid.Children.Clear();

            // main grid for adding products
            Grid addProductGrid = new Grid();
            addProductGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            addProductGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition());
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition());
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonClickGrid.Children.Add(addProductGrid);

            TextBlock addProductHeader = ShopUtils.CreateTextBlock("Urval för bilder", 14, TextAlignment.Center);
            addProductGrid.Children.Add(addProductHeader);
            Grid.SetRow(addProductHeader, 0);
            Grid.SetColumn(addProductHeader, 1);

            // using a class field grid to be able to set the currently selected image from selectionchanged eventhandler
            imageGrid = new Grid();
            addProductGrid.Children.Add(imageGrid);
            Grid.SetRow(imageGrid, 0);
            Grid.SetColumn(imageGrid, 3);

            Button addToImageBox = ShopUtils.CreateButton("Lägg till sökväg");
            addToImageBox.MaxWidth = 120;
            addProductGrid.Children.Add(addToImageBox);
            Grid.SetRow(addToImageBox, 1);
            Grid.SetColumn(addToImageBox, 1);
            addToImageBox.Click += AddToImageBox;

            // A ListBox containing available sample images and displaying them using the selectionchanged event
            pictureListBox = new ListBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MaxHeight = 425,
                MaxWidth = 300,
                MinWidth = 300,
                MinHeight = 300,
                Background = listBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            addProductGrid.Children.Add(pictureListBox);
            Grid.SetRow(pictureListBox, 0);
            Grid.SetColumn(pictureListBox, 1);
            pictureListBox.SelectionChanged += DisplayPicture;

            for (int i = 0; i < imageArray.Length; i++)
            {
                pictureListBox.Items.Add("Bildförslag " + (i + 1));
            }

            // grid containing all textboxes and textblocks to set the new products properties
            Grid productPropertiesGrid = new Grid { HorizontalAlignment = HorizontalAlignment.Right };
            for (int i = 0; i < 7; i++)
            {
                productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            productPropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            productPropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            addProductGrid.Children.Add(productPropertiesGrid);
            Grid.SetRow(productPropertiesGrid, 0);
            Grid.SetColumn(productPropertiesGrid, 0);

            TextBlock editProductHeader = ShopUtils.CreateTextBlock("Produktens egenskaper", 14, TextAlignment.Center);
            productPropertiesGrid.Children.Add(editProductHeader);
            Grid.SetColumnSpan(editProductHeader, 2);

            TextBlock editNameText = ShopUtils.CreateTextBlock("Namn", 10, TextAlignment.Right);
            productPropertiesGrid.Children.Add(editNameText);
            Grid.SetRow(editNameText, 1);
            Grid.SetColumn(editNameText, 0);

            nameBox = ShopUtils.CreateTextBox(textBoxBrush);
            productPropertiesGrid.Children.Add(nameBox);
            nameBox.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(nameBox, 1);
            Grid.SetColumn(nameBox, 1);

            TextBlock editDescriptionText = ShopUtils.CreateTextBlock("Beskrivning", 10, TextAlignment.Right);
            productPropertiesGrid.Children.Add(editDescriptionText);
            Grid.SetRow(editDescriptionText, 2);
            Grid.SetColumn(editDescriptionText, 0);

            descriptionBox = ShopUtils.CreateTextBox(textBoxBrush);
            productPropertiesGrid.Children.Add(descriptionBox);
            descriptionBox.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(descriptionBox, 2);
            Grid.SetColumn(descriptionBox, 1);

            TextBlock editPriceText = ShopUtils.CreateTextBlock("Pris", 10, TextAlignment.Right);
            productPropertiesGrid.Children.Add(editPriceText);
            Grid.SetRow(editPriceText, 3);
            Grid.SetColumn(editPriceText, 0);

            priceBox = ShopUtils.CreateTextBox(textBoxBrush);
            productPropertiesGrid.Children.Add(priceBox);
            priceBox.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(priceBox, 3);
            Grid.SetColumn(priceBox, 1);

            TextBlock editCategoryText = ShopUtils.CreateTextBlock("Kategori", 10, TextAlignment.Right);
            productPropertiesGrid.Children.Add(editCategoryText);
            Grid.SetRow(editCategoryText, 4);
            Grid.SetColumn(editCategoryText, 0);

            categoryBox = ShopUtils.CreateTextBox(textBoxBrush);
            productPropertiesGrid.Children.Add(categoryBox);
            categoryBox.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(categoryBox, 4);
            Grid.SetColumn(categoryBox, 1);

            TextBlock editPathText = ShopUtils.CreateTextBlock("Bild", 10, TextAlignment.Right);
            productPropertiesGrid.Children.Add(editPathText);
            Grid.SetRow(editPathText, 5);
            Grid.SetColumn(editPathText, 0);

            pathBox = ShopUtils.CreateTextBox(textBoxBrush);
            productPropertiesGrid.Children.Add(pathBox);
            pathBox.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetRow(pathBox, 5);
            Grid.SetColumn(pathBox, 1);

            Button addProductButton = ShopUtils.CreateButton("Lägg till produkt");
            addProductButton.MaxWidth = 120;
            productPropertiesGrid.Children.Add(addProductButton);
            Grid.SetRow(addProductButton, 6);
            Grid.SetColumnSpan(addProductButton, 2);
            addProductButton.Click += AddProductClick;
        }

        private void AddToImageBox(object sender, RoutedEventArgs e)
        {
            // adds the path from the currently displayed image to the path TextBox
            if (pictureListBox.SelectedIndex != -1)
            {
                pathBox.Text = imageArray[pictureListBox.SelectedIndex];
            }
        }

        private void DisplayPicture(object sender, SelectionChangedEventArgs e)
        {
            // Gets a path from the imageArray by referring to the selected index in the listbox and adds it in the imageGrid
            if (pictureListBox.SelectedIndex != -1)
            {
                imageGrid.Children.Clear();
                Image currentImage = ShopUtils.CreateImage("SampleImages/" + imageArray[pictureListBox.SelectedIndex]);
                imageGrid.Children.Add(currentImage);
            }
        }

        private void AddProductClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // throws an exception if the image doesn't exist
                if (!File.Exists("Images/" + pathBox.Text))
                {
                    throw new FileNotFoundException();
                }
                // instantiate a new product and set it's properties using textbox.text
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
            // throw exception for input other than integers and decimals in the priceBox
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

            // the main grid for editing products
            Grid editProductGrid = new Grid();
            editProductGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            editProductGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            editProductGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            editProductGrid.ColumnDefinitions.Add(new ColumnDefinition());
            editProductGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonClickGrid.Children.Add(editProductGrid);

            TextBlock changeProductsHeader = ShopUtils.CreateTextBlock("Ändra produkt", 16, TextAlignment.Center);
            editProductGrid.Children.Add(changeProductsHeader);
            Grid.SetRow(changeProductsHeader, 0);
            Grid.SetColumnSpan(changeProductsHeader, 2);

            editProductListBox = new ListBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                MaxHeight = 425,
                MaxWidth = 300,
                MinHeight = 300,
                Background = listBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            editProductGrid.Children.Add(editProductListBox);
            Grid.SetRow(editProductListBox, 1);
            Grid.SetColumn(editProductListBox, 0);
            UpdateProductListBox();
            editProductListBox.SelectionChanged += AddSelectedProductToTextBox;

            Button removeProductButton = ShopUtils.CreateButton("Ta bort vald produkt");
            removeProductButton.MaxWidth = 120;
            editProductGrid.Children.Add(removeProductButton);
            Grid.SetRow(removeProductButton, 2);
            Grid.SetColumn(removeProductButton, 0);
            removeProductButton.Click += RemoveProductClick;

            // using a nested grid to keep the columns leveled and since the other columns doesn't need 7 rows
            Grid productPropertiesGrid = new Grid();
            for (int i = 0; i < 7; i++)
            {
                productPropertiesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            productPropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            productPropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition());
            editProductGrid.Children.Add(productPropertiesGrid);
            Grid.SetRow(productPropertiesGrid, 1);
            Grid.SetColumn(productPropertiesGrid, 1);

            // add children productPropertiesGrid
            TextBlock editProductHeader = ShopUtils.CreateTextBlock("Produktens egenskaper", 14, TextAlignment.Center);
            productPropertiesGrid.Children.Add(editProductHeader);

            TextBlock editNameText = ShopUtils.CreateTextBlock("Namn", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editNameText);
            Grid.SetRow(editNameText, 1);
            Grid.SetColumn(editNameText, 0);

            nameBox = ShopUtils.CreateTextBox(textBoxBrush);
            productPropertiesGrid.Children.Add(nameBox);
            nameBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(nameBox, 1);
            Grid.SetColumn(nameBox, 1);

            TextBlock editDescriptionText = ShopUtils.CreateTextBlock("Beskrivning", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editDescriptionText);
            Grid.SetRow(editDescriptionText, 2);
            Grid.SetColumn(editDescriptionText, 0);

            descriptionBox = ShopUtils.CreateTextBox(textBoxBrush);
            productPropertiesGrid.Children.Add(descriptionBox);
            descriptionBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(descriptionBox, 2);
            Grid.SetColumn(descriptionBox, 1);

            TextBlock editPriceText = ShopUtils.CreateTextBlock("Pris", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editPriceText);
            Grid.SetRow(editPriceText, 3);
            Grid.SetColumn(editPriceText, 0);

            priceBox = ShopUtils.CreateTextBox(textBoxBrush);
            productPropertiesGrid.Children.Add(priceBox);
            priceBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(priceBox, 3);
            Grid.SetColumn(priceBox, 1);

            TextBlock editCategoryText = ShopUtils.CreateTextBlock("Kategori", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editCategoryText);
            Grid.SetRow(editCategoryText, 4);
            Grid.SetColumn(editCategoryText, 0);

            categoryBox = ShopUtils.CreateTextBox(textBoxBrush);
            productPropertiesGrid.Children.Add(categoryBox);
            categoryBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(categoryBox, 4);
            Grid.SetColumn(categoryBox, 1);

            TextBlock editPathText = ShopUtils.CreateTextBlock("Bild", 10, TextAlignment.Left);
            productPropertiesGrid.Children.Add(editPathText);
            Grid.SetRow(editPathText, 5);
            Grid.SetColumn(editPathText, 0);

            pathBox = ShopUtils.CreateTextBox(textBoxBrush);
            productPropertiesGrid.Children.Add(pathBox);
            pathBox.HorizontalAlignment = HorizontalAlignment.Left;
            Grid.SetRow(pathBox, 5);
            Grid.SetColumn(pathBox, 1);

            Button saveProductChanges = ShopUtils.CreateButton("Spara ändringar");
            productPropertiesGrid.Children.Add(saveProductChanges);
            Grid.SetRow(saveProductChanges, 6);
            saveProductChanges.Click += SaveProductChangesClick;
        }

        private void RemoveProductClick(object sender, RoutedEventArgs e)
        {
            // to prevent program crash always check if a product has been selected, remove the product from productList using selectedindex in the listbox
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
            // try to set the products properties before serializing the changes
            try
            {
                productList[editProductListBox.SelectedIndex].Title = nameBox.Text;
                productList[editProductListBox.SelectedIndex].Description = descriptionBox.Text;
                productList[editProductListBox.SelectedIndex].Price = decimal.Parse(priceBox.Text);
                productList[editProductListBox.SelectedIndex].Category = categoryBox.Text;
                productList[editProductListBox.SelectedIndex].ProductImage = pathBox.Text;
                UpdateProductListBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Felaktig inmatning: " + ex.Message);
            }
        }

        private void AddSelectedProductToTextBox(object sender, SelectionChangedEventArgs e)
        {
            // to facilitate editing single product properties set the text box content to the selected products properties
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
            // every time changes are made to products, update the listbox in order to display the changes
            editProductListBox.Items.Clear();
            foreach (Product product in productList)
            {
                editProductListBox.Items.Add(product.Title + " (" + product.Price + ") kr");
            }
            //serialize the changes in the productList in order to keep the changes
            productList.Serialize(ShopUtils.GetFilePath("Products.json"));
        }
    }
}