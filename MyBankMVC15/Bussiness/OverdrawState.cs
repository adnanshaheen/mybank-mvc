﻿using MyBankMVC15.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyBankMVC15.Bussiness
{
    class OverdrawState : BaseState
    {
        public OverdrawState(BaseState State) : this(State.Account)
        {
        }

        public OverdrawState(BusinessLayer acct)
        {
            this.account = acct;
            StateInfo = StateEnum.OVERDRAW;
        }

        public override bool TransferFromChkgToSav(string chkAcctNum, string savAcctNum, double amt)
        {
            bool bRes = false;
            try
            {
                bRes = account.dataAccount.TransferChkToSav(chkAcctNum, savAcctNum, amt);
            }
            catch(Exception)
            {
                throw;
            }
            return bRes;
        }

        public override double GetCheckingBalance(string chkAcctNum)
        {
            double nRes = 0;
            try
            {
                nRes = account.dataAccount.GetCheckingBalance(chkAcctNum);
            }
            catch (Exception)
            {
                throw;
            }
            return nRes;
        }

        public override double GetSavingBalance(string savAcctNum)
        {
            double nRes = 0;
            try
            {
                nRes = account.dataAccount.GetSavingBalance(savAcctNum);
            }
            catch (Exception)
            {
                throw;
            }
            return nRes;
        }

        void CheckState()
        {
            if (account.Balance >= (double)StateEnum.OVERDRAW && account.Balance < (double)StateEnum.SILVER)
                account.state = new SilverState(this);
            else if (account.Balance >= (double)StateEnum.SILVER && account.Balance < (double)StateEnum.GOLD)
                account.state = new GoldState(this);
            else if (account.Balance >= (double)StateEnum.GOLD)
                account.state = new PlatinumState(this);
        }
    }
}