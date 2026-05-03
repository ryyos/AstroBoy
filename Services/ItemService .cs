using System;
using Database;
using System.Collections.Generic;
using System.Text;
using AstroBoy.Models;

namespace AstroBoy.Services
{
    public class ItemService
    {
        private DatabaseContext db;

        public ItemService()
        {
            db = new DatabaseContext();
        }
        public int GetTotalItems()
        {
            return db.GetAllItems().Count;
        }

        public List<Item> GetAllItems()
        {
            return db.GetAllItems();
        }

        public List<Item> GetItemsByStore(string id)
        {
            return db.GetItemsByStore(id);
        }
    }
}
