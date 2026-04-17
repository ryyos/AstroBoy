using System;
using System.Collections.Generic;
using System.Text;
using AstroBoy.Models;

namespace AstroBoy.Services
{
    internal class CustomerService
    {

        public int GetTotalCustomers()
        {
            return 67;
        }

        public void AddCustomer(Customer customer)
        {
        }

        public List<Customer> GetAllCustomers()
        {
            return new List<Customer> {
                new Customer(
                    name: "Emma myers",
                    email: "emma@gmail.com",
                    password: "emma123",
                    role: "admin"
                )
            };
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
