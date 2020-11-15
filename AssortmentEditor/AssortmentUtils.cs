using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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
    }
}
