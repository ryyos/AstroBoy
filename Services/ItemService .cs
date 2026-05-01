using System;
using Database;
using System.Collections.Generic;
using System.Text;
using AstroBoy.Models;

namespace AstroBoy.Services
{
    internal class ItemService
    {
        private List<Item> items;
        private DatabaseContext db;

        public ItemService()
        {
            db = new DatabaseContext();
            items = db.GetAllItems();
        }
        public int GetTotalItems()
        {
            return items.Count;
        }

        public List<Item> GetAllItems()
        {
            return items;
        }
    }
}
