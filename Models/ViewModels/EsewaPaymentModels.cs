using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using dotnet_project_ecommerce.Models;
using Newtonsoft.Json;

namespace dotnet_project_ecommerce.ViewModels.Payment
{
    public class EsewaPaymentRequest
    {
        public string amount { get; set; }
        public string tax_amount { get; set; }
        public string total_amount { get; set; }
        public string transaction_uuid { get; set; }
        public string product_code { get; set; }
        public string product_service_charge { get; set; }
        public string product_delivery_charge { get; set; }
        public string success_url { get; set; }
        public string failure_url { get; set; }
        public string signed_field_names { get; set; }
        public string signature { get; set; }
    }

    public class EsewaPaymentResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("transaction_code")]
        public string TransactionCode { get; set; }

        [JsonProperty("total_amount")]
        public string TotalAmount { get; set; }

        [JsonProperty("transaction_uuid")]
        public string TransactionUuid { get; set; }

        [JsonProperty("product_code")]
        public string ProductCode { get; set; }
    }
    public class PaymentTransaction
    {
        [Key]
        public string TransactionId { get; set; }

        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string TransactionCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }





}