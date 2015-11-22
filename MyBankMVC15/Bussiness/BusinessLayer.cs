using System;
using System.Collections.Generic;
using MyBankMVC15.Bussiness;
using MyBankMVC15.DataAccess;

/// <summary>
/// Summary description for BusinessLayer
/// </summary>
namespace MyBankMVC15.Business
{
    public class BusinessLayer : IBusinessAccount, IBusinessAuthentication
    {
        IRepositoryDataAccount idac = null;
        IAuthenticationService authService = null;

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

        public BusinessLayer(IAuthenticationService auth, IRepositoryDataAccount idacc)
        {
            idac = idacc;
            authService = auth;
            state = new SilverState(this);
        }

        public BusinessLayer() :
            this(GenericFactory<AuthenticationService, IAuthenticationService>.CreateInstance(),
                GenericFactory<Repository, IRepositoryDataAccount>.CreateInstance())
        {
        }

        //public BusinessLayer() :
        //    this(GenericFactory<AuthhenticationServiceMySql, IAuthenticationService>.CreateInstance(),
        //        GenericFactory<RepositoryMySql, IRepositoryDataAccount>.CreateInstance())
        //{
        //}

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

        #region IBusinessAuthentication Members
        public string GetRolesForUser(string uname)
        {
            return authService.GetRolesForUser(uname);
        }

        public bool SignIn(string userName, string password, bool createPersistentCookie)
        {
            return authService.SignIn(userName, password, createPersistentCookie);
        }

        public bool ChangePassword(string userName, string password, string newPassword)
        {
            return authService.ChangePassword(userName, password, newPassword);
        }

        public void SignOut()
        {
            authService.SignOut();
        }

        public bool ValidateUser(string userName, string password)
        {
            return authService.ValidateUser(userName, password);
        }
        #endregion
    }
}