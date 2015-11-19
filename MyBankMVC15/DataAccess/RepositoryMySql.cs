using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using MyBankMVC15.DataAccess;

/// <summary>
/// Summary description for RespositoryMySql
/// </summary>
public class RepositoryMySql : IRepositoryDataAccount
{
    IDataAbstraction _idataAccess = null;
    CacheAbstraction webCache = null;

	public RepositoryMySql(IDataAbstraction idac, CacheAbstraction webc)
	{
        _idataAccess = idac;
        webCache = webc;
	}

    public RepositoryMySql()
        : this(GenericFactory<DataAbstraction, IDataAbstraction>.CreateInstance(),
        new CacheAbstraction())
    {
    }

    #region IDataAccount Members

    public bool TransferChkToSav(string chkAcctNum, string savAcctNum, double amt)
    {
        bool res = false;
        string CONNSTR = ConfigurationManager.ConnectionStrings["BANKDBCONN"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(CONNSTR);
        MySqlTransaction Transection = null;

        try
        {
            conn.Open();
            Transection = conn.BeginTransaction();
            DbParameter p1 = new MySqlParameter("@chkAcctNum", MySqlDbType.VarChar, 50);
            p1.Value = chkAcctNum;
            string sql1 = "update  CheckingAccounts set balance=balance-" +
                amt.ToString() + " where checkingaccountnumber=@chkAcctNum";
            MySqlCommand cmd1 = new MySqlCommand(sql1, conn);
            cmd1.Parameters.Add(p1);
            cmd1.Transaction = Transection;
            int rows = cmd1.ExecuteNonQuery();

            string sql2 = "select balance from CheckingAccounts where CheckingAccountNumber=@chkAcctNum";
            DbCommand cmd2 = new MySqlCommand(sql2, conn);
            cmd2.Parameters.Add(p1);
            object obal = cmd2.ExecuteScalar();
            if (double.Parse(obal.ToString()) < 0)
                throw new Exception("Amount cannot be transferred - results in negative balance..");

            string sql3 = "update  SavingAccounts set balance=balance+" +
                amt.ToString() + " where SavingAccountnumber=@savAcctNum";
            MySqlCommand cmd3 = new MySqlCommand(sql3, conn);
            DbParameter p1a = new MySqlParameter("@savAcctNum", MySqlDbType.VarChar, 50);
            p1a.Value = savAcctNum;
            cmd3.Parameters.Add(p1a);
            cmd3.Transaction = Transection;
            rows = cmd3.ExecuteNonQuery();

            string sql4 = "insert into TransferHistory(FromAccountNum,ToAccountNum,Amount," +
                "CheckingAccountNumber) values (@chkAcctNum,@savAcctNum,@amt,@chkAcctNum)";
            MySqlCommand cmd4 = new MySqlCommand(sql4, conn);
            DbParameter p4a = new MySqlParameter("@chkAcctNum", MySqlDbType.VarChar, 50);
            p4a.Value = chkAcctNum;
            cmd4.Parameters.Add(p4a);
            DbParameter p4b = new MySqlParameter("@savAcctNum", MySqlDbType.VarChar, 50);
            p4b.Value = savAcctNum;
            cmd4.Parameters.Add(p4b);
            DbParameter p4c = new MySqlParameter("@amt", MySqlDbType.Decimal, 20);
            p4c.Value = amt;
            cmd4.Parameters.Add(p4c);
            cmd4.Transaction = Transection;
            rows = cmd4.ExecuteNonQuery();
            Transection.Commit();
            res = true;

            // clear cache for TransferHistory
            string key = String.Format("TransferHistory_{0}",
                chkAcctNum);
            webCache.Remove(key);
        }
        catch (Exception ex)
        {
            Transection.Rollback();
            throw ex;
        }
        finally
        {
            Transection.Dispose();
        }

        return res;
    }

    public double GetCheckingBalance(string chkAcctNum)
    {
        double res = 0;
        try
        {
            string sql = "select Balance from CheckingAccounts where " +
                "CheckingAccountNumber=@chkAcctNum";
            List<DbParameter> PList = new List<DbParameter>();
            DbParameter p1 = new MySqlParameter("@chkAcctNum", MySqlDbType.VarChar, 50);
            p1.Value = chkAcctNum;
            PList.Add(p1);
            object obj = _idataAccess.GetSingleAnswer(sql, PList);
            if (obj != null)
                res = double.Parse(obj.ToString());
        }
        catch (Exception ex)
        {
            throw ex;
        };
        return res;
    }

    public bool TransferChkToSavViaSP(string chkAcctNum, string savAcctNum,
           double amt)
    {
        string CONNSTR = ConfigurationManager.ConnectionStrings["BANKMYSQLCONN"].ConnectionString;
        bool res = false;
        MySqlConnection conn = new MySqlConnection(CONNSTR);
        try
        {
            conn.Open();
            string sql = "SPXferChkToSav"; // name of SP
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlParameter p1 = new MySqlParameter("@ChkAcctNum", MySqlDbType.VarChar, 50);

            cmd.CommandType = CommandType.StoredProcedure;
            p1.Value = chkAcctNum;
            cmd.Parameters.Add(p1);
            MySqlParameter p2 = new MySqlParameter("@SavAcctNum", MySqlDbType.VarChar, 50);
            p2.Value = savAcctNum;
            cmd.Parameters.Add(p2);
            MySqlParameter p3 = new MySqlParameter("@amt", MySqlDbType.Decimal);
            p3.Value = amt;
            cmd.Parameters.Add(p3);
            int rows = cmd.ExecuteNonQuery();
            if (rows == 3)
                res = true;

            // clear cache for TransferHistory
            string key = String.Format("TransferHistory_{0}", chkAcctNum);
            webCache.Remove(key);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return res;
    }


    public double GetSavingBalance(string savAcctNum)
    {
        double res = 0;
        try
        {
            string sql = "select Balance from SavingAccounts where " +
                "SavingAccountNumber=@savAcctNum";
            List<DbParameter> PList = new List<DbParameter>();
            DbParameter p1 = new MySqlParameter("@savAcctNum", MySqlDbType.VarChar, 50);
            p1.Value = savAcctNum;
            PList.Add(p1);
            object obj = _idataAccess.GetSingleAnswer(sql, PList);
            if (obj != null)
                res = double.Parse(obj.ToString());
        }
        catch (Exception ex)
        {
            throw ex;
        };
        return res;
    }

    #endregion

    #region IDataAccount Members
    public List<TransferHistory> GetTransferHistory(string chkAcctNum)
    {
        List<TransferHistory> TList = null;
        try
        {
            string key = String.Format("TransferHistory_{0}",
                chkAcctNum);
            TList = webCache.Retrieve<List<TransferHistory>>(key);
            if (TList == null)
            {
                //TList = new List<TransferHistory>();
                DataTable dt = GetTransferHistoryDB(chkAcctNum);
                TList = RepositoryHelper.ConvertDataTableToList<TransferHistory>(dt);
                //foreach (DataRow dr in dt.Rows)
                //{
                //    TransferHistory the = new TransferHistory();
                //    the.SetFields(dr);
                //    TList.Add(the);
                //}
                webCache.Insert(key, TList);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        };
        return TList;
    }



    #endregion

    public System.Data.DataTable GetTransferHistoryDB(string chkAcctNum)
    {
        DataTable dt = null;
        try
        {
            string sql = "select * from TransferHistory where " +
                "CheckingAccountNumber=@chkAcctNum";
            List<DbParameter> PList = new List<DbParameter>();
            DbParameter p1 = new MySqlParameter("@chkAcctNum", MySqlDbType.VarChar, 50);
            p1.Value = chkAcctNum;
            PList.Add(p1);
            dt = _idataAccess.GetDataTable(sql, PList);
        }
        catch (Exception ex)
        {
            throw ex;
        };
        return dt;
    }

    #region IDataAccount Members

    public string GetCheckingAccount(string userName)
    {
        string Account = "";
        if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");

        try
        {
            string sql = "select CheckingAccountNum from Users where Username=@userName";

            List<DbParameter> PList = new List<DbParameter>();
            DbParameter p1 = new MySqlParameter("@userName", MySqlDbType.VarChar, 50);
            p1.Value = userName;
            PList.Add(p1);
            object obj = _idataAccess.GetSingleAnswer(sql, PList);
            if (obj != null)
                Account = obj.ToString();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return Account;
    }
    #endregion
}
