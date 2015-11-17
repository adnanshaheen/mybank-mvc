using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.Common;

/// <summary>
/// Summary description for DataLayer
/// </summary>
public class Repository : IRepositoryDataAuthentication, IRepositoryDataAccount
{
    IDataAccess _idataAccess = null;
    CacheAbstraction webCache = null;

    public Repository(IDataAccess ida, CacheAbstraction webc)
    {
        _idataAccess = ida;
        webCache = webc;
    }

    public Repository()
        : this(GenericFactory<DataAccess,IDataAccess>.CreateInstance(),
        new CacheAbstraction())
    {
    }


    #region IDataAuthentication Members

    public string IsValidUser(string uname, string pwd)
    {
        string res = "";
        try
        {
            string sql = "select CheckingAccountNum from Users where " +
                "Username=@uname and Password=@pwd";
            List<DbParameter> PList = new List<DbParameter>();
            DbParameter p1 = new SqlParameter("@uname", SqlDbType.VarChar, 50);
            p1.Value = uname;
            PList.Add(p1);
            DbParameter p2 = new SqlParameter("@pwd", SqlDbType.VarChar, 50);
            p2.Value = pwd;
            PList.Add(p2);
            object obj = _idataAccess.GetSingleAnswer(sql, PList);
            if (obj != null)
                res = obj.ToString();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return res;

    }

    #endregion

    #region IDataAccount Members

    public bool TransferChkToSav(string chkAcctNum, string savAcctNum, double amt)
    {
        bool res = false;
        string CONNSTR = ConfigurationManager.ConnectionStrings["BANKDBCONN"].ConnectionString;
        SqlConnection conn = new SqlConnection(CONNSTR);
        SqlTransaction Transection = conn.BeginTransaction();

        try
        {
            double bal = GetCheckingBalance(chkAcctNum);
            if (bal > 0)
            {
                List<DbParameter> PList = new List<DbParameter>();
                double newBal = bal - amt;
                string sql = "Update CheckingAccounts set Balance=@newBal where CheckingAccountNumber=@ChkAcctNum";
                DbParameter p1 = new SqlParameter("@newBal", SqlDbType.Decimal);
                p1.Value = newBal;
                PList.Add(p1);

                DbParameter p2 = new SqlParameter("@ChkAcctNum", SqlDbType.VarChar, 50);
                p2.Value = chkAcctNum;
                PList.Add(p2);

                if (_idataAccess.InsOrUpdOrDel(sql, PList) > 0) // technically it should return 1
                {
                    //double.Parse(obj.ToString());
                    newBal = GetSavingBalance(savAcctNum) + amt;
                    sql = "Update SavingAccounts set Balance=@newBal where SavingAccountNumber=@SavAcctNum";

                    PList.Clear();              // clear plist to reuse
                    p1.Value = newBal;          // since p1 is same type of money, we can reuse it
                    PList.Add(p1);

                    DbParameter p3 = new SqlParameter("@SavAcctNum", SqlDbType.VarChar, 50);
                    p3.Value = savAcctNum;      // since p2 is same type of varchar, we can reuse it
                    PList.Add(p3);

                    if (_idataAccess.InsOrUpdOrDel(sql, PList) > 0) // it should return 1
                    {
                        sql = "insert into TransferHistory(FromAccountNum, ToAccountNum, Amount, CheckingAccountNumber)" +
                            "values (@ChkAcctNum, @SavAcctNum, @amt, @ChkAcctNum)";

                        PList.Clear();

                        p2.Value = chkAcctNum;
                        PList.Add(p2);

                        p3.Value = savAcctNum;
                        PList.Add(p3);

                        DbParameter p4 = new SqlParameter("@amt", SqlDbType.VarChar, 50);
                        p4.Value = amt;
                        PList.Add(p4);

                        res = _idataAccess.InsOrUpdOrDel(sql, PList) > 0 ? true : false;
                        if (res == true)
                            Transection.Commit();
                    }
                }
            }
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
            DbParameter p1 = new SqlParameter("@chkAcctNum", SqlDbType.VarChar, 50);
            p1.Value = chkAcctNum;
            PList.Add(p1);
            object obj = _idataAccess.GetSingleAnswer(sql,PList);
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
        string CONNSTR = ConfigurationManager.ConnectionStrings["BANKDBCONN"].ConnectionString;
        bool res = false;
        SqlConnection conn = new SqlConnection(CONNSTR);
        try
        {
            conn.Open();
            string sql = "SPXferChkToSav"; // name of SP
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlParameter p1 = new SqlParameter("@ChkAcctNum",
                                System.Data.SqlDbType.VarChar, 50);

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            p1.Value = chkAcctNum;
            cmd.Parameters.Add(p1);
            SqlParameter p2 = new SqlParameter("@SavAcctNum",
                                System.Data.SqlDbType.VarChar, 50);
            p2.Value = savAcctNum;
            cmd.Parameters.Add(p2);
            SqlParameter p3 = new SqlParameter("@amt",
                                System.Data.SqlDbType.Money);
            p3.Value = amt;
            cmd.Parameters.Add(p3);
            int rows = cmd.ExecuteNonQuery();
            if (rows == 3)
                res = true;
            
            // clear cache for TransferHistory
            string key = String.Format("TransferHistory_{0}",
                chkAcctNum);
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
            DbParameter p1 = new SqlParameter("@savAcctNum", SqlDbType.VarChar, 50);
            p1.Value = savAcctNum;
            PList.Add(p1);
            object obj = _idataAccess.GetSingleAnswer(sql,PList);
            if (obj != null)
                res = double.Parse(obj.ToString());
        }
        catch (Exception)
        {
            throw;
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
            DbParameter p1 = new SqlParameter("@chkAcctNum", SqlDbType.VarChar, 50);
            p1.Value = chkAcctNum;
            PList.Add(p1);
            dt = _idataAccess.GetDataTable(sql,PList);
        }
        catch (Exception ex)
        {
            throw ex;
        };
        return dt;
    }


    public bool UpdatePassword(string uname, string oldPW, string newPW)
    {
        string res = "";
        try
        {
            res = IsValidUser(uname, oldPW);
            string sql = "update Users set Password=@newPW where " +
                "Username=@uname and Password=@oldPW";
            List<DbParameter> PList = new List<DbParameter>();
            DbParameter p1 = new SqlParameter("@uname", SqlDbType.VarChar, 50);
            p1.Value = uname;
            PList.Add(p1);
            DbParameter p2 = new SqlParameter("@oldPW", SqlDbType.VarChar, 50);
            p2.Value = oldPW;
            PList.Add(p2);
            DbParameter p3 = new SqlParameter("@newPW", SqlDbType.VarChar, 50);
            p3.Value = newPW;
            PList.Add(p3);
            object obj = _idataAccess.GetSingleAnswer(sql, PList);
            if (obj != null)
                res = obj.ToString();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return true;
   }
}