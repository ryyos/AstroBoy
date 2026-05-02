using System;
using Database;

using System.Collections.Generic;
using System.Text;
using AstroBoy.Models;

namespace AstroBoy.Services
{
    internal class CustomerService
    {
        private DatabaseContext db;

        public CustomerService()
        {
            db = new DatabaseContext();
        }

        public int GetTotalCustomers()
        {
            return db.GetAllCustomers().Count;
        }

        public void AddCustomer(Customer customer)
        {
            db.InsertUser(customer);
        }

        public List<Customer> GetAllCustomers()
        {
            return db.GetAllCustomers();
        }

        public void Delete(string id)
        {
        }

        public Customer GetById(string id)
        {
            return new Customer(
                    name: "Emma myers",
                    email: "emma@gmail.com",
                    password: "emma123",
                    role: "admin"
                );
        }

        public void Update(Customer customer)
        {

        }
    }

}
