﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.Security;

namespace MyBankMVC15.DataAccess
{
    public class AuthhenticationServiceMySql : IAuthenticationService
    {
        private IDataAccess _idataAccess;

        public AuthhenticationServiceMySql()
        {
            _idataAccess = GenericFactory<DataAccessMySql, IDataAccess>.CreateInstance();
        }

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

            string sql = "select Username from Users where Username='" +
                                 userName + "' and Password='" + password + "'";
            string connStr = ConfigurationManager.ConnectionStrings["BANKDBCONN"].ConnectionString;
            MySqlConnection conn = new MySqlConnection(connStr);
            object obj = null;
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                obj = cmd.ExecuteScalar();
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
            if (obj != null)
            {
                //HttpContext.Current.Session[SessionKeys.USERID] = obj;
                return true;
            }
            else
                return false;
        }
    }
}