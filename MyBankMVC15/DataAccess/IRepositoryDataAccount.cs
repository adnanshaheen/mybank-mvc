using System.Collections.Generic;

/// <summary>
/// Summary description for IDataAccount
/// </summary>
public interface IRepositoryDataAccount
{
    string GetCheckingAccount(string userName);
    bool TransferChkToSav(string chkAcctNum, string savAcctNum, double amt);
    bool TransferChkToSavViaSP(string chkAcctNum, 
               string savAcctNum, double amt);
    double GetCheckingBalance(string chkAcctNum);
    double GetSavingBalance(string savAcctNum);
    List<TransferHistory> GetTransferHistory(string chkAcctNum);
}