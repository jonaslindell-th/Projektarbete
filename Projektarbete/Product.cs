using System.Collections.Generic;
using System.IO;
using System.Text.Json;
namespace Projektarbete
{
    public class Product
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ProductImage { get; set; }
        public int Count { get; set; }
        public string Category { get; set; }


        public static string GetFilePath(string fileName)
        {
            return $@"C:\Windows\Temp\{fileName}";
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
                List<Product> items = Product.DeserializeProducts(GetFilePath("Cart.json"));
                return items;
            }
            catch (JsonException) { return new List<Product>(); }
        }
    }
}
