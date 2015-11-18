using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for BusinessLayer
/// </summary>
public class BusinessLayer : IBusinessAccount
{
    IRepositoryDataAccount idac = null;
    public BusinessLayer(IRepositoryDataAccount idacc)
    {
        idac = idacc;
    }

    //public BusinessLayer():
    //    this(GenericFactory<Repository, IRepositoryDataAuthentication>.CreateInstance(),
    //GenericFactory<Repository, IRepositoryDataAccount>.CreateInstance())
    //{
    //}

    public BusinessLayer() :
        this(GenericFactory<RepositoryMySql, IRepositoryDataAccount>.CreateInstance())
    {
    }

    #region IBusinessAccount Members

    public bool TransferFromChkgToSav(string chkAcctNum, string savAcctNum, double amt)
    {
        return idac.TransferChkToSavViaSP(chkAcctNum,savAcctNum,amt);
    }

    #endregion

    #region IBusinessAccount Members


    public double GetCheckingBalance(string chkAcctNum)
    {
        double res = 0;
        try
        {
            res = idac.GetCheckingBalance(chkAcctNum);
        }
        catch (Exception)
        {
            throw;
        }
        return res;
    }

    #endregion

    #region IBusinessAccount Members


    public double GetSavingBalance(string savAcctNum)
    {
        double res = 0;
        try
        {
            res = idac.GetSavingBalance(savAcctNum);
        }
        catch (Exception)
        {
            throw;
        }
        return res;
    }

    #endregion

    #region IBusinessAccount Members


    public List<TransferHistory> GetTransferHistory(string chkAcctNum)
    {
        List<TransferHistory> TList = null;
        try
        {
            TList = idac.GetTransferHistory(chkAcctNum);
        }
        catch (Exception)
        {
            throw;
        }
        return TList;
    }

    #endregion
}