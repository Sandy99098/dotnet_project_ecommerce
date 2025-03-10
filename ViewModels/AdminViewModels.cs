using System.ComponentModel.DataAnnotations;

namespace dotnet_project_ecommerce.ViewModels
{
    public class AdminRegisterViewModel
    {
        [Required(ErrorMessage = "Admin name is required.")]
        [StringLength(50, ErrorMessage = "Admin name cannot exceed 50 characters.")]
        public string admin_name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string admin_email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string admin_password { get; set; }

        [DataType(DataType.Password)]
        [Compare("admin_password", ErrorMessage = "Passwords do not match.")]
        public string confirm_password { get; set; }
    }

    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string admin_email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string admin_password { get; set; }
    }
}