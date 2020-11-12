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
        List<Projektarbete.Product> productList = Projektarbete.ShopUtils.DeserializeProducts(@"JSON\Products.json");
        List<Projektarbete.Coupon> couponList = Projektarbete.Coupon.CouponCodes();

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

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
            Grid selectionGrid = new Grid();
            root.Content = selectionGrid;
            //mainGrid.Margin = new Thickness(5);
            selectionGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            selectionGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            selectionGrid.RowDefinitions.Add(new RowDefinition ());
            selectionGrid.ColumnDefinitions.Add(new ColumnDefinition());
            selectionGrid.Background = backgroundBrush;
            selectionGrid.ShowGridLines = true;

            TextBlock headingTextBlock = Projektarbete.ShopUtils.CreateTextBlock("Sortiment hanteraren", 18, TextAlignment.Center);
            selectionGrid.Children.Add(headingTextBlock);
            Grid.SetRow(headingTextBlock, 0);
            Grid.SetColumnSpan(headingTextBlock, 1);
        }
    }
}