using System;
using System.Collections.Generic;
using System.Text;
using AstroBoy.Models;

namespace AstroBoy.Services
{
    internal class UserService
    {

        public int GetTotalUsers()
        {
            return 67;
        }

        public List<User> GetAllUsers()
        {
            return new List<User> {
                new Customer(
                    name: "Emma myers",
                    email: "emma@gmail.com",
                    password: "emma123",
                    role: "admin"
                )
            };
        }

    }

}
