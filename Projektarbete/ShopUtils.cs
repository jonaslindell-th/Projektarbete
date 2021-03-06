﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Media.Imaging;

namespace Projektarbete
{
    public static class ShopUtils
    {
        public static string GetFilePath(string fileName) => $@"C:\Windows\Temp\Sebastian_Jonas\{fileName}";

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
        public static Button CreateButton(string content)
        {
            Button button = new Button
            {
                Content = content,
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
            };
            return button;
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
        public static TextBox CreateTextBox(Brush background)
        {
            TextBox textBox = new TextBox
            {
                Margin = new Thickness(5),
                Background = background,
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                FontWeight = FontWeights.SemiBold,
                Width = 200
            };
            return textBox;
        }

        public static Image CreateImage(string filePath, bool maxwidth)
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
            if (maxwidth)
            {
                image.MaxWidth = 300;
            }
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            return image;
        }

        public static List<Product> DeserializeProducts(string path)
        {
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

        public static void Serialize<T>(this List<T> data, string serializePath)
        {
            Type dataType = data.GetType().GetGenericArguments().Single();
            //Serializing other list types is no problem, though we only want to serialize lists of products or coupons.
            //A secondary option would be to make method overloads for the specific lists we want to use
            if (dataType != typeof(Product) && dataType != typeof(Coupon))
            {
                throw new NotImplementedException("This method should only serialize Products or Coupons.");
            }

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions() { WriteIndented = true });

            File.WriteAllText(serializePath, json);
        }

        public static void CreateFiles()
        {
            if (!Directory.Exists($@"C:\Windows\Temp\Sebastian_Jonas"))
            {
                Directory.CreateDirectory(@"C:\Windows\Temp\Sebastian_Jonas");
                Product.CreateProductFile();
                Coupon.CreateCouponFile();
            }
        }
    }
}
