using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_project_ecommerce.Models
{
    public class Cart
    {
        [Key]
        public int cart_id { get; set; }

        [ForeignKey("Product")]
        public int prod_id { get; set; }
        public Product Product { get; set; } // Navigation property for Product

        [ForeignKey("Customer")]
        public int cust_id { get; set; }
        public Customer Customer { get; set; } // Navigation property for Customer

        public int product_quantity { get; set; }

        public int cart_status { get; set; } // 0 = Inactive, 1 = Active

        [NotMapped] // This property is not stored in the database
        public decimal TotalPrice => Product?.product_price * product_quantity ?? 0;
    }
}