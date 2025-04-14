namespace dotnet_project_ecommerce.ViewModels
{
    public class KhaltiPaymentReceiptViewModel
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        //public List<CartItemDetail> Items { get; set; } 

    }
    public class CartItemDetail
    {
        public string ProductName { get; set; }
        public int Items { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
