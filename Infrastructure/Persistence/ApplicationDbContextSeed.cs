using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            // Seed, if necessary
            if (!context.Products.Any())
            {
                context.Products.Add(new Product
                {
                    Name = "Test",
                    Description = "Test"
                    
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
