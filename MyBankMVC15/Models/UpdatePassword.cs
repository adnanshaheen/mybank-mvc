using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyBankMVC15.Models
{
    public class UpdatePassword
    {
        [Required(ErrorMessage = "Old password is required ...")]
        public string oldPassword { get; set; }

        [Required(ErrorMessage ="New password is required ...")]
        public string newPassword { get; set; }

        [Required(ErrorMessage ="New password is required ...")]
        [CompareAttribute("newPassword", ErrorMessage = "Passwords don't match.")]
        public string reNewPassword { get; set; }

        public string Status { get; set; }
    }
}