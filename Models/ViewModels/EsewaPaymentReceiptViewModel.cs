namespace dotnet_project_ecommerce.Models.ViewModels
{
    public class EsewaPaymentReceiptViewModel
    {
        public string TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }

        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }

        // Optional: You can add more info like phone, address, etc. if needed
    }
}
