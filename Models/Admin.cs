using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;

namespace dotnet_project_ecommerce.Models
{
    public class Admin
    {
        

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int admin_id { get; set; }

        [Required]
        [StringLength(50)]
        public required string admin_name { get; set; }

        [Required]
        [EmailAddress]
        public required string admin_email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string admin_password { get; set; }

        public  string admin_image { get; set; }

        // Hash the password
        public void HashPassword()
        {
            this.admin_password = BCrypt.Net.BCrypt.HashPassword(this.admin_password);
        }

        // Verify the password
        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, this.admin_password);
        }
    }
}