using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AssortmentEditor
{
    class AssortmentUtils
    {
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
    }
}
