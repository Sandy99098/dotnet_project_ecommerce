namespace dotnet_project_ecommerce.ViewModels
{
    public class KhaltiResponse
    {
        public string pidx { get; set; }
        public string payment_url { get; set; }
        public DateTime expires_at { get; set; }
        public int expires_in { get; set; }
    }

}
