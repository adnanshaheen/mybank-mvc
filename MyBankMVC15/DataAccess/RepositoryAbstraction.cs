using MyBankMVC15.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RepositoryAbstraction
/// </summary>
public class RepositoryAbstraction : IRepositoryDataAccount, IAuthenticationService
{
    IRepositoryDataAccount _irepdacc = null;
    IAuthenticationService _irepdauth = null;
	public RepositoryAbstraction()
    {
        _irepdacc = GenericFactory<Repository, IRepositoryDataAccount>.CreateInstance();
        _irepdauth = GenericFactory<Repository, IAuthenticationService>.CreateInstance();
    }

    public T CreateInstance<T>(T trep)
        where T : IRepositoryDataAccount, IAuthenticationService, new()
    {
        trep = new T();
        _irepdacc = (IRepositoryDataAccount) trep;
        _irepdauth = (IAuthenticationService) trep;
        return trep;
    }

    public string GetCheckingAccount(string userName)
    {
        return _irepdacc.GetCheckingAccount(userName);
    }

    public bool TransferChkToSav(string chkAcctNum, string savAcctNum, double amt)
    {
        return _irepdacc.TransferChkToSav(chkAcctNum,savAcctNum,amt);
    }

    public bool TransferChkToSavViaSP(string chkAcctNum, string savAcctNum, double amt)
    {
        throw new NotImplementedException();
    }

    public double GetCheckingBalance(string chkAcctNum)
    {
        return _irepdacc.GetCheckingBalance(chkAcctNum);
    }

    public double GetSavingBalance(string savAcctNum)
    {
        return _irepdacc.GetSavingBalance(savAcctNum);
    }

    public List<TransferHistory> GetTransferHistory(string chkAcctNum)
    {
        return _irepdacc.GetTransferHistory(chkAcctNum);
    }

    public bool ValidateUser(string userName, string password)
    {
        return _irepdauth.ValidateUser(userName, password);
    }

    public string GetRolesForUser(string uname)
    {
        return _irepdauth.GetRolesForUser(uname);
    }

    public bool SignIn(string userName, string password, bool createPersistentCookie)
    {
        return _irepdauth.SignIn(userName, password, createPersistentCookie);
    }

    public bool ChangePassword(string userName, string password, string newPassword)
    {
        return _irepdauth.ChangePassword(userName, password, newPassword);
    }

    public void SignOut()
    {
        _irepdauth.SignOut();
    }
}