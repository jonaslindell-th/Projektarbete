using System;
using System.Collections.Generic;
using System.Text;
using Projektarbete;
using System.IO;
using System.Text.Json;

namespace AdminShop
{
    //Temporärt namn - vi byter till nåt rimligare senare, tänkte bara ha det så här för tillfället
    public static class Methods
    {
        /* 
         * Använder method extensions här
         * Vilket innebär att istället för att kalla på dessa metoder genom Methods.AddProduct(product)
         * så kan vi kalla på dem via våran product variabel
         * ex: product.AddProduct(); <-- så läggs den till i produktutbudet
         */
        public static void AddProduct(this Product product)
        {

        }

        public static void RemoveProduct(this Product product)
        {

        }

        public static void ModifyProduct(Product product)
        {

        }

        public static void AddCoupon(this Coupon coupon)
        {

        }

        public static void RemoveCoupon(this Coupon coupon)
        {

        }

        /// <summary>
        /// Serializes the data from the calling list instance to the specified file path in parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The file path that this instance should be serialized to.</param>
        public static void Serialize<T>(this List<T> data, string path)
        {
            using(StreamWriter stream = new StreamWriter(path))
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions()
                {
                    WriteIndented = true,
                });
                stream.Write(json);
                stream.Close();
            }
        }

        [Obsolete("Ta bort denna innan vi skickar in det slutliga projektet :).")]
        public static void SerializeExample()
        {
            List<Product> products = new List<Product>()
            {
                new Product()
                {
                    //Product properties
                },
                new Product()
                {
                    //Product properties
                }
            };
            products.Serialize(@"JSON\Products.json");

            List<Coupon> coupons = new List<Coupon>()
            {
                new Coupon()
                {
                    //Coupon properties
                },
                new Coupon()
                {
                    //Coupon properties
                }
            };
            coupons.Serialize(@"JSON\Coupons.json");
        }
    }
}