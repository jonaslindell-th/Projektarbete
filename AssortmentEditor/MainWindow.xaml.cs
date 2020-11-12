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

namespace AssortmentEditor
{
    public partial class MainWindow : Window
    {
        DataGrid productDataGrid;

        TextBox pathBox;

        List<Projektarbete.Product> productList;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            productList = Projektarbete.ShopUtils.DeserializeProducts(@"JSON\Products.json");
            List<Projektarbete.Coupon> couponList = Projektarbete.Coupon.CouponCodes();


            #region Custom brushes
            // declare a brushconverter to convert a hex color code string to a Brush color
            BrushConverter brushConverter = new System.Windows.Media.BrushConverter();
            Brush backgroundBrush = (Brush)brushConverter.ConvertFromString("#2F3136");
            Brush listBoxBrush = (Brush)brushConverter.ConvertFromString("#36393F");
            Brush textBoxBrush = (Brush)brushConverter.ConvertFromString("#40444B");
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
            Grid mainGrid = new Grid();
            root.Content = mainGrid;
            //mainGrid.Margin = new Thickness(5);
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.Background = backgroundBrush;
            mainGrid.ShowGridLines = true;

            Grid assortmentGrid = new Grid();
            assortmentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            assortmentGrid.RowDefinitions.Add(new RowDefinition());
            assortmentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            assortmentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            assortmentGrid.RowDefinitions.Add(new RowDefinition());
            assortmentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            assortmentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.Children.Add(assortmentGrid);
            Grid.SetRow(assortmentGrid, 0);

            TextBlock productHeader = Projektarbete.ShopUtils.CreateTextBlock("Produktsortiment", 18, TextAlignment.Center, assortmentGrid, 0, 0, 2);

            productDataGrid = new DataGrid
            {
                MaxColumnWidth = 270,
                MaxHeight = 200,
                MaxWidth = 710,
                Margin = new Thickness(10, 5, 10, 0)
            };
            productDataGrid.ItemsSource = productList;
            assortmentGrid.Children.Add(productDataGrid);
            Grid.SetRow(productDataGrid, 1);
            productDataGrid.IsReadOnly = true;


            Button saveProductAssortment = new Button
            {
                Content = "Spara produktsortiment",
                Margin = new Thickness(10, 5, 10, 40),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold
            };
            assortmentGrid.Children.Add(saveProductAssortment);
            Grid.SetRow(saveProductAssortment, 2);
            saveProductAssortment.Click += SaveProductAssortment;

            TextBlock couponHeader = Projektarbete.ShopUtils.CreateTextBlock("Kuponger", 18, TextAlignment.Center, assortmentGrid, 3, 0, 2);

            DataGrid couponDataGrid = new DataGrid
            {
                MaxColumnWidth = 290,
                MaxHeight = 200,
                MaxWidth = 180,
                Margin = new Thickness(10, 5, 10, 0)
            };
            assortmentGrid.Children.Add(couponDataGrid);
            couponDataGrid.ItemsSource = couponList;
            Grid.SetRow(couponDataGrid, 4);

            Button saveCouponChanges = new Button
            {
                Content = "Spara kuponger",
                Margin = new Thickness(10, 5, 10, 30),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                MaxWidth = 180
            };
            assortmentGrid.Children.Add(saveCouponChanges);
            Grid.SetRow(saveCouponChanges, 5);
            saveCouponChanges.Click += SaveCouponChanges;

            Grid imageGrid = new Grid();
            imageGrid.ColumnDefinitions.Add(new ColumnDefinition());
            imageGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            imageGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            imageGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            imageGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.Children.Add(imageGrid);
            Grid.SetColumn(imageGrid, 1);

            TextBlock imageHeader = Projektarbete.ShopUtils.CreateTextBlock("Välj bland befintliga bilder", 18, TextAlignment.Center, imageGrid, 0, 0, 1);

            TextBlock pathHeader = Projektarbete.ShopUtils.CreateTextBlock("Bildens sökväg", 14, TextAlignment.Center, imageGrid, 1, 0, 1);

            pathBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = textBoxBrush,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                MaxWidth = 200
            };
            imageGrid.Children.Add(pathBox);
            Grid.SetRow(pathBox, 2);

            //productDataGrid.SelectedCellsChanged += DisplayImage;

        }

        private void DisplayImage(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void SaveCouponChanges(object sender, RoutedEventArgs e)
        {

        }

        private void SaveProductAssortment(object sender, RoutedEventArgs e)
        {

        }
    }
}
