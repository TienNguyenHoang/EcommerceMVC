// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Areas.Identity.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
            [Required(ErrorMessage = "Phải nhập {0}")]
            [EmailAddress(ErrorMessage="Phải đúng định dạng email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(100, ErrorMessage = "{0} phải có từ {2} đến {1} ký tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nhập mật khẩu mới")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Lặp lại mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu lặp lại không chính xác..")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }

    }
}
