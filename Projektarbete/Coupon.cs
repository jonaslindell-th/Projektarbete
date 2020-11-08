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
    public class Coupon
    {
        public string Code { get; set; }
        public decimal Discount { get; set; }

        //Check if any coupon code in the json matches the user inputted code
        public static bool IsValid(string code)
        {
            return CouponCodes().Any(x => x.Code == code);
        }

        //We'll use this to generate a coupon for the user (After they input the discount percentage)
        public static Coupon GenerateCoupon(int discount)
        {
            string code = string.Empty;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVXYZ123456789";
            Random r = new Random();

            //generate an uppercase alphanumeric code of length 10 that'll be useable as a coupon code
            for(int i = 0; i < 10; i++)
            {
                int random = r.Next(0, chars.Length - 1);
                code += chars[random];
            }

            return new Coupon()
            {
                Code = code,
                Discount = 1 - ((decimal)discount / 100) //discount is an int so the user can input "70" for 70% (more user friendly than inputting 0.7)
            };
        }

        public static List<Coupon> CouponCodes()
        {
            List<Coupon> coupons = new List<Coupon>();
            string path = Path.Combine(Environment.CurrentDirectory, @"JSON\Coupons.json");

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
