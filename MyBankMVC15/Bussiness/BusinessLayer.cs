using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// Summary description for BusinessLayer
/// </summary>
public class BusinessLayer : IBusinessAuthentication, IBusinessAccount
{
    IRepositoryDataAuthentication idau = null;
    IRepositoryDataAccount idac = null;
    public BusinessLayer(IRepositoryDataAuthentication idauth, IRepositoryDataAccount idacc)
    {
        idau = idauth;
        idac = idacc;
    }

    //public BusinessLayer():
    //    this(GenericFactory<Repository, IRepositoryDataAuthentication>.CreateInstance(),
    //GenericFactory<Repository, IRepositoryDataAccount>.CreateInstance())
    //{
    //}

    public BusinessLayer() :
        this(GenericFactory<RepositoryMySql, IRepositoryDataAuthentication>.CreateInstance(),
         GenericFactory<RepositoryMySql, IRepositoryDataAccount>.CreateInstance())
    {
    }

    #region IAuthentication Members

    public string IsValidUser(string uname, string pwd)
    {
        string res = "";
        try
        {
            res = idau.IsValidUser(uname, pwd);
        }
        catch (Exception)
        {
            throw;
        }
        return res;
    }

    public bool ChangePassword(string uname, string oldpwd, string newpwd)
    {
        bool res = false;
        try
        {
            res = idau.UpdatePassword(uname, oldpwd, newpwd);
        }
        catch (Exception)
        {
            throw;
        }
        return res;
    }

    #endregion

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