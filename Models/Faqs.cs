using System.ComponentModel.DataAnnotations;

namespace dotnet_project_ecommerce.Models
{
    public class Faqs
    {
        [Key]
        public int faq_id {  get; set; }
        public string faq_question { get; set; }
        public string faq_answer { get; set; }



    }
}
