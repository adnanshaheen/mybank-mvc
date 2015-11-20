using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MyBankMVC15.Bussiness;

/// <summary>
/// Summary description for BusinessLayer
/// </summary>
public class BusinessLayer : IBusinessAccount
{
    IRepositoryDataAccount idac = null;
    internal IRepositoryDataAccount dataAccount
    {
        get { return idac; }
    }
    public double Balance { get; set; }
    BaseState State;
    internal BaseState state
    {
        get { return State; }
        set { State = value; }
    }

    public BusinessLayer(IRepositoryDataAccount idacc)
    {
        idac = idacc;
        state = new SilverState(this);
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
        bool bRes = false;
        try
        {
            bRes = state.TransferFromChkgToSav(chkAcctNum, savAcctNum, amt);
        }
        catch (Exception)
        {
            throw;
        }
        return bRes;
    }

    #endregion

    #region IBusinessAccount Members


    public double GetCheckingBalance(string chkAcctNum)
    {
        double res = 0;
        try
        {
            res = state.GetCheckingBalance(chkAcctNum);
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
            res = state.GetSavingBalance(savAcctNum);
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
            TList = state.GetTransferHistory(chkAcctNum);
        }
        catch (Exception)
        {
            throw;
        }
        return TList;
    }

    #endregion

    #region IBusinessAccount Members
    public string GetCheckingAccount(string userName)
    {
        string AccountNumber = "";
        try
        {
            AccountNumber = state.GetCheckingAccount(userName);
        }
        catch (Exception)
        {
            throw;
        }
        return AccountNumber;
    }
    #endregion
}