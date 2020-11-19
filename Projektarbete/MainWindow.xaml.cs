using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data;
using System.IO;

namespace Projektarbete
{
    public partial class MainWindow : Window
    {
        private List<Product> productList;
        private List<Product> shoppingCart;
        private List<Product> searchTermList = new List<Product>();
        private List<string> categoryList = new List<string>();
        private List<Coupon> couponList;

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
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            ShopUtils.CreateFiles();

            productList = ShopUtils.DeserializeProducts(ShopUtils.GetFilePath("Products.json"));
            shoppingCart = ShopUtils.DeserializeProducts(ShopUtils.GetFilePath("Cart.json"));
            couponList = Coupon.DeserializeCoupons();

            #region Custom brushes
            // declare a brushconverter to convert a hex color code string to a Brush color
            BrushConverter brushConverter = new System.Windows.Media.BrushConverter();
            Brush backgroundBrush = (Brush)brushConverter.ConvertFromString("#2F3136");
            Brush listBoxBrush = (Brush)brushConverter.ConvertFromString("#36393F");
            Brush textBoxBrush = (Brush)brushConverter.ConvertFromString("#40444B");
            Brush expanderBrush = (Brush)brushConverter.ConvertFromString("#202225");
            #endregion

            // Set Window properties
            Title = "Butiken";
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

            // Main grid definition
            Grid mainGrid = new Grid();
            root.Content = mainGrid;
            mainGrid.RowDefinitions.Add(new RowDefinition());
            // First column contains the shoppingcart and product assortment.
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            // The second column displays the selected product and upon payment displays the receipt
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.Background = backgroundBrush;

            #region grid definiton for the Expander in leftGrid
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

            TextBlock cartTextBlock = ShopUtils.CreateTextBlock("Varukorg", 18, TextAlignment.Center);
            expanderCartGrid.Children.Add(cartTextBlock);
            Grid.SetRow(cartTextBlock, 0);
            Grid.SetColumn(cartTextBlock, 0);
            Grid.SetColumnSpan(cartTextBlock, 2);

            TextBlock discountTextBlock = ShopUtils.CreateTextBlock("Mata in rabattkod nedan", 12, TextAlignment.Center);
            expanderCartGrid.Children.Add(discountTextBlock);
            Grid.SetRow(discountTextBlock, 1);
            Grid.SetColumn(discountTextBlock, 0);
            Grid.SetColumnSpan(discountTextBlock, 1);

            // A combobox to display available coupons to the user
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
            // add a default index at 0, can be selected to clear the couponTextBox.Text
            couponComboBox.Items.Add("Dina rabattkoder");
            couponComboBox.SelectedIndex = 0;
            couponComboBox.SelectionChanged += AddToCouponTextBox;
            // Adds all available coupons to the couponComboBox items from the couponList which recieves predetermined coupons from file
            foreach (var coupon in couponList)
            {
                couponComboBox.Items.Add(coupon.Code + " " + (100 - Math.Round(coupon.Discount * 100, 0)) + "%");
            }

            // A textbox for the user to enter coupon codes
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

            ShopUtils.CreateButton("Använd rabattkod", expanderCartGrid, 2, 2, 1, ValidateCoupon);
            ShopUtils.CreateButton("Rensa varukorg", expanderCartGrid, row: 3, column: 0, columnspan: 1, ClearCartClick);
            ShopUtils.CreateButton("Spara varukorg", expanderCartGrid, 3, 1, 1, SaveCartClick);
            ShopUtils.CreateButton("Ta bort en vald produkt", expanderCartGrid, row: 4, column: 0, columnspan: 1, RemoveProductClick);
            ShopUtils.CreateButton("Ta bort varje vald produkt", expanderCartGrid, 4, 1, 1, RemoveAllSelectedProductsClick);

            // the cartListBox display all products in the shoppingCart list
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

            sumTextBlock = ShopUtils.CreateTextBlock("Varukorgens summa: 0 kr", 12, TextAlignment.Left);
            expanderCartGrid.Children.Add(sumTextBlock);
            Grid.SetRow(sumTextBlock, 6);
            Grid.SetColumn(sumTextBlock, 0);
            Grid.SetColumnSpan(sumTextBlock, 1);

            ShopUtils.CreateButton("Till kassan", expanderCartGrid, 6, 1, 1, ShowReceipt);
            #endregion

            #region leftGrid definition
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

            // Expander definition, when expanded the expanderCartGrid will be displayed in the leftGrid
            cartExpander = new Expander
            {
                // sets the expanders content to the expanderCartGrid defined above
                Content = expanderCartGrid,
                Header = "Din varukorg 0 kr",
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.White,
                Background = expanderBrush
            };
            leftGrid.Children.Add(cartExpander);
            Grid.SetRow(cartExpander, 0);
            Grid.SetColumn(cartExpander, 0);
            // when expanded the cartExpander's columnspan increases to take up two columns and when collapsed shrinks to one column
            cartExpander.Collapsed += DecreaseCartColumnSpan;
            cartExpander.Expanded += IncreaseCartColumnSpan;
            // Update the cartListBox in the Expander to add items from the shoppingCart list
            UpdateCartListBox();

            TextBlock products = ShopUtils.CreateTextBlock("Produkter", 18, TextAlignment.Center);
            leftGrid.Children.Add(products);
            Grid.SetRow(products, 1);
            Grid.SetColumn(products, 0);
            Grid.SetColumnSpan(products, 2);

            TextBlock searchHeading = ShopUtils.CreateTextBlock("Sök efter produkt", 12, TextAlignment.Center);
            leftGrid.Children.Add(searchHeading);
            Grid.SetRow(searchHeading, 2);
            Grid.SetColumn(searchHeading, 0);
            Grid.SetColumnSpan(searchHeading, 2);

            // A textbox definition where the user can input a search term
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
            // when the searchbox text changes the ShowSearch event will run
            searchBox.TextChanged += ShowSearch;

            // A combobox which displays available categories
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
            categoryList = ShopUtils.GenerateCategories(productList);
            // adds categories to the categoryBox
            foreach (string category in categoryList)
            {
                categoryBox.Items.Add(category);
            }
            categoryBox.SelectionChanged += ShowCategory;

            ShopUtils.CreateButton("Lägg till vald produkt", leftGrid, row: 5, column: 0, columnspan: 2, AddProductToCart);

            // Listbox definition, used to display the product assortment from the searchTermList
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
            // set the searchTerm to empty string in order to add every product from the productList upon start
            UpdateProductListBox("");
            // selecting an item in the listbox will display productinformation in the stackpanel to the right
            productListBox.SelectionChanged += DisplaySelectedProduct;
            #endregion

            #region rightStackPanel definition
            StackPanel rightStackPanel = new StackPanel();
            mainGrid.Children.Add(rightStackPanel);
            Grid.SetRow(rightStackPanel, 0);
            Grid.SetColumn(rightStackPanel, 1);

            productHeading = ShopUtils.CreateTextBlock("Välj produkt", 18, TextAlignment.Center, rightStackPanel);
            productHeading.Margin = new Thickness(5, 30, 5, 32);

            imageGrid = new Grid();
            rightStackPanel.Children.Add(imageGrid);

            // sets a startup image
            currentImage = ShopUtils.CreateImage("Images/Ica.png", false);
            imageGrid.Children.Add(currentImage);

            productDescriptionHeading = ShopUtils.CreateTextBlock("", 16, TextAlignment.Center, rightStackPanel);
            productDescription = ShopUtils.CreateTextBlock("", 12, TextAlignment.Center, rightStackPanel);
            productDescription.FontWeight = FontWeights.Thin;
            productDescription.Margin = new Thickness(30, 5, 30, 5);
            #endregion
        }

        private void SaveCartClick(object sender, RoutedEventArgs e)
        {
            string path = ShopUtils.GetFilePath("Cart.json");
            shoppingCart.Serialize(path);
        }

        private void ShowReceipt(object sender, RoutedEventArgs e)
        {
            productHeading.Text = "Ditt kvitto";
            productDescriptionHeading.Text = "Ditt köp har genomförts";
            productDescription.Text = "Tack för att du handlar hos oss!";
            // without clearing the imageGrid a picture would still be visible behind the datagrid
            imageGrid.Children.Clear();

            decimal sum = 0;

            // DataGrid definition
            DataGrid receiptDataGrid = new DataGrid
            {
                Margin = new Thickness(5),
                MaxWidth = 450,
                MinHeight = 300,
                MaxHeight = 300,
                BorderThickness = new Thickness(0)
            };
            // prevents the user from altering the receipt
            receiptDataGrid.IsReadOnly = true;

            // DataTable definition, which is the DataGrid's itemsource
            DataTable receiptTable = new DataTable();
            // add columns for the receipt datatable
            receiptTable.Columns.Add(new DataColumn("Produkt", typeof(string)));
            receiptTable.Columns.Add(new DataColumn("Antal", typeof(int)));
            receiptTable.Columns.Add(new DataColumn("aPris", typeof(decimal)));
            receiptTable.Columns.Add(new DataColumn("Rabatt", typeof(decimal)));
            receiptTable.Columns.Add(new DataColumn("Summa", typeof(decimal)));

            //if a discount code has been validated the loop will add a product to a new row for each product in the shoppingCart list displaying the total discount for each product and summarize the total discount and price
            if (hasDiscount)
            {
                foreach (var product in shoppingCart)
                {
                    decimal discount = product.Price - (product.Price * currentCoupon.Discount);

                    receiptTable.Rows.Add(new object[]
                    {
                    product.Title,
                    product.Count,
                    product.Price,
                    Math.Round(-discount*product.Count, 2),
                    Math.Round(product.Price*product.Count*currentCoupon.Discount, 2)
                    });
                    sum += product.Price * product.Count;
                }
                // adds a bottom row containing total discount and total price with discount
                receiptTable.Rows.Add(new object[] { "Totalt", null, null, Math.Round(-(sum - (sum * currentCoupon.Discount)), 2), Math.Round((sum * currentCoupon.Discount), 2) });
            }
            //if no discount has been validated, the else statement set the discount values to null and present the products by adding a new row foreach product in the shoppingCart list
            else
            {
                foreach (var product in shoppingCart)
                {
                    receiptTable.Rows.Add(new object[]
                    {
                    product.Title,
                    product.Count,
                    product.Price,
                    null,
                    Math.Round(product.Price*product.Count, 2)
                    });
                    sum += product.Price * product.Count;
                }
                // adds a bottom row containing the total price
                receiptTable.Rows.Add(new object[] { "Totalt", null, null, null, Math.Round(sum, 2) });
            }
            // sets the datagrids content to the datatable containing the products
            receiptDataGrid.ItemsSource = receiptTable.DefaultView;
            // displays the datagrid in the imageGrid in the StackPanel
            imageGrid.Children.Add(receiptDataGrid);
            Grid.SetRow(receiptDataGrid, 0);
            Grid.SetColumn(receiptDataGrid, 0);

            // reset the shopping cart
            shoppingCart.Clear();
            string path = ShopUtils.GetFilePath("Cart.json");
            shoppingCart.Serialize(path);
            currentCoupon = null;
            hasDiscount = false;
            couponComboBox.SelectedIndex = 0;
            UpdateCartListBox();
        }

        private void AddToCouponTextBox(object sender, SelectionChangedEventArgs e)
        {
            if (couponComboBox.SelectedIndex != 0)
            {
                // using selectedindex - 1 since there is a default item "Dina rabattkoder" added at index 0 
                couponTextBox.Text = couponList[couponComboBox.SelectedIndex - 1].Code;
            }
            else
            {
                couponTextBox.Text = "";
            }
        }

        private void IncreaseCartColumnSpan(object sender, RoutedEventArgs e)
        {
            Grid.SetColumnSpan(cartExpander, 2);
        }

        private void DecreaseCartColumnSpan(object sender, RoutedEventArgs e)
        {
            Grid.SetColumnSpan(cartExpander, 1);
        }

        private void ShowCategory(object sender, SelectionChangedEventArgs e)
        {
            searchTermList.Clear();
            if (categoryBox.SelectedIndex != 0 && categoryBox != null)
            {
                string category = categoryList[categoryBox.SelectedIndex - 1];
                var searchTermProducts = productList.Where(product => product.Category == category);
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
            if (validCoupon && currentCoupon != null)
            {
                Coupon compare = Coupon.DeserializeCoupons().First(x => x.Code == input);
                if (compare.Discount < currentCoupon.Discount)
                {
                    currentCoupon = compare;
                }
                else
                {
                    MessageBox.Show("Din nuvarande rabattkod har en bättre rabatt än den du nyligen matade in!");
                    return;
                }

            }

            currentCoupon = validCoupon ? Coupon.DeserializeCoupons().First(x => x.Code == input) : null;

            hasDiscount = currentCoupon != null;

            if (currentCoupon == null)
            {
                MessageBox.Show("Den inmatade rabattkoden är ej giltig, försök igen.");
                return;
            }
            else
            {
                MessageBox.Show("Du har aktiverat en giltig rabattkod.");
                UpdateCartListBox();
            }
        }

        private void DisplaySelectedProduct(object sender, SelectionChangedEventArgs e)
        {
            if (productListBox.SelectedIndex != -1)
            {
                //change the heading, image, descriptionheading, description by referring to the selectedindex in the searchTermList
                productHeading.Text = searchTermList[productListBox.SelectedIndex].Title;
                // clear the imageGrid children to prevent images from stacking on top of each other
                imageGrid.Children.Clear();
                currentImage = ShopUtils.CreateImage("Images/" + searchTermList[productListBox.SelectedIndex].ProductImage, true);
                imageGrid.Children.Add(currentImage);
                productDescriptionHeading.Text = "Produktbeskrivning";
                productDescription.Text = searchTermList[productListBox.SelectedIndex].Description;
            }
        }

        private void RemoveProductClick(object sender, RoutedEventArgs e)
        {
            if (cartListBox.SelectedIndex != -1 && shoppingCart[cartListBox.SelectedIndex].Count > 1)
            {
                shoppingCart[cartListBox.SelectedIndex].Count--;
                UpdateCartListBox();
                return;
            }
            else if (cartListBox.SelectedIndex != -1 && shoppingCart[cartListBox.SelectedIndex].Count == 1)
            {
                shoppingCart.RemoveAt(cartListBox.SelectedIndex);
                UpdateCartListBox();
                return;
            }
            MessageBox.Show("Välj en produkt att ta bort");
        }

        private void RemoveAllSelectedProductsClick(object sender, RoutedEventArgs e)
        {
            if (cartListBox.SelectedIndex != -1)
            {
                shoppingCart.RemoveAt(cartListBox.SelectedIndex);
                UpdateCartListBox();
                return;
            }
            MessageBox.Show("Välj en produkt att ta bort");
        }

        private void ClearCartClick(object sender, RoutedEventArgs e)
        {
            shoppingCart.Clear();
            UpdateCartListBox();
        }

        private void AddProductToCart(object sender, RoutedEventArgs e)
        {
            // check if a product is selected using an if statement to prevent the program from crashing
            if (productListBox.SelectedIndex != -1)
            {
                // check if shoppingCart already contains the product in order to set the Product.Count property
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
                UpdateCartListBox();
                // add return to avoid displaying the messagebox when a product is added
                return;
            }
            // if no product has been selected a messagebox is displayed
            MessageBox.Show("Ingen produkt vald");
        }

        private void UpdateCartListBox()
        {
            decimal sum = 0;
            // in order to display the added product in the shoppingCart the cartListBox needs to be cleared to avoid displaying the previous products in the shoppinCart and the new products
            cartListBox.Items.Clear();
            // adds the products from the shoppingCart to the items in cartListBox
            foreach (Product product in shoppingCart)
            {
                cartListBox.Items.Add(product.Title + " (" + product.Price + ") kr" + " (" + product.Count + ")");
            }
            // calculates the shoppingCarts total sum
            foreach (Product product in shoppingCart)
            {
                sum += product.Price * product.Count;
            }
            // if a coupon has been validated calculate the sum with a discount
            if (hasDiscount) sum *= currentCoupon.Discount;

            sumTextBlock.Text = "Varukorgens summa: " + (Convert.ToString(Math.Round(sum, 1))) + " kr";
            cartExpander.Header = "Din varukorg " + (Convert.ToString(Math.Round(sum, 1))) + " kr";
        }

        private void UpdateProductListBox(string searchTerm)
        {
            //clear the searchTermList and add products where the product title contains the search term
            searchTermList.Clear();

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
    }
}