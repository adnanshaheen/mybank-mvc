using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyBankMVC15.Models
{
    public class XferChkToSavModel
    {
        public double CheckingBalance { get; set; }
        public double SavingBalance { get; set; }
        public double AmountTransfer { get; set; }
        public string Status { get; set; }
    }
}