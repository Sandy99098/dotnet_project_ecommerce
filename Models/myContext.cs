using dotnet_project_ecommerce.ViewModels.Payment;
using Microsoft.EntityFrameworkCore;

namespace dotnet_project_ecommerce.Models
{
    public class myContext : DbContext

    {
        public myContext(DbContextOptions<myContext> options) : base(options)
        {
        }

        public DbSet<Admin> tbl_admin { get; set; }
        public DbSet<Customer> tbl_customer { get; set; }
        public DbSet<Category> tbl_category { get; set; }
        public DbSet<Product> tbl_product { get; set; }
        public DbSet<Cart> tbl_cart { get; set; }
        public DbSet<Feedback> tbl_feedback
        { get; set; }
        public DbSet<Faqs> tbl_faq { get; set; }
        public object Admin { get; internal set; }
      
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }   // for esewa payment

      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            ////esewa
            //modelBuilder.Entity<PaymentTransaction>()
            //    .HasKey(pt => pt.TransactionId); // Define TransactionId as the primary key
            modelBuilder.Entity<PaymentTransaction>()
    .HasKey(pt => pt.TransactionId);  // Ensure TransactionId is set as the primary key


            base.OnModelCreating(modelBuilder);
            // Configure the decimal properties with precision and scale
            modelBuilder.Entity<PaymentTransaction>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");  // For example, precision 18 and scale 2

            modelBuilder.Entity<PaymentTransaction>()
                .Property(p => p.TaxAmount)
                .HasColumnType("decimal(18,2)");  // Same precision and scale as Amount

            modelBuilder.Entity<PaymentTransaction>()
                .Property(p => p.TotalAmount)
                .HasColumnType("decimal(18,2)");  // Same precision and scale as Amount

            modelBuilder.Entity<Product>()
        .Property(p => p.product_price)
        .HasColumnType("decimal(18,2)");

            //relationship  between product and category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Product)
                .HasForeignKey(p => p.cat_id);
            // relationship between Cart and Product
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.prod_id);

            //  between Cart and Customer
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.cust_id);


            //esewa payment transaction relationn
        
            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.Product)
                .WithMany()
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.Customer)
                .WithMany()
                .HasForeignKey(pt => pt.CustomerId);
        

    }
    }
}
