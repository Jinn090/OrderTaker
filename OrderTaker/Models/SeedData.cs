using Bogus;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using OrderTaker.Data;
using System.Linq;

namespace OrderTaker.Models
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, User user)
        {
            using var context = new OrderTakerContext(
                serviceProvider.GetRequiredService<DbContextOptions<OrderTakerContext>>());

            if (context.Customers.Any())
            {
                return;
            }

            Random rnd = new();
            float probabilityTrue = (float)0.7;
            var now = DateTime.Now;
            var start = new DateTime(now.Year, now.Month, 1);
            var end = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(start.Year, start.Month));

            var productTest = new Faker<SKU>("en")
                .Rules((faker, sku) =>
                {

                    sku.Name = $"{faker.Commerce.ProductName()}-{faker.UniqueIndex}";
                    sku.Code = $"{faker.Commerce.Product()}-{faker.UniqueIndex}";
                    sku.UnitPrice = Convert.ToDecimal(faker.Commerce.Price(1000, 100000, 2));
                    sku.DateCreated = faker.Date.Between(start, end);
                    sku.CreatedBy = $"{user.LastName}, {user.FirstName}";
                    sku.Timestamp = sku.DateCreated;
                    sku.UserID = user.Id;
                    sku.IsActive = faker.Random.Bool(probabilityTrue);

                });
            var productList = productTest.Generate(500);
            await context.AddRangeAsync(productList);

            var purchaseOrderTest = new Faker<PurchaseOrder>("en")
                .Rules((faker, po) =>
                {
                    po.DateOfDelivery = faker.Date.Between(start, end);
                    po.Status = faker.PickRandom<Status>().ToString();
                    po.DateCreated = faker.Date.Between(start, end);
                    po.CreatedBy = $"{user.LastName}, {user.FirstName}";
                    po.Timestamp = po.DateCreated;
                    po.UserID = user.Id;
                    po.IsActive = faker.Random.Bool(probabilityTrue);
                });

            var customerTest = new Faker<Customer>("en")
                .Rules((faker, customer) =>
                {
                    customer.FirstName = $"{faker.Name.FirstName()} - {faker.UniqueIndex}";
                    customer.LastName = faker.Name.LastName();
                    customer.MobileNumber = faker.Random.ReplaceNumbers("##########");
                    customer.City = faker.Address.City();
                    customer.DateCreated = faker.Date.Between(start, end);
                    customer.CreatedBy = $"{user.LastName}, {user.FirstName}";
                    customer.Timestamp = customer.DateCreated;
                    customer.UserID = user.Id;
                    customer.IsActive = faker.Random.Bool(probabilityTrue);

                });

            var purchasedItemTest = new Faker<PurchaseItem>("en")
                .Rules((faker, item) =>
                {
                    item.Quantity = faker.Random.Number(1, 50);
                    item.Timestamp = faker.Date.Between(start, end);
                    item.UserID = user.Id;
                });

            var customerList = customerTest.Generate(1000);
            
            foreach (var customer in customerList)
            {
                customer.PurchaseOrders.AddRange(purchaseOrderTest.Generate(rnd.Next(1, 20)));

                foreach (var order in customer.PurchaseOrders)
                {
                    var purchasedItems = purchasedItemTest.Generate(rnd.Next(1, 9));
                    foreach (var item in purchasedItems)
                    {

                        var sku = productList
                            .OrderBy(x => rnd.Next(0, productList.Count))
                            .First();

                        var existingPurchasedItem = order.PurchaseItems
                            .FirstOrDefault(x => x.SKU.Code == sku.Code);

                        if (existingPurchasedItem == null)
                        {
                            item.PurchaseOrder = order;
                            item.SKU = sku;
                            item.Price = item.Quantity * sku.UnitPrice;
                            order.PurchaseItems.Add(item);
                        }

                    }

                    order.AmountDue = order.PurchaseItems.Sum(x => x.Price);
                }
            }

            await context.AddRangeAsync(customerList);

            await context.SaveChangesAsync();


        }
    }
}
