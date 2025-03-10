using System.ComponentModel.DataAnnotations;

namespace dotnet_project_ecommerce.Models
{
    public class Category
    {
        [Key]
        public int category_id { get; set; }
        public string category_name { get; set; } 
        public List<Product> Product { get; set; }
    }
}
