using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using MyBankMVC15.DataAccess;
using System.Web.Security;
using System.Web;

/// <summary>
/// Summary description for RespositoryMySql
/// </summary>
public class RepositoryMySql : IRepositoryDataAccount, IAuthenticationService
{
    IDataAccess _idataAccess = null;
    CacheAbstraction webCache = null;

	public RepositoryMySql(IDataAccess idac, CacheAbstraction webc)
	{
        _idataAccess = idac;
        webCache = webc;
	}

    public RepositoryMySql()
        : this(GenericFactory<DataAccessMySql, IDataAccess>.CreateInstance(),
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
            DbParameter p2 = new MySqlParameter("@chkAcctNum", MySqlDbType.VarChar, 50);
            p2.Value = chkAcctNum;
            cmd2.Parameters.Add(p2);
            cmd2.Transaction = Transection;
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
            if (Transection != null)
                Transection.Rollback();
            throw ex;
        }
        finally
        {
            if (Transection != null)
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

    #region IAuthenticate
    public string GetRolesForUser(string uname)
    {
        string roles = "";
        string connStr = ConfigurationManager.ConnectionStrings["BANKMYSQLCONN"].ConnectionString;
        MySqlConnection conn = new MySqlConnection(connStr);

        try
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("GetUserRoles", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new MySqlParameter("@Username", uname));
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                roles += reader["RoleName"].ToString() + "|";
            if (roles != "")  // remove last "|"
                roles = roles.Substring(0, roles.Length - 1);
            conn.Close();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            conn.Close();
        }
        return roles;

    }

    public bool SignIn(string userName, string password, bool createPersistentCookie)
    {
        bool bret = false;
        if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
        try
        {
            //-----------Create authentication cookie----
            if (ValidateUser(userName, password) == true)
            {
                string roles = GetRolesForUser(userName);//pipe or comma delimited role list - add later
                FormsAuthenticationTicket authTicket     // cookie timeout is also set
                        = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(5), false, roles);
                //  encrypt the ticket
                string encryptedTicket =
                    FormsAuthentication.Encrypt(authTicket);

                // add the encrypted ticket to the cookie as data
                HttpCookie authCookie = new HttpCookie
                    (FormsAuthentication.FormsCookieName, encryptedTicket);
                System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                bret = true;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return bret;

    }

    public bool ChangePassword(string userName, string password, string newPassword)
    {
        bool bRes = false;
        if (String.IsNullOrEmpty(userName))
            throw new ArgumentException("Value cannot be null or empty.", "userName");
        if (String.IsNullOrEmpty(password))
            throw new ArgumentException("Value cannot be null or empty.", "password");
        if (String.IsNullOrEmpty(newPassword))
            throw new ArgumentException("Value cannot be null or empty.", "newPassword");

        try
        {
            // user is already validated in AuthController.cs
            // Hence we don't need to validate again
            string sql = "update Users set Password=@newPassword where " +
                 "Username=@userName and Password=@password";
            List<DbParameter> PList = new List<DbParameter>();
            DbParameter p1 = new MySqlParameter("@uname", MySqlDbType.VarChar, 50);
            p1.Value = userName;
            PList.Add(p1);
            DbParameter p2 = new MySqlParameter("@userName", MySqlDbType.VarChar, 50);
            p2.Value = password;
            PList.Add(p2);
            DbParameter p3 = new MySqlParameter("@newPassword", MySqlDbType.VarChar, 50);
            p3.Value = newPassword;
            PList.Add(p3);
            object obj = _idataAccess.GetSingleAnswer(sql, PList);
            if (obj != null)
            {
                // Check the obj, it should have the userName
                bRes = true;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return bRes;
    }

    public void SignOut()
    {
        FormsAuthentication.SignOut();
    }

    public bool ValidateUser(string userName, string password)
    {
        if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
        if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

        object obj = null;
        try
        {
            string sql = "select Username from Users where Username=@userName" +
                " and Password=@password";
            List<DbParameter> PList = new List<DbParameter>();
            DbParameter p1 = new MySqlParameter("@userName", MySqlDbType.VarChar, 50);
            p1.Value = userName;
            PList.Add(p1);
            DbParameter p2 = new MySqlParameter("@password", MySqlDbType.VarChar, 50);
            p2.Value = password;
            PList.Add(p2);

            obj = _idataAccess.GetSingleAnswer(sql, PList);
        }
        catch (Exception ex)
        {
            throw ex;
        }

        if (obj != null)
        {
            //HttpContext.Current.Session[SessionKeys.USERID] = obj;
            return true;
        }
        else
            return false;
    }
    #endregion
}
