using Microsoft.VisualStudio.TestTools.UnitTesting;
using Projektarbete;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Text.Json;
using System;

namespace ProjektarbeteTest
{
    [TestClass]
    public class MainWindowTests
    {
        [TestMethod]
        public void InvalidCouponTest()
        {
            Assert.AreEqual(false, Coupon.IsValid(""));
        }
        [TestMethod]
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
                    Description = "Rött äpple",
                    ProductImage = ""
                },
                new Product()
                {
                    Title = "Banan",
                    Category = "Mat",
                    Count = 4,
                    Price = 9.99M,
                    Description = "Guleböj",
                    ProductImage = ""
                },
                new Product()
                {
                    Title = "Päron",
                    Category = "Mat",
                    Count = 4541354,
                    Price = 0.99M,
                    Description = "Lila Päron",
                    ProductImage = ""
                }
            };
            ShopUtils.SaveCart(preSaveCart);
            List<Product> deserializedCart = ShopUtils.LoadCart();

            bool areEqual = deserializedCart.Count == preSaveCart.Count;

            if (areEqual)
            {
                for (int i = 0; i < preSaveCart.Count; i++)
                {
                    foreach(PropertyInfo p in preSaveCart[i].GetType().GetProperties())
                    {
                        if(p.GetValue(preSaveCart[i]).ToString() != p.GetValue(deserializedCart[i]).ToString())
                        {
                            areEqual = false;
                            break;
                        }
                    }
                    if (!areEqual) break;
                }
            }

            Assert.AreEqual(true, areEqual);
        }
        [TestMethod]
        public void InvalidJsonFormatting()
        {
            List<Product> products = new List<Product>() { new Product(), new Product(), new Product() };
            string path = ShopUtils.GetFilePath("Cart.json");

            Action action = delegate 
            {
                File.WriteAllText(path, "This is not json");
                products = ShopUtils.DeserializeProducts(path);
            };

            Assert.ThrowsException<JsonException>(action);
            Assert.AreEqual(3, products.Count);
        }


    }
}