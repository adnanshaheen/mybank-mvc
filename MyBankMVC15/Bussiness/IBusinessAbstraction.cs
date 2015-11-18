using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for IBusinessAbstraction
/// </summary>
public interface IBusinessAbstraction
{
    bool TransferFromChkgToSav(string chkAcctNum, string savAcctNum, double amt);
    double GetCheckingBalance(string chkAcctNum);
    double GetSavingBalance(string savAcctNum);
    List<TransferHistory> GetTransferHistory(string chkAcctNum);
}
