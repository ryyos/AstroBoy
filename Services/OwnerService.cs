using System;
using Database;
using System.Collections.Generic;
using System.Text;
using AstroBoy.Models;

namespace AstroBoy.Services
{
    public class OwnerService
    {

        private List<Owner> owners;
        private DatabaseContext db;

        public OwnerService()
        {
            db = new DatabaseContext();
            owners = db.GetAllOwners();
        }
        public List<Owner> GetAllOwners()
        {
            return owners;
        }

        public int GetTotalOwners()
        {
            return owners.Count;
        }
    }
}
