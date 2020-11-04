using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Projektarbete
{
    class Coupon
    {
        public string Code { get; set; }
        public decimal Discount { get; set; }

        //Check if any coupon code in the json matches the user inputted code
        public static bool IsValid(string code)
        {
            return CouponCodes().Any(x => x.Code == code);
        }

        public static Coupon GenerateCoupon(int discount)
        {
            string code = string.Empty;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVXYZ123456789";
            Random r = new Random();

            for(int i = 0; i < 10; i++)
            {
                int random = r.Next(0, chars.Length - 1);
                code += chars[random];
            }

            return new Coupon()
            {
                Code = code,
                Discount = (decimal)discount / 100
            };
        }

        private static List<Coupon> CouponCodes()
        {
            List<Coupon> coupons = new List<Coupon>();
            string path = Path.Combine(Environment.CurrentDirectory, @"JSON\Coupon.json");

            using (StreamReader reader = new StreamReader(path))
            {
                var json = reader.ReadToEnd();

                coupons = JsonSerializer.Deserialize<List<Coupon>>(json);
                reader.Close();
            }
            return coupons;
        }
    }
}
