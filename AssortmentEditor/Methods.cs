using System;
using System.Collections.Generic;
using System.Text;
using Projektarbete;

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

        public static Coupon ModifyCoupon(Coupon coupon)
        {
            return null;
        }

        public static void Exempel()
        {
            new Product()
            {
                Title = "Test",
                Price = 5.0M,
                Description = "Test produkt",
                Count = int.MaxValue - 1,
                Category = "Test"
            }.AddProduct();

            //vs.

            Product p = new Product()
            {
                Title = "Test",
                Price = 5.0M,
                Description = "Test produkt",
                Count = int.MaxValue - 1,
                Category = "Test"
            };

            Methods.AddProduct(p);
        }
    }
}