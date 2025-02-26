namespace EcommerceMVC.Helpers
{
    public class MySetting
    {
        public static string CART_KEY = "MYCART";
		public static string CLAIM_CUSTOMERID = "CustomerID";
        public const int ITEMS_PER_PAGE = 9;

        public class PaymentType
		{
			public  string COD = "COD";
			public static string Paypal = "Paypal";
			public static string VNPAY = "VnPay";
			public static string MOMO = "MoMo";
			public static string STRIPE = "Stripe";
		}
        public enum SortOption
        {
            Az,        
            Za,        
            PriceAsc,  
            PriceDesc  
        }
	}
}
