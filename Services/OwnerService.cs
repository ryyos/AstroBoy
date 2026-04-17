using System;
using System.Collections.Generic;
using System.Text;
using AstroBoy.Models;

namespace AstroBoy.Services
{
    internal class OwnerService
    {
        public List<Owner> GetAll()
        {
            return new List<Owner>
            {
                new Owner(
                    name: "Ryyos",
                    email: "ryyos@gmail.com",
                    password: "123",
                    role: "Owner"
                )
            };
        }
    }
}
