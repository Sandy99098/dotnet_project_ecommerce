using System.ComponentModel.DataAnnotations;

namespace dotnet_project_ecommerce.Models
{
    public class Feedback
    {
        [Key]
        public int feedback_id {  get; set; }   

        public string user_name { get; set; } 
        public string user_message { get; set; }
    }
}
