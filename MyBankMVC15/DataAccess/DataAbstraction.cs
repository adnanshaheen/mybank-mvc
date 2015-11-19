using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MyBankMVC15.DataAccess
{
    public class DataAbstraction : IDataAbstraction
    {
        IDataAccess _idac = null;
        public DataAbstraction() : this(new DataAccessMySql())
        {
        }

        public DataAbstraction(IDataAccess idac)
        {
            this._idac = idac;
        }

        public object GetSingleAnswer(string sql, List<DbParameter> PList)
        {
            try
            {
                return _idac.GetSingleAnswer(sql, PList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public System.Data.DataTable GetDataTable(string sql, List<DbParameter> PList)
        {
            try
            {
                return _idac.GetDataTable(sql, PList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int InsOrUpdOrDel(string sql, List<DbParameter> PList)
        {
            try
            {
                return _idac.InsOrUpdOrDel(sql, PList);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}