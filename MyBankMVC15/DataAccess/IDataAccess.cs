using System.Collections.Generic;
using System.Data;
using System.Data.Common;

/// <summary>
/// Summary description for IDataAccess
/// </summary>
public interface IDataAccess
{
	object GetSingleAnswer(string sql,List<DbParameter> PList);
    DataTable GetDataTable(string sql, List<DbParameter> PList);
    int InsOrUpdOrDel(string sql, List<DbParameter> PList);
}