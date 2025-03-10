using dotnet_project_ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace dotnet_project_ecommerce.Data
{
    public class myContextFactory : IDesignTimeDbContextFactory<myContext>
    {
        public myContext CreateDbContext(string[] args)
        {
            // Build configuration to read the connection string from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Ensure this points to your project root
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<myContext>();

            // Replace "DefaultConnection" with your connection string key if different
            var connectionString = configuration.GetConnectionString("myconnection");
            builder.UseSqlServer(connectionString);

            return new myContext(builder.Options);
        }
    }
}
