using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for IBusinessAbstraction
/// </summary>
public interface IBusinessAbstraction
{
    string IsValidUser(string uname, string pwd);
    bool ChangePassword(string uname, string oldpwd, string newpwd);
    bool TransferFromChkgToSav(string chkAcctNum, string savAcctNum, double amt);
    double GetCheckingBalance(string chkAcctNum);
    double GetSavingBalance(string savAcctNum);
    List<TransferHistory> GetTransferHistory(string chkAcctNum);
}
