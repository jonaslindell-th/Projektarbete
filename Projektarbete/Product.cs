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
    }
}
