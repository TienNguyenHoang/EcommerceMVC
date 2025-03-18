using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.ViewModels
{
	public class CheckoutVM
	{
		[Required(ErrorMessage = "Vui lòng nhập {0}")]
		[DisplayName("Tên người nhận")]
		[StringLength(50,MinimumLength = 3, ErrorMessage = "{0} phải từ {2} đến {1} ký tự")]
		public string ReceiverName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
		[StringLength(60, MinimumLength = 3, ErrorMessage = "{0} phải từ {2} đến {1} ký tự")]
		[DisplayName("Địa chỉ")]
		public string Address { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập {0}")]
		[Phone(ErrorMessage = "Sai định dạng {0}")]
		[DisplayName("Số điện thoại")]
		public string Phone { get; set; }
		public string? Note { get; set; }
	}
}
