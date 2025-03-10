using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace dotnet_project_ecommerce.Models
{
    public class Product
    {
        [Key]
        public int product_id {  get; set; }    
        public string product_name { get; set; } 
        public string product_description { get; set; }
        public double product_price { get; set; }
        public string product_image { get; set; }
        public int cat_id { get; set; }
        public Category Category { get; set; }
    }
}


