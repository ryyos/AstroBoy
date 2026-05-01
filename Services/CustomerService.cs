using System;
using Database;

using System.Collections.Generic;
using System.Text;
using AstroBoy.Models;

namespace AstroBoy.Services
{
    internal class CustomerService
    {
        private List<Customer> customers;
        private DatabaseContext db;

        public CustomerService()
        {
            db = new DatabaseContext();
            customers = db.GetAllCustomers();
        }

        public int GetTotalCustomers()
        {
            return customers.Count;
        }

        public void AddCustomer(Customer customer)
        {
        }

        public List<Customer> GetAllCustomers()
        {
            return customers;
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
