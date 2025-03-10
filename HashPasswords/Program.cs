using BCrypt.Net;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HashPasswords
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json")
                .Build();

            using (var context = new myContext(configuration))
            {
                var admins = context.tbl_admin.ToList();
                foreach (var admin in admins)
                {
                    // Hash the plain text password
                    admin.admin_password = BCrypt.Net.BCrypt.HashPassword(admin.admin_password);
                }
                context.SaveChanges();
            }
            Console.WriteLine("Passwords hashed successfully.");
        }
    }
}