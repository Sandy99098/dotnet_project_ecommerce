using Microsoft.EntityFrameworkCore;
using dotnet_project_ecommerce.Models;
using Microsoft.Extensions.Configuration; // Adjust the namespace if needed

public class myContext : DbContext
{
    private IConfigurationRoot configuration;

    public myContext(IConfigurationRoot configuration)
    {
        this.configuration = configuration;
    }

    public DbSet<Admin> tbl_admin { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("myconnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}