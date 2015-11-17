using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for IDataAccount
/// </summary>
public interface IRepositoryDataAccount
{
    bool TransferChkToSav(string chkAcctNum, string savAcctNum, double amt);
    bool TransferChkToSavViaSP(string chkAcctNum, 
               string savAcctNum, double amt);
    double GetCheckingBalance(string chkAcctNum);
    double GetSavingBalance(string savAcctNum);
    List<TransferHistory> GetTransferHistory(string chkAcctNum);
}