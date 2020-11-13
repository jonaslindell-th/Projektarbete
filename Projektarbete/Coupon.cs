using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Projektarbete
{
    public class Coupon
    {
        public string Code { get; set; }
        public decimal Discount { get; set; }

        //Check if any coupon code in the json matches the user inputted code
        public static bool IsValid(string code)
        {
            return DeserializeCoupons().Any(x => x.Code == code);
        }

        public static List<Coupon> DeserializeCoupons()
        {
            List<Coupon> coupons = new List<Coupon>();
            string path = ShopUtils.GetFilePath("Coupons.json");

            using (StreamReader reader = new StreamReader(path))
            {
                var json = reader.ReadToEnd();

                coupons = JsonSerializer.Deserialize<List<Coupon>>(json);
                reader.Close();
            }
            return coupons;
        }

        public static void CreateCouponFile()
        {
            List<Coupon> coupons = new List<Coupon>()
            {
                new Coupon()
                {
                    Code = "ABCDE12345",
                    Discount = 0.7M
                },
                new Coupon()
                {
                    Code = "XYZ321",
                    Discount = 0.3M
                }
            };
            coupons.Serialize(ShopUtils.GetFilePath("Coupons.json"));
        }
    }
}
