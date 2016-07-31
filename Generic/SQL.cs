using System;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Diagnostics;
using Insignia;
using System.Collections;
using System.Data.Sql;
namespace Generic
{
   // 
   // Created Mid august 2002.
   //
   // This class is a compilation of functions and structures which allow me
   //  to write easy to maintain SQL queries.
   public class SQL
   {
      static public DataTable SortedTable(DataTable dt, string column, bool ASC)
      {
         DataTable dtClone = dt.Clone();
         dtClone.Clear();
         if (ASC)
            column = column + " ASC";
         else
            column = column + " DESC";
         DataRow[] rows = dt.Select("1=1", column);
         foreach (DataRow row in rows)
         {
            dtClone.ImportRow(row);
         }
         return dtClone;
      }
      static public string BuildConnectionString(string src,
        string db, string user_id, string password)
      {
         return "Data Source=" + src + "; Initial Catalog=" + db
           + "; User ID = " + user_id + "; Password = " + password;
      }
      static public void SetConnection(string connect)
      {
         DBOperation.GlobalDBConnectionStr = connect;
      }

      #region Paramlization Execution sql
      public static DataTable ExecuteSQLParamLization(string sql,params object[] paramList)
      {
          DataTable dt = new DataTable();

          SqlCommand command = new SqlCommand();
          command.CommandType = CommandType.Text;
          command.CommandText = sql;
          if (paramList != null)
          {
              for (int i = 0; i < paramList.Length; i= i+2)
              {
                  string strName = SQL.ToString(paramList[i]);
                  object oValue = paramList[i+1];
                  command.Parameters.Add("@" + strName, oValue);
                  //if (oValue is DateTime)
                  //{
                  //    command.Parameters.Add("@" + strName, SqlDbType.DateTime);
                  //}
                  //else if (oValue is String)
                  //{
                  //    command.Parameters.Add("@" + strName, SqlDbType.NVarChar);
                  //}
                  //else if (oValue is Int32)
                  //{
                  //    command.Parameters.Add("@" + strName, SqlDbType.Int);
                  //}

                  //command.Parameters["@" + strName].Value = oValue;
              }
          }

          DBOperation dbop = new DBOperation();
          dbop.SetCommand(command);
          dt = dbop.GetDataTable();
          return dt;
      }
      #endregion


      #region Functions for executing SQL queries.
      // Execute an SQL statement and return a DataTable with the information
      //  gathered. This function is best suited for executing SQL queries.



      static public DataTable ExecuteSQL_fill(string sql_statement)
      {
         try
         {
            DBOperation db_op = new DBOperation(sql_statement);
            DataTable table = new DataTable();
            db_op.Fill(table);
            db_op.Close();
            return table;
         }
         catch (Exception ex)
         {
            throw (ex);
         }
      }

      static public DataTable ExecuteSQL_fill(string format,
         params object[] obs)
      {
         return ExecuteSQL_fill(String.Format(format, obs));
      }


      static public void ExecuteAdminSQL(string sql_statement)
      {
         ExecuteAdminSQL(sql_statement, false);
      }
      static public void ExecuteAdminSQL(string sql_statement, bool ingoreException)
      {
         ExecuteAdminSQL(sql_statement, ingoreException, "master");
      }

      static public void ExecuteAdminSQL(string sql_statement, bool ingoreException, string databaseName)
      {

         try
         {
            DBOperation db_op = new DBOperation(sql_statement, databaseName);
            db_op.ExecuteNonQuery();
            db_op.Close();
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.ExecuteAdminSQL]" + ex.ToString());
            return;
         }
      }

      // Execute an SQL statement. The statements executed by this function
      //  should probably not be queries since no data is returned by
      //  this function but instead should be simple inserts, deletes, etc.


      static public void ExecuteSQL(string sql_statement)
      {
         try
         {
            DBOperation db_op = new DBOperation(sql_statement);
            db_op.ExecuteNonQuery();
            db_op.Close();
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.ExecuteSQL]" + ex.ToString());
            throw ex;
         }
      }
      static public void DeleteRecordRecursivelyWithoutLogging(string TableName, int values)
      {
         string sql = "";
         //Assuming we only using ID as parent Key.
         try
         {
            sql = string.Format(@"Delete {0} Where ID = {1}", TableName, values);
            ExecuteSQL(sql);
            return;
         }
         catch
         {
            //try to delete it directly, if it failed, we will try recursively.
         }

         sql = string.Format(@"
Select SO1.Name Child, SC1.Name ChildColumn, So2.Name Parent, SC2.Name ParentColumn
From sysforeignkeys SF JOIN SysObjects SO1 ON SF.fkeyid = SO1.id
JOIN SysObjects SO2 ON SF.rkeyid = SO2.id
JOIN syscolumns SC1 ON SF.fkey = SC1.colid And SC1.id = SO1.id
JOIN syscolumns SC2 ON SF.rkey = SC2.colid And SC2.id = SO2.id
WHERE SO2.Name = {0}", SQL.Q(TableName));
         DataTable dt = ExecuteSQL_fill(sql);
         if (dt != null)
         {
            foreach (DataRow dr in dt.Rows)
            {
               string childTable = dr["Child"].ToString();
               string childColumn = dr["ChildColumn"].ToString();
               try
               {
                  sql = string.Format(@"
Delete {0} Where {1} = {2}", childTable, childColumn, values);
                  ExecuteSQL(sql);
                  continue;
               }
               catch
               {
                  //if we can not delete it directly, do recursive thing
               }


               sql = string.Format(@"
Select Count(*)
From sysforeignkeys SF JOIN SysObjects SO1 ON SF.fkeyid = SO1.id
JOIN SysObjects SO2 ON SF.rkeyid = SO2.id
JOIN syscolumns SC1 ON SF.fkey = SC1.colid And SC1.id = SO1.id
JOIN syscolumns SC2 ON SF.rkey = SC2.colid And SC2.id = SO2.id
WHERE SO2.Name = {0}", SQL.Q(childTable));
               int count = ExecuteSQL_GetInt(sql, -1);
               if (count <= 0)
               {
                  sql = string.Format(@"
Delete {0} Where {1} = {2}", childTable, childColumn, values);
                  ExecuteSQL(sql);
                  continue;
               }
               sql = string.Format(@"
Select ID
From {0} Where {1} = {2}", childTable, childColumn, values);
               DataTable dt2 = ExecuteSQL_fill(sql);
               if (dt2 == null)
                  continue;
               foreach (DataRow dr2 in dt2.Rows)
               {
                  DeleteRecordRecursivelyWithoutLogging(childTable, SQL.ToInt(dr2["ID"]));
               }
            }
         }
         sql = string.Format(@"Delete {0} Where ID = {1}", TableName, values);
         ExecuteSQL(sql);
      }
      static public void WebDebug(string message)
      {

         //			string sql = "";
         //			sql = string.Format(@"Insert Into WebDebug(Message)
         //Values({0})",SQL.Q(message));
         //			ExecuteSQL(sql);

      }

      static public void ExecuteSQL(string format,
         params object[] obs)
      {
         ExecuteSQL(String.Format(format, obs));
      }

      // Execute an SQL statement. The statements executed by this function
      //  should probably not be queries since no data is returned by
      //  this function but instead should be simple inserts, deletes, etc.
      static public int ExecuteSQL_GetID(string sql_statement)
      {
         return ExecuteSQL_GetInt(sql_statement + " SELECT SCOPE_IDENTITY()");
      }

      static public int ExecuteSQL_GetRowCount(string sql_statement)
      {
         string oldSQL = sql_statement.Trim();
         sql_statement = sql_statement.ToLower().Trim();
         if (sql_statement.StartsWith("select *"))
         {
            oldSQL = "select count(*) " + oldSQL.Substring(8, oldSQL.Length - 8);
            return SQL.ExecuteSQL_GetInt(oldSQL);
         }
         else if (sql_statement.StartsWith("select"))
         {
            DataTable dt = SQL.ExecuteSQL_fill(oldSQL);
            if (dt == null)
               return 0;
            else
               return dt.Rows.Count;
         }
         else
            return ExecuteSQL_GetInt(oldSQL + " SELECT @@ROWCOUNT", 0);
      }

      static public int ExecuteSQL_GetID(string format,
         params object[] obs)
      {
         return ExecuteSQL_GetID(String.Format(format, obs));
      }
      #endregion


      #region Functions for retrieving a single value
      /// <summary>
      /// if the sql return null, the return value is false
      /// </summary>
      /// <param name="sql_statement"></param>
      /// <returns></returns>
      static public bool ExecuteSQL_GetBoolean(string sql_statement)
      {
         DataTable tmp = SQL.ExecuteSQL_fill(sql_statement);
         if (tmp.Rows.Count == 0)
         {
            return false;
         }
         return SQL.ToBoolean(tmp.Rows[0][0]);
      }
      /// <summary>
      /// if the sql return null, the return value is -1
      /// </summary>
      /// <param name="sql_statement"></param>
      /// <returns></returns>
      static public int ExecuteSQL_GetInt(string sql_statement)
      {
         DataTable tmp = SQL.ExecuteSQL_fill(sql_statement);
         if (tmp.Rows.Count == 0)
         {
            return -1;
         }
         return SQL.ToInt(tmp.Rows[0][0]);
      }
      /// <summary>
      /// if the sql return null, the return value will be def
      /// </summary>
      /// <param name="sql_statement"></param>
      /// <param name="def"></param>
      /// <returns></returns>
      static public int ExecuteSQL_GetInt(string sql_statement, int def)
      {
         DataTable tmp = SQL.ExecuteSQL_fill(sql_statement);
         if (tmp.Rows.Count == 0) return def;
         return SQL.ToInt(tmp.Rows[0][0]);
      }

      static public decimal ExecuteSQL_GetDecimal(string sql_statement, decimal def)
      {
         DataTable tmp = SQL.ExecuteSQL_fill(sql_statement);
         if (tmp.Rows.Count == 0) return def;
         return SQL.ToDecimal(tmp.Rows[0][0]);
      }

      static public double ExecuteSQL_GetDouble(string sql_statement, double def)
      {
         DataTable tmp = SQL.ExecuteSQL_fill(sql_statement);
         if (tmp.Rows.Count == 0) return def;
         return SQL.ToDouble(tmp.Rows[0][0]);
      }

      static public DateTime ExecuteSQL_GetDateTime(string sql_statement, DateTime dt)
      {
         DataTable tmp = SQL.ExecuteSQL_fill(sql_statement);
         if (tmp.Rows.Count == 0)
         {
            return dt;
         }
         return SQL.ToDate(tmp.Rows[0][0]);
      }
      static public DateTime ExecuteSQL_GetDateTime(string sql_statement)
      {
         DateTime dt = new DateTime(1900, 1, 1);
         return ExecuteSQL_GetDateTime(sql_statement, dt);
      }
      static public string ExecuteSQL_GetString(string sql_statement)
      {
         return ExecuteSQL_GetString(sql_statement, "");
      }

      static public string ExecuteSQL_GetString(string sql_statement, string def)
      {
         DataTable tmp = SQL.ExecuteSQL_fill(sql_statement);
         if (tmp == null)
            return def;
         if (tmp.Rows.Count == 0) return def;
         return SQL.ToString(tmp.Rows[0][0]);
      }
      #endregion

      #region FUNCTIONS FOR EXECUTING STORED PROCEDURES.

      #region EXECUTE STORED FUNCTIONS

      static public DataTable GetDataTableFromSP(string function_name,
         params StoredFunctionParameters[] list)
      {
         return GetDataTableFromSP(function_name, false, list);
      }

      static public DataTable GetDataTableFromSP(string function_name, bool ingoreException,
         params StoredFunctionParameters[] list)
      {
         try
         {
            DataTable dt = new DataTable();
            SqlCommand command = new SqlCommand(function_name);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;

            for (int i = 0; i < list.Length; i++)
               command.Parameters.Add(new SqlParameter("@" + list[i].name,
                  list[i].type, list[i].size,
                  list[i].direction, true, 0, 0, "", DataRowVersion.Default, 0));

            DBOperation op = new DBOperation(command);
            for (int i = 0; i < list.Length; i++)
               op.SetParameter("@" + list[i].name, list[i].v);
            dt = op.GetDataTable();
            op.Close();
            return dt;
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.GetDataTableFromSP]" + ex.ToString());
            throw ex;
         }
      }
      static public DataSet GetDataSetFromSP(string function_name, bool ingoreException,
   params StoredFunctionParameters[] list)
      {
         try
         {
            DataSet dt = new DataSet();
            SqlCommand command = new SqlCommand(function_name);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;

            for (int i = 0; i < list.Length; i++)
               command.Parameters.Add(new SqlParameter("@" + list[i].name,
                  list[i].type, list[i].size,
                  list[i].direction, true, 0, 0, "", DataRowVersion.Default, 0));

            DBOperation op = new DBOperation(command);
            for (int i = 0; i < list.Length; i++)
               op.SetParameter("@" + list[i].name, list[i].v);
            dt = op.GetDataSet();
            op.Close();
            return dt;
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.GetDataTableFromSP]" + ex.ToString());
            throw ex;
         }
      }

      static public DataTable GetDataTableFromSP(string function_name, ref Hashtable returnedPararmeterHT,
         params StoredFunctionParameters[] list)
      {
         try
         {
            DataTable dt = new DataTable();
            SqlCommand command = new SqlCommand(function_name);
            returnedPararmeterHT.Clear();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;

            for (int i = 0; i < list.Length; i++)
               command.Parameters.Add(new SqlParameter("@" + list[i].name,
                  list[i].type, list[i].size,
                  list[i].direction, true, 0, 0, "", DataRowVersion.Default, 0));

            DBOperation op = new DBOperation(command);
            for (int i = 0; i < list.Length; i++)
               op.SetParameter("@" + list[i].name, list[i].v);
            dt = op.GetDataTable();
            for (int i = 0; i < list.Length; i++)
            {
               try
               {
                  returnedPararmeterHT.Add(list[i].name, op.GetParameter("@" + list[i].name));
               }
               catch { }
            }
            op.Close();
            return dt;
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.GetDataTableFromSP]" + ex.ToString());
            return null;
         }
      }

      static public bool ExecuteStored(string function_name,
         params StoredFunctionParameters[] list)
      {
          
         try
         {
            SqlCommand command = new SqlCommand(function_name);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;

            for (int i = 0; i < list.Length; i++)
               command.Parameters.Add(new SqlParameter("@" + list[i].name,
                  list[i].type, list[i].size,
                  list[i].direction, true, 0, 0, "", DataRowVersion.Default, 0));

            DBOperation op = new DBOperation(command);
            for (int i = 0; i < list.Length; i++)
               op.SetParameter("@" + list[i].name, list[i].v);

            op.ExecuteNonQuery(6000);
            op.Close();
            return true;
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.ExecuteStored]" + ex.ToString());
            throw ex;
         }
      }
      static public bool ExecuteStoredGetBoolean(string function_name,
         StoredFunctionParameters ret,
         params StoredFunctionParameters[] list)
      {
         try
         {
            SqlCommand command = new SqlCommand(function_name);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;

            command.Parameters.Add(new SqlParameter("@" + ret.name, ret.type, ret.size,
               ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Default, 0));

            for (int i = 0; i < list.Length; i++)
               command.Parameters.Add(new SqlParameter("@" + list[i].name,
                  list[i].type, list[i].size,
                  list[i].direction, true, 0, 0, "", DataRowVersion.Default, 0));

            DBOperation op = new DBOperation(command);
            op.SetParameter("@" + ret.name, ret.v);
            for (int i = 0; i < list.Length; i++)
            {

               Trace.Write(String.Format("{0} -> {1}", "@" + list[i].name, list[i].v).ToString());
               op.SetParameter("@" + list[i].name, list[i].v);
            }

            op.ExecuteNonQuery();

            bool r = SQL.ToBoolean(op.GetParameter("@" + ret.name));
            op.Close();
            return r;
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.ExecuteStoredGetBoolean]" + ex.ToString());
            return false;
         }

      }
      static public Hashtable ExecuteStoredGetHashTable(string function_name,
         params StoredFunctionParameters[] list)
      {
         Hashtable ht = new Hashtable();
         try
         {
            SqlCommand command = new SqlCommand(function_name);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;

            for (int i = 0; i < list.Length; i++)
               command.Parameters.Add(new SqlParameter("@" + list[i].name,
                  list[i].type, list[i].size,
                  list[i].direction, true, 0, 0, "", DataRowVersion.Default, 0));

            DBOperation op = new DBOperation(command);
            for (int i = 0; i < list.Length; i++)
               op.SetParameter("@" + list[i].name, list[i].v);


            op.ExecuteNonQuery(6000);
            for (int i = 0; i < list.Length; i++)
            {
               try
               {
                  ht.Add(list[i].name, op.GetParameter("@" + list[i].name));
               }
               catch { }
            }
            op.Close();
            return ht;
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.ExecuteStoredGetHashTable]" + ex.ToString());
            throw (ex);
         }

      }
      static public int ExecuteStoredInt(string function_name,
   StoredFunctionParameters ret,
   params StoredFunctionParameters[] list)
      {
         return ExecuteStoredID(function_name, ret, list);
      }
      static public int ExecuteStoredID(string function_name,
         StoredFunctionParameters ret,
         params StoredFunctionParameters[] list)
      {
         try
         {
            SqlCommand command = new SqlCommand(function_name);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;

            command.Parameters.Add(new SqlParameter("@" + ret.name, ret.type, ret.size,
               ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Default, 0));

            for (int i = 0; i < list.Length; i++)
               command.Parameters.Add(new SqlParameter("@" + list[i].name,
                  list[i].type, list[i].size,
                  list[i].direction, true, 0, 0, "", DataRowVersion.Default, 0));

            DBOperation op = new DBOperation(command);
            op.SetParameter("@" + ret.name, ret.v);
            for (int i = 0; i < list.Length; i++)
            {

               Trace.Write(String.Format("{0} -> {1}", "@" + list[i].name, list[i].v).ToString());
               op.SetParameter("@" + list[i].name, list[i].v);
            }

            op.ExecuteNonQuery(6000);

            int r = (int)op.GetParameter("@" + ret.name);
            op.Close();
            return r;
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.ExecuteStoredID]" + ex.ToString());
            return -1;
         }
      }

      static public decimal ExecuteStoredDecimal(string function_name,
         StoredFunctionParameters ret,
         params StoredFunctionParameters[] list)
      {
         try
         {
            SqlCommand command = new SqlCommand(function_name);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;

            command.Parameters.Add(new SqlParameter("@" + ret.name, ret.type, 1000,
               ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Default, 0));

            for (int i = 0; i < list.Length; i++)
               command.Parameters.Add(new SqlParameter("@" + list[i].name,
                  list[i].type, list[i].size,
                  list[i].direction, true, 0, 0, "", DataRowVersion.Default, 0));

            DBOperation op = new DBOperation(command);
            op.SetParameter("@" + ret.name, ret.v);
            for (int i = 0; i < list.Length; i++)
            {
               //        Debug.WriteLine("{0} -> {1}",  "@" + list[i].name, list[i].v);
               op.SetParameter("@" + list[i].name, list[i].v);
            }

            op.ExecuteNonQuery(6000);

            decimal r = SQL.ToDecimal(op.GetParameter("@" + ret.name));
            op.Close();
            return r;
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.ExecuteStored]" + ex.ToString());
            return 0;
         }

      }
      static public DateTime ExecuteStoredGetDate(string function_name,
   StoredFunctionParameters ret,
   params StoredFunctionParameters[] list)
      {
         try
         {
            SqlCommand command = new SqlCommand(function_name);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;

            command.Parameters.Add(new SqlParameter("@" + ret.name, ret.type, 1000,
               ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Default, 0));

            for (int i = 0; i < list.Length; i++)
               command.Parameters.Add(new SqlParameter("@" + list[i].name,
                  list[i].type, list[i].size,
                  list[i].direction, true, 0, 0, "", DataRowVersion.Default, 0));

            DBOperation op = new DBOperation(command);
            op.SetParameter("@" + ret.name, ret.v);
            for (int i = 0; i < list.Length; i++)
            {
               //        Debug.WriteLine("{0} -> {1}",  "@" + list[i].name, list[i].v);
               op.SetParameter("@" + list[i].name, list[i].v);
            }

            op.ExecuteNonQuery(6000);

            DateTime r = SQL.ToDate(op.GetParameter("@" + ret.name));
            op.Close();
            return r;
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.ExecuteStored]" + ex.ToString());
            return DateTime.MinValue;
         }

      }
      static public string ExecuteStoredString(string function_name,
         StoredFunctionParameters ret,
         params StoredFunctionParameters[] list)
      {
         try
         {
            SqlCommand command = new SqlCommand(function_name);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;

            command.Parameters.Add(new SqlParameter("@" + ret.name, ret.type, 1000,
               ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Default, 0));

            for (int i = 0; i < list.Length; i++)
               command.Parameters.Add(new SqlParameter("@" + list[i].name,
                  list[i].type, list[i].size,
                  list[i].direction, true, 0, 0, "", DataRowVersion.Default, 0));

            DBOperation op = new DBOperation(command);
            op.SetParameter("@" + ret.name, ret.v);
            for (int i = 0; i < list.Length; i++)
            {
               //        Debug.WriteLine("{0} -> {1}",  "@" + list[i].name, list[i].v);
               op.SetParameter("@" + list[i].name, list[i].v);
            }

            op.ExecuteNonQuery(6000);

            string r = SQL.ToString(op.GetParameter("@" + ret.name));
            op.Close();
            return r;
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.ExecuteStored]" + ex.ToString());
            return null;
         }

      }
      static public int GetIdentity(string tablename)
      {
         try
         {
            SqlCommand command = new SqlCommand("GetCounter");

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;

            command.Parameters.Add(new SqlParameter("@TableName", SqlDbType.NVarChar, 50, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, 0));
            command.Parameters.Add(new SqlParameter("@Current", SqlDbType.Int, 4, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, 0));

            DBOperation op = new DBOperation(command);
            int Id = 0;
            op.SetParameter("@TableName", tablename);
            op.SetParameter("@Current", Id);
            op.ExecuteNonQuery();
            return (int)op.GetParameter("@Current");
         }
         catch (Exception ex)
         {
            Debug.WriteLine("[SQL.GetIdentity]" + ex.ToString());
            return 0;
         }

      }
      #endregion

      #region STORED FUNCTION PARAMETERS CLASS
      public class StoredFunctionParameters
      {
         public ParameterDirection direction = ParameterDirection.Input;
         public string name;
         public SqlDbType type;
         public int size;
         public object v;
      };
      static public StoredFunctionParameters SFP(string parameter_name,
         SqlDbType parameter_type, int parameter_size, object parameter_value, ParameterDirection parameter_direction)
      {
         StoredFunctionParameters tmp = new StoredFunctionParameters();
         tmp.name = parameter_name;
         tmp.type = parameter_type;
         tmp.size = parameter_size;
         tmp.v = parameter_value;
         tmp.direction = parameter_direction;
         return tmp;
      }

      static public StoredFunctionParameters SFP(string parameter_name,
         SqlDbType parameter_type, int parameter_size, object parameter_value)
      {
         StoredFunctionParameters tmp = new StoredFunctionParameters();
         tmp.name = parameter_name;
         tmp.type = parameter_type;
         tmp.size = parameter_size;
         tmp.v = parameter_value;
         return tmp;
      }

      static public StoredFunctionParameters SFP(string parameter_name, object o)
      {
         StoredFunctionParameters tmp = new StoredFunctionParameters();
         tmp.name = parameter_name;
         tmp.type = GetType(o);
         tmp.size = GetSize(o);
         tmp.v = GetValue(o);
         return tmp;
      }

      static public StoredFunctionParameters SFP(string parameter_name, string data)
      {
         StoredFunctionParameters tmp = new StoredFunctionParameters();
         tmp.name = parameter_name;
         tmp.type = SqlDbType.NVarChar;
         if (data == null)
         {
            tmp.size = 1;
            tmp.v = DBNull.Value;
         }
         else
         {
            tmp.size = data.Length;
            tmp.v = data;
         }
         return tmp;
      }
      static public StoredFunctionParameters SFP(string parameter_name, string data, ParameterDirection parameter_direction)
      {
         StoredFunctionParameters tmp = new StoredFunctionParameters();
         tmp.name = parameter_name;
         tmp.type = SqlDbType.NVarChar;
         tmp.size = 4;
         tmp.v = data;
         tmp.direction = parameter_direction;
         return tmp;
      }
      static public StoredFunctionParameters SFP(string parameter_name, int data)
      {
         return SFP(parameter_name, data, ParameterDirection.Input);
      }
      static public StoredFunctionParameters SFP(string parameter_name, int data, ParameterDirection parameter_direction)
      {
         StoredFunctionParameters tmp = new StoredFunctionParameters();
         tmp.name = parameter_name;
         tmp.type = SqlDbType.Int;
         tmp.size = 4;
         tmp.v = data;
         tmp.direction = parameter_direction;
         return tmp;
      }
      static public StoredFunctionParameters SFP(string parameter_name, bool data)
      {
         return SFP(parameter_name, data, ParameterDirection.Input);
      }

      static public StoredFunctionParameters SFP(string parameter_name, bool data, ParameterDirection parameter_direction)
      {
         StoredFunctionParameters tmp = new StoredFunctionParameters();
         tmp.name = parameter_name;
         tmp.type = SqlDbType.Bit;
         tmp.size = 4;
         tmp.v = data;
         tmp.direction = parameter_direction;
         return tmp;
      }
      static public StoredFunctionParameters SFP(string parameter_name, Decimal data)
      {
         return SFP(parameter_name, data, ParameterDirection.Input);
      }
      static public StoredFunctionParameters SFP(string parameter_name, Decimal data, ParameterDirection parameter_direction)
      {
         StoredFunctionParameters tmp = new StoredFunctionParameters();
         tmp.name = parameter_name;
         tmp.type = SqlDbType.Decimal;
         tmp.size = 9;
         tmp.v = data;
         tmp.direction = parameter_direction;
         return tmp;
      }

      static public StoredFunctionParameters SFP(string parameter_name, DateTime data)
      {
         return SFP(parameter_name, data, ParameterDirection.Input);
      }
      static public StoredFunctionParameters SFP(string parameter_name, DateTime data, ParameterDirection parameter_direction)
      {
         StoredFunctionParameters tmp = new StoredFunctionParameters();
         tmp.name = parameter_name;
         tmp.type = SqlDbType.DateTime;
         tmp.size = 8;
         tmp.v = data;
         tmp.direction = parameter_direction;
         return tmp;
      }

      static public StoredFunctionParameters SFP(string parameter_name, byte[] data)
      {
         return SFP(parameter_name, data, ParameterDirection.Input);
      }
      static public StoredFunctionParameters SFP(string parameter_name, byte[] data, ParameterDirection parameter_direction)
      {
         StoredFunctionParameters tmp = new StoredFunctionParameters();
         tmp.name = parameter_name;
         tmp.type = SqlDbType.Image;
         tmp.size = data.Length;
         tmp.v = data;
         tmp.direction = parameter_direction;
         return tmp;
      }
      //----- for maybe "null" case
      static public StoredFunctionParameters SFP(string parameter_name, object data, SqlDbType type, int size)
      {
         return SFP(parameter_name, data, type, size, ParameterDirection.Input);
      }
      static public StoredFunctionParameters SFP(string parameter_name, object data, SqlDbType type, int size, ParameterDirection parameter_direction)
      {
         StoredFunctionParameters tmp = new StoredFunctionParameters();
         tmp.name = parameter_name;
         tmp.type = type;
         tmp.size = size;
         tmp.v = GetValue(data);
         tmp.direction = parameter_direction;
         return tmp;
      }
      #endregion

      #region GET FUNCTIONS

      static SqlDbType GetType(object o)
      {
         string type = o.GetType().ToString();
         switch (type)
         {
            case "System.Byte":
               return SqlDbType.TinyInt;

            case "System.Int16":
            case "System.Int32":
               return SqlDbType.Int;

            case "System.Int64":
               return SqlDbType.BigInt;

            case "System.Single":
            case "System.Double":
            case "System.Decimal":
               return SqlDbType.Real;

            case "System.Boolean":
               return SqlDbType.Bit;

            case "System.Char":
               return SqlDbType.Char;

            case "System.DateTime":
               return SqlDbType.DateTime;

            case "System.DBNull":
            case "System.String":
            case "System.Guid":
               return SqlDbType.NVarChar;

            default:
               {
                  return SqlDbType.Variant;
               }
         }
      }

      static int GetSize(object o)
      {
         string type = o.GetType().ToString();
         switch (type)
         {
            case "System.DBNull":
               return 0;

            case "System.Boolean":
            case "System.Char":
            case "System.Byte":
               return 1;

            case "System.Int16":
            case "System.Int32":
               return 4;

            case "System.Int64":
            case "System.Double":
            case "System.Decimal":
            case "System.Single":
            case "System.DateTime":
               return 8;

            case "System.String":
               return ((string)o).Length;
            case "System.Guid":
               return o.ToString().Length;
            default:
               {
                  return o.ToString().Length;
               }
         }
      }

      static object GetValue(object o)
      {
         if (o == null)
            return DBNull.Value;

         string type = o.GetType().ToString();
         switch (type)
         {
            case "System.DBNull":
               return DBNull.Value;

            default:
               return o;
         }
      }
      #endregion

      #endregion

      #region DATATABLE TO DATABASE COMMUNICATION AND VICE-VERSA
      static public void saveWithStored(string storedProcedureName,
         DataTable data, params StoredFunctionParameters[] constants)
      {
         SqlCommand command = new SqlCommand(storedProcedureName);
         command.CommandType = CommandType.StoredProcedure;
         command.CommandTimeout = 3600;

         // add set of constants as parameters.
         for (int i = 0; i < constants.Length; i++)
         {
            command.Parameters.Add(new SqlParameter("@" + constants[i].name,
               constants[i].type, constants[i].size,
               ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Default, 0));
         }

         for (int i = 0; i < data.Columns.Count; i++)
         {
            DataColumn col = data.Columns[i];
            command.Parameters.Add(new SqlParameter("@" + col.ColumnName,
               SqlDbType.Int, 4,
               ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Default, 0));
         }

         foreach (DataRow row in data.Rows)
         {
            DBOperation op = new DBOperation(command);

            for (int i = 0; i < constants.Length; i++)
               op.SetParameter("@" + constants[i].name, constants[i].v);

            foreach (DataColumn col in data.Columns)
            {
               op.SetParameter("@" + col.ColumnName, row[col]);
            }

            op.ExecuteNonQuery();
            op.Close();
         }
      }
      #endregion

      #region DATABASE DATATYPE CONVERSION.
      // Normalizes a database object. In other words convert any DBNull object
      //  returned by the database to an actual null object. All other objects are
      //  left as is.
      static public object Null(object o)
      {
         if (o.GetType().Equals(typeof(System.DBNull))) return null;
         return o;
      }
      static public DateTime ToDate(object o, DateTime defaults, bool withMilisecond)
      {
         if (defaults.CompareTo(new DateTime(1753, 1, 1)) < 0)
            defaults = new DateTime(1753, 1, 1);
         if (o == null) return defaults;
         if (o.GetType().Equals(typeof(System.DBNull))) return defaults;
         try
         {
            if (!withMilisecond)
               return DateTime.Parse(o.ToString());
            else
               return (DateTime)o;
         }
         catch
         {
            return defaults;
         }
      }
       static public DateTime ToDate(object o, DateTime defaults)
       {
          return ToDate(o, defaults, false);
       }
       static public DateTime ToSmallDate(object o, DateTime defaults)
       {
           if (defaults.CompareTo(new DateTime(1900, 1, 1)) < 0)
               defaults = new DateTime(1900, 1, 1);
           else if (defaults.CompareTo(new DateTime(2079, 6, 6)) > 0)
               defaults = new DateTime(2079, 6, 6);

           if (o == null)
               return defaults;

           if (o.GetType().Equals(typeof(System.DBNull))) return defaults;
           try
           {
               DateTime dt = DateTime.Parse(o.ToString());
               if (dt.CompareTo(new DateTime(1900, 1, 1)) < 0)
                   dt = new DateTime(1900, 1, 1);
               else if (dt.CompareTo(new DateTime(2079, 6, 6)) > 0)
                   dt = new DateTime(2079, 6, 6);
               return dt;
           }
           catch
           {
               return defaults;
           }
       }
      static public bool IsDate(object o)
      {
         if (o == null) return false;
         if (o.GetType().Equals(typeof(System.DBNull))) return false;
         try
         {
             DateTime.Parse(o.ToString());
             return true;
         }
         catch
         {
            return false;
         }
      }
      static public DateTime ToDate(object o)
      {
         return ToDate(o, new DateTime(1800, 1, 1));
      }
       static public DateTime ToSmallDate(object o)
       {
           return ToSmallDate(o, new DateTime(1900, 1, 1));
       }
      static public bool ToBoolean(object o)
      {
         if (o == null) return false;
         if (o.GetType().Equals(typeof(System.DBNull))) return false;
         if (o.GetType().Equals(typeof(System.Boolean))) return (bool)o;
         if (o.GetType().Equals(typeof(System.String)))
         {
            if (o.ToString().Trim().ToLower() == "false" || o.ToString().Trim().ToLower() == "0" || o.ToString().Trim().ToLower() == "")
               return false;
            else
               return true;
         }
         return (ToInt(o) != 0);
      }
      static public Double ToDouble(object o)
      {
         return ToDouble(o, 0.0);
      }
      static public Double ToDouble(object o, double defaults)
      {
         if (o == null)
            return (System.Double)0;
         if (o.GetType().Equals(typeof(System.DBNull)))
            return (System.Double)0;
         if (o.GetType().Equals(typeof(System.Double)))
            return (System.Double)o;
         if (o.GetType().Equals(typeof(System.Single)))
            return (System.Double)(System.Single)o;
         else if (o.GetType().Equals(typeof(System.Decimal)))
            return (System.Double)(System.Decimal)o;
         else if (o.GetType().Equals(typeof(System.Int64)))
            return Double.Parse(o.ToString());
         else if (o.GetType().Equals(typeof(System.Int32)))
            return Double.Parse(o.ToString());
         else if (o.GetType().Equals(typeof(System.Int16)))
            return Double.Parse(o.ToString());
         else if (o.GetType().Equals(typeof(System.String)))
         {
            try
            {
               return Double.Parse(o.ToString(), System.Globalization.NumberStyles.Currency);
            }
            catch
            {
               return defaults;
            }
         }
         else
         {
            return (System.Double)o;
         }

         //throw ( new Exception(String.Format("SQL.ToDouble: Can't handle {0}.",
         //  o.GetType().ToString())));
      }
      static public Decimal ToDecimal(object o)
      {
         return ToDecimal(o, (System.Decimal)0);
      }
      static public Decimal ToDecimal(object o, decimal defaults)
      {
         if (o == null) return defaults;
         if (o.GetType().Equals(typeof(System.DBNull))) return defaults;

         if (o.GetType().Equals(typeof(System.Single)))
            return (System.Decimal)(System.Single)o;

         if (o.GetType().Equals(typeof(System.Double)))
            return (System.Decimal)(System.Double)o;
         try
         {
            return Decimal.Parse(o.ToString(), System.Globalization.NumberStyles.Currency);
         }
         catch
         {
            return defaults;
         }

      }

      // Converts any of the database value types (int32, tiny int, byte, etc.) to
      //  an integer.
      static public int ToInt(object o)
      {
         return ToInt(o, 0);  // if no default provided, we use 0 as defaut if exception occur or null value
      }
      // Converts any of the database value types (int32, tiny int, byte, etc.) to
      //  an integer.
      static public int ToInt(object o, int defaultInt)
      {
         if (o == null)
            return defaultInt;

         if (o.GetType().Equals(typeof(System.Boolean)))
         {
            if (((bool)o) == true)
               return 1;
            else
               return 0;
         }

         if (o.GetType().Equals(typeof(System.Int32)))
            return (int)o;

         try
         {
            if (o.GetType().Equals(typeof(System.String)))
            {
               if (o.ToString().Trim().Length <= 0 || o.ToString().Trim() == string.Empty)
               {
                  return defaultInt;
               }else
               {
                  return (int)Int32.Parse(o.ToString());
               }
            }

         }
         catch (ArgumentNullException)
         {
            return defaultInt;
         }
         catch (FormatException)
         {
            return defaultInt;
         }
         catch (OverflowException)
         {
            return defaultInt;
         }
         catch
         {
            return defaultInt;
         }

         if (o.GetType().Equals(typeof(System.Int16)))
            return (Int16)o;

         if (o.GetType().Equals(typeof(System.Decimal)))
            return System.Decimal.ToInt32((System.Decimal)o);

         if (o.GetType().Equals(typeof(System.Byte)))
            return (int)((Byte)o);

         if (o.GetType().Equals(typeof(System.Single)))
            return (int)((Single)o);

         if (o.GetType().Equals(typeof(System.DBNull)))
            return defaultInt;

         if (o.GetType().Equals(typeof(FG_Data)))
            return (int)((FG_Data)o).id;

         if (o.GetType().Equals(typeof(double)))
            return (int)((double)o);
        try
        {
            return (int)Int32.Parse(o.ToString());
        }
        catch
        {
            // Why THROW ERROR!!!
            throw (new Exception(String.Format("SQL.ToInt: Can't handle {0}.",
               o.GetType().ToString())));
        }
      }

      // Converts any of the database value types (int32, tiny int, byte, etc.) to
      //  an integer.
      static public string ToString(object o)
      {
         if (o == null) return string.Empty;
         if (o.GetType().Equals(typeof(System.DBNull)))
            return string.Empty;

         return o.ToString();
      }

      static public char ToChar(object o)
      {
         if (o == null)
            return ' ';

         if (o.GetType().Equals(typeof(System.Char)))
            return ((char)o);
         else if (o.GetType().Equals(typeof(System.String)))
            return (o.ToString().ToCharArray())[0];
         else

         return ' ';
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="o"></param>
      /// <param name="fileName"></param>
      /// <param name="overwrite"></param>
      public static void SaveToFile(Byte[] o, string fileName, bool overwrite)
      {
         if (o == null)
            return;
         FileStream file = null;
         if (overwrite)
            file = new FileStream(fileName, FileMode.Create);
         else
            file = new FileStream(fileName, FileMode.Append);
         file.Write(o, 0, o.Length);
         file.Close();
      }



      #endregion

      #region Standard search string
      static public string StandardSearchstring(string str)
      {
         str = str.Replace("'", "''");
         return str;
      }

      /// <summary>
      /// Returns the string "'abc''a'" when you give it "abc'a".
      /// </summary>
      static public string Str(string str)
      {
         return "'" + str.Replace("'", "''") + "'";
      }

      /// <summary>
      /// Returns the string "'2001-12-29'" when you give it a datetime
      /// corresponding to that date.
      /// </summary>
      static public string Date(object o)
      {
         DateTime date = (DateTime)o;
         return "'" + date.ToString("yyyy-MM-dd") + "'";
      }
      static public string DateAndTime(object o)
      {
         DateTime date = (DateTime)o;
         return "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
      }
      #endregion

      #region Extra Stuff

      static public string ObjectToSQLField(object o)
      {
         if (o == null)
            return "''";

         string type = o.GetType().ToString();
         switch (type)
         {

            case "System.Int16":
            case "System.Int32":
            case "System.Int64":
               return SQL.ToInt(o).ToString();

            case "System.Single":
            case "System.Double":
            case "System.Decimal":
               return SQL.ToDecimal(o).ToString();

            case "System.DBNull":
               return "NULL";
            case "System.String":
               return "'" + SQL.ToString(o).Replace("'", "''") + "'";
            case "System.DateTime":
               return DateAndTime(o);
            case "System.Guid":
               return "'" + ((Guid)o).ToString() + "'";
            case "System.Boolean":
               if (SQL.ToBoolean(o))
                  return "1";
               else
                  return "0";
            case "System.Char":
               if (o.ToString().Length >= 1)//Gang&Leo add Char function June 20, 2006
                  return "'" + SQL.ToString(o).Substring(0, 1).Replace("'", "''") + "'";
               else
                  return "''";
            default:
               return "''";
         }
      }
      /// <summary>
      /// Returns the string "'abc''a'" when you give it "abc'a".
      /// </summary>
      /// 
      static public string Q(object o)
      {
         if (o == null)
            return "''";

         string type = o.GetType().ToString();
         switch (type)
         {
            case "System.DBNull":
               return "''";
            case "System.String":
               return "'" + SQL.ToString(o).Replace("'", "''") + "'";
            case "System.DateTime":
               return DateAndTime(o);
            case "System.Guid":
               return "'" + ((Guid)o).ToString() + "'";
            case "System.Boolean":
               return "'" + ((bool)o).ToString() + "'";
            case "System.Char":
               if (o.ToString().Length >= 1)//Gang&Leo add Char function June 20, 2006
                  return "'" + SQL.ToString(o).Substring(0, 1).Replace("'", "''") + "'";
               else
                  return "''";
            default:
               return "''";
         }
      }
      #endregion
   }
}
