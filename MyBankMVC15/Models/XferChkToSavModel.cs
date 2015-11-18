using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyBankMVC15.Models
{
    public class XferChkToSavModel
    {
        [Display(Name ="Checking Balance:")]
        public double CheckingBalance { get; set; }

        [Display(Name ="Saving Balance:")]
        public double SavingBalance { get; set; }

        [Required(ErrorMessage = "Enter amount")]
        [Display(Name ="Amount to transfer")]
        public double AmountTransfer { get; set; }
        public string Status { get; set; }
        public string AccountNumber { get; set; }
    }
}