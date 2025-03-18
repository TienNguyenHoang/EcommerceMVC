using System;
using System.ComponentModel.DataAnnotations;

namespace App.Areas.Identity.Models.ManageViewModels
{
  public class EditExtraProfileModel
  {
      [Display(Name = "Tên tài khoản")]
      public string UserName { get; set; }

      [Display(Name = "Địa chỉ email")]
      public string UserEmail { get; set; }
      [Display(Name = "Số điện thoại")]
      public string PhoneNumber { get; set; }

      [Display(Name = "Địa chỉ")]
      [Required(ErrorMessage = "Phải nhập {0}")]
      [StringLength(400,MinimumLength = 6, ErrorMessage = "{0} phải từ {2} đến {1} ký tự")]
      public string HomeAdress { get; set; }


      [Display(Name = "Ngày sinh")]
      [Required(ErrorMessage = "Phải nhập {0}")]
      public DateTime? BirthDate { get; set; }
  }
}