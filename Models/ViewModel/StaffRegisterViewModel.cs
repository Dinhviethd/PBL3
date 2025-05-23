﻿using System.ComponentModel.DataAnnotations;

namespace PBL3.Models.ViewModel
{
    public class StaffRegisterViewModel : RegisterViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc cho nhân viên")]
        [Display(Name = "Địa chỉ")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string DiaChi { get; set; }
        public IEnumerable<Staff> Staffs { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
