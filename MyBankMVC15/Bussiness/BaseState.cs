using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyBankMVC15.Bussiness
{
    abstract class BaseState : IBusinessAccount
    {
        protected BusinessLayer account;

        internal BusinessLayer Account
        {
            get { return account; }
            set { account = value; }
        }

        protected StateEnum stateInfo;
        protected StateEnum StateInfo
        {
            get { return stateInfo; }
            set { stateInfo = value; }
        }

        public string GetCheckingAccount(string userName)
        {
            string csRes = "";
            try
            {
                csRes = account.dataAccount.GetCheckingAccount(userName);
            }
            catch (Exception)
            {
                throw;
            }
            return csRes;
        }
        public abstract bool TransferFromChkgToSav(string chkAcctNum, string savAcctNum, double amt);
        public abstract double GetCheckingBalance(string chkAcctNum);
        public abstract double GetSavingBalance(string savAcctNum);
        public List<TransferHistory> GetTransferHistory(string chkAcctNum)
        {
            List<TransferHistory> TList;
            try
            {
                TList = account.dataAccount.GetTransferHistory(chkAcctNum);
            }
            catch (Exception)
            {
                throw;
            }
            return TList;
        }
    }
}