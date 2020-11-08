using Microsoft.VisualStudio.TestTools.UnitTesting;
using Projektarbete;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

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
            List<Product> preSaveCart = new List<Product>()
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
                },
                new Product()
                {
                    Title = "Päron",
                    Category = "Mat",
                    Count = 4541354,
                    Price = 0.99M,
                    Description = "Lila Päron"
                }
            };
            Product.SaveCart(preSaveCart);
            List<Product> deserializedCart = Product.LoadCart();
                
            Assert.AreEqual(preSaveCart.Count, deserializedCart.Count);
        }
    }
}