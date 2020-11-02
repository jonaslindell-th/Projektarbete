using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
class Product
{
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ProductImage { get; set; }
    public int Count { get; set; }
    public string Category { get; set; }


    //public static List<Product> DeserializeProducts()
    //{
    //    List<Product> items = new List<Product>();
    //    string path = "C:/Users/Muste/source/repos/Projektarbete/Projektarbete/Products.json";
    //    using (StreamReader reader = new StreamReader(path))
    //    {
    //        var products = reader.ReadToEnd();

    //        items = JsonSerializer.Deserialize<List<Product>>(products);
    //        reader.Close();
    //    }

    //    return items;
    //}
}
