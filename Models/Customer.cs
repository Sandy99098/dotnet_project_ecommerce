using System.ComponentModel.DataAnnotations;

namespace dotnet_project_ecommerce.Models
{
    public class Customer
    {
        [Key]
        public int customer_id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string customer_name { get; set; }

        public string customer_phone { get; set; } = "Unknown";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string customer_email { get; set; }  

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string customer_password { get; set; }

        public string customer_gender { get; set; } = "Not Specified";

        public string customer_country { get; set; } = "Unknown";

        public string customer_city { get; set; } = "Unknown";

        public string customer_address { get; set; } = "Not Provided";

        public string customer_image { get; set; } = "noimage.jpg";
    }
}