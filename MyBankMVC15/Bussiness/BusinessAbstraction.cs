using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BusinessAbstraction
/// </summary>
/// bridge pattern
public class BusinessAbstraction : IBusinessAbstraction
{
    IRepositoryDataAuthentication _idau = null;
    IRepositoryDataAccount _idac = null;

	public BusinessAbstraction(IRepositoryDataAuthentication idau, IRepositoryDataAccount idac)
	{
        _idac = idac;
        _idau = idau;
	}

    //public BusinessAbstraction() :
    //    this(GenericFactory<Repository, IRepositoryDataAuthentication>.CreateInstance(),
    //    GenericFactory<Repository, IRepositoryDataAccount>.CreateInstance())
    //{
    //}

    public BusinessAbstraction() :
        this(GenericFactory<RepositoryMySql, IRepositoryDataAuthentication>.CreateInstance(),
         GenericFactory<RepositoryMySql, IRepositoryDataAccount>.CreateInstance())
    {
    }

    #region IAuthentication Members

    public string IsValidUser(string uname, string pwd)
    {
       return _idau.IsValidUser(uname, pwd);
    }

    public bool ChangePassword(string uname, string oldpwd, string newpwd)
    {
        return _idau.UpdatePassword(uname, oldpwd, newpwd);
    }

    #endregion

    #region IBusinessAccount Members

    public bool TransferFromChkgToSav(string chkAcctNum, string savAcctNum, double amt)
    {
        return _idac.TransferChkToSav(chkAcctNum,savAcctNum,amt);
    }

    #endregion

    #region IBusinessAccount Members

    public double GetCheckingBalance(string chkAcctNum)
    {
        return _idac.GetCheckingBalance(chkAcctNum);
    }

    #endregion

    #region IBusinessAccount Members


    public double GetSavingBalance(string savAcctNum)
    {
        return _idac.GetSavingBalance(savAcctNum);
    }

    #endregion

    #region IBusinessAccount Members

    public List<TransferHistory> GetTransferHistory(string chkAcctNum)
    {
        return _idac.GetTransferHistory(chkAcctNum);
    }

    #endregion
}
