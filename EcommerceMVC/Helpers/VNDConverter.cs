using System.Globalization;

namespace EcommerceMVC.Helpers
{
    public class VNDConverter
    {
        public const int OneDollarPerVND = 25;
        public static string ConvertToVND(double number)
        {
            return number.ToString("#,##0", new CultureInfo("vi-VN")) + " VND";
        }
		public static double VNDToUSD(double number)
		{
            return number / OneDollarPerVND;
		}
	}
}
