﻿using System.Collections.Generic;
using System.IO;
using System.Text.Json;
namespace Projektarbete
{
    public class Product
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ProductImage { get; set; }
        public int Count { get; set; }
        public string Category { get; set; }

        public static void CreateProductFile()
        {
            List<Product> products = new List<Product>()
            {
                new Product()
                {
                    Title = "Äpple Royal Gala ca 150g",
                    Description = "Äpple med söt frisk smak och vitt fruktkött. Prova i sallad, kakor, desserter eller ät det precis som det är. Passar som mellis och i utflyktskorgen.",
                    Price = 4.36M,
                    Category ="Frukt och grönt",
                    ProductImage= "Apple.jpg"
                },
                new Product()
                {
                    Title = "Banan ca 180g",
                    Description = "Smart mellanmål som blir sötare ju mognare den är. Annars är bananen också god att tillaga, t ex stekt i smör, sirap och kardemumma med glass till.",
                    Price = 18.95M,
                    Category = "Frukt och grönt",
                    ProductImage = "Banan.jpg"
                },
                new Product()
                {
                    Title = "Ananas extra sweet ca 950g",
                    Description = "Ursprungsland: Costa Rica",
                    Price = 23.95M,
                    Category = "Frukt och grönt",
                    ProductImage = "Ananas.jpg",
                },
                new Product()
                {
                    Title = "Revben",
                    Description = "ICA Revbensspjäll Tjocka är matiga spjäll som går att koka, grilla eller steka i ugn. Du kan marinera dem upp till ett dygn i kylskåp och sedan ugnssteka på 175 grader i cirka 1-1,5 tim tills innertemperaturen når cirka 85 grader. Du kan också koka dem först, sedan marinera över natten och slutligen ugnssteka dem. En snabbvariant är också att tillaga dem som de är i cirka 1 timme och sen pensla på glaze och in i ugnen igen i cirka 10 min för lite fin yta. Och det viktigaste: köttet kommer från utvalda svenska gårdar.",
                    Price = 77.95M,
                    Category = "Kött, fågel & fisk",
                    ProductImage = "Revben.jpg",
                },
                new Product()
                {
                    Title = "Bacon",
                    Description = "INGREDIENSER: Svenskt sidfläsk (96%), vatten, salt, antioxidationsmedel (E301), konserveringsmedel (E250), rökarom. Bacon är en rå produkt som ska tillagas före användning.",
                    Price = 14.95M,
                    Category = "Kött, fågel & fisk",
                    ProductImage = "Bacon.jpg"
                },
                new Product()
                {
                    Title = "Herrgård mild 28% ca 667g",
                    Description = "Energi (kcal) 360 kcal, Energi (kJ) 1493 kJ, Fett 28 g, Varav mättat fett 18 g, Kolhydrater 0 g, Varav socker 0 g, Protein 26 g, Salt 1.3 g, Vitamin B12 1.5 µg, Kalcium 774 mg",
                    Price = 55.99M,
                    Category = "Mejeri",
                    ProductImage = "Cheese.jpg",
                },
                new Product()
                {
                    Title = "Cottage cheese Naturell 4% 500g Keso",
                    Description = "pastöriserad MJÖLK, salt, modifierad stärkelse, VASSLEPERMEAT, syrningskultur, ystenzym.",
                    Price = 24.99M,
                    Category = "Mejeri",
                    ProductImage = "Keso.jpg",
                },
                new Product()
                {
                    Title = "Mellanmjölk 1.5%",
                    Description = "ICA Mellanmjölk Lite längre hållbarhet låter kanske för bra för att vara sann.",
                    Price = 10.99M,
                    Category = "Mejeri",
                    ProductImage = "Milk.jpg"
                },
                new Product()
                {
                    Title = "Blandfärs 50/50 ca 1,6kg ICA",
                    Description = "ICA Blandfärs är svenskt, fint malen 50/50-blandning av nöt- och fläskfärs som passar till allt ifrån köttbullar och järpar till köttfärssås och tacos. Fläsket ger färsen en lätt och rund smak och gör den enklare att forma om du till exempel vill göra köttbullar.",
                    Price = 115.12M,
                    Category = "Kött, fågel & fisk",
                    ProductImage = "Mincedmeat.jpg"
                },
                new Product()
                {
                    Title = "Jäst för matbröd 50g Kronjäst",
                    Description = "Färsk jäst som ger jäskraft till degar för matbröd. Rör ut jästen i fingervarm degvätska (ca 38 grader), eller kall degvätska (ca 10 grader) för kalljäsning.",
                    Price = 3.50M,
                    Category = "Skafferi",
                    ProductImage = "Yeast.jpg"
                },
                new Product()
                {
                    Title = "Krossade Tomater 500g ICA",
                    Description = "ICA Krossade tomater är krossade italienska solmogna tomater i tomatjuice och kanske den ultimata basvaran att ha i skafferiet för att snabbt få ihop en middag. God att bara krydda upp med lite vitlök och basilika till en pastasås eller låta bli smakbärare i en soppa.",
                    Price = 11.50M,
                    Category = "Skafferi",
                    ProductImage = "Crushedtomatoes.jpg"
                }
            };
            List<Product> cart = new List<Product>();
            products.Serialize(ShopUtils.GetFilePath("Products.json"));
            cart.Serialize(ShopUtils.GetFilePath("Cart.json"));

        }
    }
}
