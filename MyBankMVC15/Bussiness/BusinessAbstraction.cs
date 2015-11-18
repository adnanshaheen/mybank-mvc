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
    IRepositoryDataAccount _idac = null;

	public BusinessAbstraction(IRepositoryDataAccount idac)
	{
        _idac = idac;
	}

    //public BusinessAbstraction() :
    //    this(GenericFactory<Repository, IRepositoryDataAuthentication>.CreateInstance(),
    //    GenericFactory<Repository, IRepositoryDataAccount>.CreateInstance())
    //{
    //}

    public BusinessAbstraction() :
        this(GenericFactory<RepositoryMySql, IRepositoryDataAccount>.CreateInstance())
    {
    }

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
