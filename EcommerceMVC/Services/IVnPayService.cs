using EcommerceMVC.ViewModels;

namespace EcommerceMVC.Services
{
	public interface IVnPayService
	{
		string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model, CheckoutVM checkOutVm);
		VnPaymentResponseModel PaymentExecute(IQueryCollection collection);
	}
}
