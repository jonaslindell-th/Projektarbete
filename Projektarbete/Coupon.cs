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
            return CouponCodes().Any(x => x.Code == code);
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
