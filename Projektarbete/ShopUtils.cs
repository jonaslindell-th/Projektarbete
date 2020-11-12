using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Text.Json;

namespace Projektarbete
{
    public static class ShopUtils
    {
        public static string GetFilePath(string fileName) => $@"C:\Windows\Temp\{fileName}";

        public static void CreateButton(string content, Grid grid, int row, int column, int columnspan, RoutedEventHandler onClick)
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

        public static TextBlock CreateTextBlock(string text, int fontSize, TextAlignment alignment)
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
            return textBlock;
        }

        public static TextBlock CreateTextBlock(string text, int fontSize, TextAlignment alignment, StackPanel grid)
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

        public static List<Product> DeserializeProducts(string path)
        {
            if (!File.Exists(path)) File.Create(path).Close();

            List<Product> items = new List<Product>();
            using (StreamReader reader = new StreamReader(path))
            {
                var products = reader.ReadToEnd();

                items = JsonSerializer.Deserialize<List<Product>>(products);
                reader.Close();
            }
            return items;
        }

        public static List<string> GenerateCategories(List<Product> products)
        {
            List<string> categories = new List<string>();
            //Loop through products in the productList, if the categoryList does not already contain the product category it gets added to the categoryList
            foreach (Product product in products)
            {
                if (!categories.Contains(product.Category))
                {
                    categories.Add(product.Category);
                }
            }
            return categories;
        }

        public static void SaveCart(List<Product> cart)
        {
            var json = JsonSerializer.Serialize(cart,
                new JsonSerializerOptions()
                {
                    WriteIndented = true
                });

            string path = GetFilePath("Cart.json");
            File.WriteAllText(path, json);
        }

        public static List<Product> LoadCart()
        {
            try
            {
                List<Product> items = DeserializeProducts(GetFilePath("Cart.json"));
                return items;
            }
            catch (JsonException) { return new List<Product>(); }
        }
    }
}
