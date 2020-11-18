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
            ShopUtils.CreateFiles();
            Assert.AreEqual(false, Coupon.IsValid(""));
        }

        [TestMethod]
        public void SaveCartTest()
        {
            ShopUtils.CreateFiles();
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
            preSaveCart.Serialize(ShopUtils.GetFilePath("TESTCART.json"));
            List<Product> deserializedCart = ShopUtils.DeserializeProducts(ShopUtils.GetFilePath("TESTCART.json"));

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
                            //If they are not equal we should break out of the loop
                            break;
                        }
                    }
                    //If they are not equal, we break out of the loop and the test will fail.
                    if (!areEqual) break;
                }
            }

            Assert.AreEqual(true, areEqual);
        }

        [TestMethod]
        public void InvalidJsonFormatting()
        {
            ShopUtils.CreateFiles();
            List<Product> products = new List<Product>() { new Product(), new Product(), new Product() };
            string path = ShopUtils.GetFilePath("TESTCART.json");

            Action action = delegate 
            {
                File.WriteAllText(path, "This is not json");
                products = ShopUtils.DeserializeProducts(path);
            };

            Assert.ThrowsException<JsonException>(action);
            //If the exception is thrown, the product count should still remain 3
            Assert.AreEqual(3, products.Count);
        }

        [TestMethod]
        public void DeserializeNonExistentFile()
        {
            ShopUtils.CreateFiles();
            Action action = delegate 
            {
                ShopUtils.DeserializeProducts("null");
            };

            Assert.ThrowsException<FileNotFoundException>(action);
        }

        [TestMethod]
        public void SerializeInvalidDataType()
        {
            ShopUtils.CreateFiles();
            Action action = delegate
            {
                List<string> list = new List<string>()
                {
                    "This",
                    "is",
                    "a",
                    "test"
                };
                list.Serialize("PathDoesNotMatterHere.png");
            };

            Assert.ThrowsException<NotImplementedException>(action);
        }

        [TestMethod]
        public void CorrectlyCreatesFilesOnStartup()
        {
            ShopUtils.CreateFiles();

            Assert.AreEqual(true, File.Exists(ShopUtils.GetFilePath("Cart.json")));
            Assert.AreEqual(true, File.Exists(ShopUtils.GetFilePath("Products.json")));
            Assert.AreEqual(true, File.Exists(ShopUtils.GetFilePath("Coupons.json")));
        }
    }
}