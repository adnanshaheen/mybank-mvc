using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TransferHistoryEntity
/// </summary>
public class TransferHistory : IEntity
{
    public string FromAccountNum { get; set; }
    public string ToAccountNum { get; set; }
    public string CheckingAccountNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransferDate { get; set; }
    #region IEntity Members

    public void SetFields(System.Data.DataRow dr)
    {
        this.FromAccountNum = (string)dr["FromAccountNum"];
        this.ToAccountNum = (string)dr["ToAccountNum"];
        this.CheckingAccountNumber = (string)dr["CheckingAccountNumber"];
        this.Amount = (decimal)dr["Amount"];
        this.TransferDate = (DateTime)dr["Date"];
    }

    #endregion
}