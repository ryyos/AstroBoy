using System;
using System.Collections.Generic;
using System.Text;
using AstroBoy.Models;

namespace AstroBoy.Services
{
    internal class ItemService
    {
        public int GetTotalItems()
        {
            return 67;
        }

        public List<Item> GetAll()
        {
            return new List<Item>
            {
                new Item { Id = Guid.NewGuid(), Name = "Laptop ASUS", Price = 8500000, Stock = 10, Category = "Elektronik", StoreId = "store-001" }
            };
        }
    }
}
