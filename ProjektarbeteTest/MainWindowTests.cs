using Microsoft.VisualStudio.TestTools.UnitTesting;
using Projektarbete;
using System.Collections.Generic;

namespace ProjektarbeteTest
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void InvalidCouponTest()
        {
            Assert.AreEqual(false, Coupon.IsValid(""));
        }
        [TestMethod()]
        public void SaveCartTest()
        {
            List<Product> p = new List<Product>()
            {
                new Product()
                {
                    Title = "Äpple",
                    Category = "Mat",
                    Count = 14,
                    Price = 4.99M,
                    Description = "Rött äpple"
                },
                new Product()
                {
                    Title = "Banan",
                    Category = "Mat",
                    Count = 4,
                    Price = 9.99M,
                    Description = "Guleböj"
                }
            };
            Product.SaveCart(p);

            List<Product> deserializedCart = Product.LoadCart();

            Assert.AreEqual(p.Count, deserializedCart.Count);
        }
    }
}