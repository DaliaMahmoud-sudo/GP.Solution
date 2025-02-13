using GP.Core.Entities;
using GP.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GP.Core.Entites.OrderAggregate;
using System.Text.Json;

namespace GP.Repository.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext dbContext)
        {
       
            //seeding product

            if (!dbContext.products.Any())
            {
                var ProductsData = File.ReadAllText("../GP.Repository/Data/DataSeed/products.json");
                var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);
                foreach (var Product in Products)
                {
                    await dbContext.Set<Product>().AddAsync(Product);
                }
                await dbContext.SaveChangesAsync();

            }
            //seeding delivery
            if (!dbContext.DeliveryMethod.Any())
            {
                var DeliveryMethodData = File.ReadAllText("../GP.Repository/Data/DataSeed/delivery.json");
                var DeliveryMethod = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodData);
                foreach (var DeliveryMethods in DeliveryMethod)
                {
                    await dbContext.Set<DeliveryMethod>().AddAsync(DeliveryMethods);
                }
                await dbContext.SaveChangesAsync();

            }





        }
    }
}
