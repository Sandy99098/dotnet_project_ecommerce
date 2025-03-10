using System.ComponentModel.DataAnnotations;

namespace dotnet_project_ecommerce.Models // Adjust the namespace if needed
{
    public class Admin
    {
        [Key]
        public int admin_id { get; set; }

        [Required]
        [StringLength(50)]
        public string admin_name { get; set; }

        [Required]
        [EmailAddress]
        public string admin_email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string admin_password { get; set; }

        public string admin_image { get; set; }
    }
}