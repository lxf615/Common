 
using System;
using System.Data; 
using System.Diagnostics;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
//using System.Drawing;

namespace Insignia
{	
	/// Database type enum
	public enum DatabaseType
	{	
		Oracle,
		SQLServer
	}
}

#if !Oracle

namespace Insignia
{
	
	/// <summary>
	/// Generic Class for process SQL Server database operations, after using this class,
	/// always call the Close method to release resource
	/// </summary>
	/// 
	/// <example>
	///	1. Populate datatable
	///		DBOpration dbop;
	///		string Sql = "select * from copies";
	///		dbop = new DBOperation(Sql);
	///		DataTable dt = dbop.GetDataTable();
	///	Or:
	///		DataTable dt = new DataTable();
	///		dbop.Fill(dt);
	///		dbop.Close();
	///	
	///	2. Run Sql statement
	///		string Sql = "Update Patrons set FirstName='John' where PatronID=" + 120;
	///		DBOperation dbop = new DBOperation(Sql);
	///		dbop.ExecuteNonQuery;
	///		dbop.Close();
	///		
	///	3. Using SqlDataReader (Better performance than using dataTable)
	///		DBOpration dbop;
	///		string Sql = "select CopyKey from copies";
	///		dbop = new DBOperation(Sql);
	///		dbop.ExecuteReader;
	///		string CopyKey;
	///		while (dbop.Read())
	///		{
	///			CopyKey = dbop.GetString("CopyKey");
	///		}
	///		dbop.Close();
	///	
	///	4. Using DataAdapter
	///		DBOperation dbop;
	///		dbop = new DBOperation(SPCommand.SaveCopy,SPCommand.DeleteCopy)
	///		dbop.Update(dt);
	///		dbop.Close();
	///		
	///	5. Run Sql stored procedure
	///		DBOperation dbop;
	///		dbop = new DBOperation(SPCommand.SaveCopy);
	///		dbop.SetParameter("@CopyID",120);
	///		dbop.SetParameter("@Price",12.12);
	///		dbop.SetParameter("@CallNo",'PR 120');
	///		dbop.ExecuteNonQuery();
	///		dbop.Close();
	///		
	///</example>
	///
	public class DBOperation : IDisposable  
	{
		/// <summary>
		/// Sql connection object
		/// </summary>
		private  SqlConnection  DBConnection; 

		/// <summary>
		/// Sql Connection string for this application
		/// </summary>
      private static string DBConnectionStr = "";
      private static string MasterDBConnectionStr = "";
      private static string MSDBConnectionStr = "";

		/// <summary>
		/// Define database type
		/// </summary>
		public static DatabaseType DBType = DatabaseType.SQLServer ;

		/// <summary>
		/// SqlDataAdapter object
		/// </summary>
		private SqlDataAdapter  SqlAdapter ;

		/// <summary>
		/// Transaction object
		/// </summary>
		private SqlTransaction DBTransaction; 

		/// <summary>
		/// Sql Command to execute query
		/// </summary>
		private SqlCommand Command;
		private string Sql;

		/// <summary>
		/// Specify if we need transaction control
		/// </summary>
		private  bool IsTransaction = false;

		/// <summary>
		/// Specify if use tempory connection
		/// </summary>
		// private bool UseTempCnnection = false;

		/// <summary>
		/// Boolean variable to determine if we need build SqlAdapter command
		/// </summary>
		private bool NeedBuildAdapterCommand = true;

		/// <summary>
		/// SqlDataAdapter's insert and update command
		/// </summary>
		private SqlCommand  SaveCommand;

    /// <summary>
		/// SqlDataAdapter's delete command
		/// </summary>
		private SqlCommand  DeleteCommand;


      /// <summary>
      /// StoredProcedure Default Timeout  
      /// by niko 2010-11-25
      /// </summary>
      private int STOREDPROCEDURE_TIMEOUT = 60;


		/// <summary>
		/// Instantiates a new instance of DBOperation calss using the SQL statement
		/// and CommandType
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="commandType"></param>
		public DBOperation(string sql, CommandType commandType)
			: this()
		{
			
			Sql = sql;
			Command = new SqlCommand(Sql, DBConnection);
			Command.CommandText  = Sql;
			Command.CommandType = commandType;
//			if (commandType == CommandType.StoredProcedure )
//			{
//				CreateParameters(Command, Sql);
//			}
		}
      public static bool LoginWithNTSecurity()
      {
         try
         {
            System.Data.SqlClient.SqlConnection sqlConn;
            if (Global.gsvServerName == "")
               return false;
            Global.gsvServerConn = "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;"
                + "Data Source=" + Global.gsvServerName + ";"
                + "Initial Catalog=" + Global.gsvDataBaseName;
            DBOperation.GlobalDBConnectionStr = "Integrated Security=SSPI;Data Source=" + Global.gsvServerName
                + ";Initial Catalog=" + Global.gsvDataBaseName;

            sqlConn = new System.Data.SqlClient.SqlConnection(DBOperation.GlobalDBConnectionStr);
            sqlConn.Open();
            sqlConn.Close();
            return true;
         }
         catch  
         {
            return false;
         }
      }
      public static void SaveNTSecurity(bool bValue)
      {
         String strInfo;
         if (bValue)
         {
            strInfo = "YES";
         }
         else
            strInfo = "NO";
         RegEdit.SetRegValue(Registry.LocalMachine, Insignia.Global.AppRegFolder, "NTSecurity", strInfo, 3);
      }
      public static bool LoginToCenterServer()
      { //Do not use this function, it is for DbSync Only
         try
         {
            System.Data.SqlClient.SqlConnection sqlConn;
            Global.gsvServerConn = "Provider=SQLOLEDB;" + "Data Source=" + Global.gsvCenterServerName + ";"
                + "Initial Catalog=" + Global.gsvCenterDataBaseName;
            DBOperation.GlobalDBConnectionStr = "Data Source=" + Global.gsvCenterServerName
                + ";Initial Catalog=" + Global.gsvCenterDataBaseName
                + ";User ID =" + Global.gsvCenterServerUserID
                + ";Password=" + Global.gsvCenterServerPassword;
            DBOperation.GlobalMasterDBConnectionStr = "Data Source=" + Global.gsvCenterServerName
                + ";Initial Catalog=Master"
                + ";User ID =" + Global.gsvCenterServerUserID
                + ";Password=" + Global.gsvCenterServerPassword;
            DBOperation.GlobalMSDBConnectionStr = "Data Source=" + Global.gsvCenterServerName
                + ";Initial Catalog=msdb"
                + ";User ID =" + Global.gsvCenterServerUserID
                + ";Password=" + Global.gsvCenterServerPassword;
            sqlConn = new System.Data.SqlClient.SqlConnection(DBOperation.GlobalDBConnectionStr);
            sqlConn.Open();
            sqlConn.Close();
            return true;
         }
         catch
         {
            return false;
         }

      }
      public static bool LoginToBranchServer()
      {
         return LoginWithSQLSecurity();
      }
      public static bool LoginWithSQLSecurity()
      {
         try
         {
            System.Data.SqlClient.SqlConnection sqlConn;
            Global.gsvServerConn = "Provider=SQLOLEDB;" + "Data Source=" + Global.gsvServerName + ";"
                + "Initial Catalog=" + Global.gsvDataBaseName;
            DBOperation.GlobalDBConnectionStr = "Data Source=" + Global.gsvServerName
                + ";Initial Catalog=" + Global.gsvDataBaseName
                + ";User ID =" + Global.gsvServerUserID
                + ";Password=" + Global.gsvServerPassword;
            DBOperation.GlobalMasterDBConnectionStr = "Data Source=" + Global.gsvServerName
                + ";Initial Catalog=Master"
                + ";User ID =" + Global.gsvServerUserID
                + ";Password=" + Global.gsvServerPassword;
            DBOperation.GlobalMSDBConnectionStr = "Data Source=" + Global.gsvServerName
                + ";Initial Catalog=msdb"
                + ";User ID =" + Global.gsvServerUserID
                + ";Password=" + Global.gsvServerPassword;
            sqlConn = new System.Data.SqlClient.SqlConnection(DBOperation.GlobalDBConnectionStr);
            sqlConn.Open();
            sqlConn.Close();
            return true;
         }
         catch 
         {
            return false;
         }

      }
      public static String QueryStringByteValue(RegistryKey RootKey, string KeyName, string ValueName)
      {
         byte[] bValue;
         try
         {
            String sValue;
            bValue = (byte[])RegEdit.QueryRegValue(RootKey, KeyName, ValueName);
            if (bValue == null)
               return "";
            sValue = Insignia.Global.GetByteString(bValue);
            sValue = sValue.Substring(0, sValue.Length - 1);
            return sValue;
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.ToString());
            return null;
         }
      }
      public static bool GetConnectionStr(string RegFolder, string serverName, string dbName)
      {
         return GetConnectionStr(RegFolder, serverName, dbName, "",false);
      }
      public static bool GetConnectionStr(string RegFolder, string serverName, string dbName, string datFile, bool overrideDatFileInfo)
      {
         String strInfo;

         try
         {
            if (DBOperation.DBType == DatabaseType.Oracle)
            {
               DBOperation.GlobalDBConnectionStr = "Data Source=ILS;User Id=SA;Password=Oracle";
               return true;
            }

            //  Load security information from registry
            if (Global.gsvDataBaseName == "")
            {
               try
               {
                  if (datFile == "")
                  {
                     String DatFileName = "\\" + Global.APPShort + ".dat";
                     String strDBInfoPath = (String)QueryStringByteValue(Registry.LocalMachine, Global.AppRegFolder, "SystemInfoDir");
                     if (strDBInfoPath != "" && strDBInfoPath != null)
                        Global.GetINIInfo(strDBInfoPath + DatFileName);
                     else
                     {
                        FileInfo vFile = new FileInfo(Global.APPShort + ".dat");
                        if (vFile.Exists)
                        {
                           Global.GetINIInfo(Global.APPShort + ".dat");
                           if (overrideDatFileInfo)
                           {
                              if (serverName.Trim() != "")
                                 Global.gsvServerName = serverName;
                              if (dbName.Trim() != "")
                                 Global.gsvDataBaseName = dbName;
                           }
                           if (LoginWithSQLSecurity())
                           {
                              SaveNTSecurity(false);
                              return true;
                           }

                        }
                     }
                  }
                  else
                  {
                     FileInfo vFile = new FileInfo(datFile);
                     if (vFile.Exists)
                     {
                        Global.GetINIInfo(datFile);
                        if (overrideDatFileInfo)
                        {
                           if (serverName.Trim() != "")
                              Global.gsvServerName = serverName;
                           if (dbName.Trim() != "")
                              Global.gsvDataBaseName = dbName;
                        }
                        if (LoginWithSQLSecurity())
                        {
                           SaveNTSecurity(false);
                           return true;
                        }
                     }
                  }

                  strInfo = QueryStringByteValue(Registry.LocalMachine, RegFolder, "DataLink");
                  strInfo = Encryption.EncryptDecrypt(strInfo);
                  if (serverName.Trim() == "")
                     Global.gsvServerName = strInfo.Substring(0, 128).Trim();
                  else
                     Global.gsvServerName = serverName;
                  Global.gsvServerUserID = strInfo.Substring(256, 128).Trim();
                  if (dbName.Trim() == "")
                     Global.gsvDataBaseName = strInfo.Substring(128, 128).Trim();
                  else
                     Global.gsvDataBaseName = dbName;
                  Global.gsvServerPassword = strInfo.Substring(384);
               }
               catch (Exception ex)
               {
                  Debug.WriteLine(ex.ToString());
               }
            }
            if (Global.gsvNTSecurity == "" || Global.gsvNTSecurity == null)
            {
               Global.gsvNTSecurity = QueryStringByteValue(Registry.LocalMachine, RegFolder, "NTSecurity");
            }

            //  Use NT authentication
            if (Global.gsvNTSecurity == "" || Global.gsvNTSecurity == "YES" || Global.gsvServerUserID == "")
            {
               if (LoginWithNTSecurity())
               {
                  if (Global.gsvServerName != "")
                     return true;
               }
            }

            //  Try to use SQL authentication if ( NT login fails
            if (LoginWithSQLSecurity())
            {
               SaveNTSecurity(false);
               return true;
            }

            return false;
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.ToString());
            return false;
         }
      }
		

		public static String GetConnectStr(string server, string database, string LoginID, string pwd)
		{
			if (LoginID == "")
				return "Integrated Security=SSPI;Data Source=" + server.Trim() +  ";Initial Catalog=" + database.Trim();
			else
				return "Data Source=" + server.Trim() + ";Initial Catalog=" + database.Trim() 
					+ ";User ID =" + LoginID + ";Password=" + pwd;
		}


      public DBOperation(string sql, string ConnectionStr)
         : base()
      {
         //SqlConnection conn;
         if (ConnectionStr.ToLower() == "master")
            DBConnection  = new SqlConnection(MasterDBConnectionStr);
         else if (ConnectionStr.ToLower() == "msdb")
            DBConnection  = new SqlConnection(MSDBConnectionStr);
         else
            DBConnection  = new SqlConnection(ConnectionStr );
         this.Sql = sql;
         // UseTempCnnection = true;
         Command = new SqlCommand(Sql, DBConnection);
         Command.CommandType = CommandType.Text  ;
      }

    /// <summary>
    /// Instantiates a new instance of DBOperation calss using the SQL statement
    /// </summary>
    /// <param name="sql"></param>
    public DBOperation(string sql)
      : this()
    {
      
      Sql = sql;
      Command = new SqlCommand(Sql, DBConnection); 
      Command.CommandType = CommandType.Text  ;
    }

		/// <summary>
		/// Instantiates a new instance of DBOperation calss using the SaveCommand
		/// and deleteCommand, this object can be used as DataAdapter to update Datatable changes
		/// into database.
		/// </summary>
		/// <param name="saveCommand"></param>
		/// <param name="deleteCommand"></param>
		public DBOperation (SqlCommand  saveCommand, SqlCommand  deleteCommand) : this()
		{
         if (saveCommand.CommandType == CommandType.StoredProcedure)
         {
            saveCommand.CommandTimeout = STOREDPROCEDURE_TIMEOUT;
         }
         if (deleteCommand.CommandType == CommandType.StoredProcedure)
         {
            deleteCommand.CommandTimeout = STOREDPROCEDURE_TIMEOUT;
         }
			SetAdapterCommand(saveCommand,deleteCommand);
		}


		/// <summary>
		/// Instantiates a new instance of DBOperation calss using the Sqlcommand
		/// </summary>
		/// <param name="command"></param>
		public DBOperation(SqlCommand command)
			: this()
		{
         if (command.CommandType == CommandType.StoredProcedure)
         {
            command.CommandTimeout = STOREDPROCEDURE_TIMEOUT;
         }
			SetCommand(command);
			
		}

		/// <summary>
		/// Instantiates a new instance of DBOperation calss with transaction
		/// </summary>
		/// <param name="isTransaction"></param>
		public DBOperation(bool isTransaction )
			: this()
		{	
					IsTransaction = isTransaction;
			Command = new SqlCommand();
			Command.Connection = DBConnection ;
		}
		
		/// <summary>
		/// Instantiates a new instance of DBOperation calss
		/// </summary>
		
		public DBOperation() 
		{
			//if (DBConnectionStr == "")
			//	return;
			DBConnection = new SqlConnection(DBConnectionStr);
		}

		/// <summary>
		/// Set a SQL statement to be executed
		/// </summary>
		/// <param name="sql"></param>
		public void SetCommand(string sql)
		{	
			Sql = sql;
			if (Command == null) 
			{
				Command = new SqlCommand();
			}
			if (Command.Connection == null)
			{
				Command.Connection = DBConnection;
			}
			Command.CommandText  = Sql;
			Command.CommandType = CommandType.Text  ;
			Command.Parameters.Clear() ;
		}

		/// <summary>
		/// Set SqlCommand to be executed
		/// </summary>
		/// <param name="command"></param>
		public void SetCommand(SqlCommand command)
		{
			Command = command;
			Command.Connection = DBConnection;  
		}

		/// <summary>
		/// Set SqlDataAdapter commands for updating datatable
		/// </summary>
		/// <param name="saveCommand"></param>
		/// <param name="deleteCommand"></param>
		public void SetAdapterCommand(SqlCommand  saveCommand, SqlCommand  deleteCommand)
		{	
			SqlAdapter = new SqlDataAdapter() ;

			SaveCommand = saveCommand ;
			SaveCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;

			DeleteCommand = deleteCommand ;
			SaveCommand.Connection = DBConnection ; 
			DeleteCommand.Connection = DBConnection ;
			SqlAdapter.DeleteCommand = DeleteCommand ;
			SqlAdapter.UpdateCommand = SaveCommand;
			SqlAdapter.InsertCommand = SaveCommand;
			NeedBuildAdapterCommand = false;
		}

		/// <summary>
		/// Using SqlDataAdapter to update Datatable
		/// </summary>
		/// <param name="Table"></param>
		public void Update(DataTable Table)
		{
			if (NeedBuildAdapterCommand)
			{
				SqlCommandBuilder CommandBuilder = new SqlCommandBuilder(SqlAdapter);
				SqlAdapter.Update(Table);
			}
			else
			{
				// First process deletes.
				DataRow[] DataRows;
//				if (SqlAdapter.DeleteCommand != null)
//					SqlAdapter.DeleteCommand.Connection = Command.Connection;
//				if (SqlAdapter.UpdateCommand != null)
//					SqlAdapter.UpdateCommand.Connection = Command.Connection;
//				if (SqlAdapter.InsertCommand != null)
//					SqlAdapter.InsertCommand.Connection = Command.Connection;
	
				SqlAdapter.Update(Table.Select(null, null, DataViewRowState.Deleted));

				// Next process updates.
				SqlAdapter.Update(Table.Select(null, null, DataViewRowState.ModifiedCurrent));

				// Finally, process inserts.
				DataRows = Table.Select(null, null, DataViewRowState.Added);
				SqlAdapter.Update(DataRows);
			}
		}

		/// <summary>
		/// Commit transaction
		/// </summary>
		public void CommitTransaction()
		{
			DBTransaction.Commit();
			DBTransaction = null;
		}

		/// <summary>
		/// Rollback transaction
		/// </summary>
		public  void RollbackTransaction()
		{
			DBTransaction.Rollback();
		}

		/// <summary>
		/// Get datatable using setted command
		/// </summary>
		/// <returns></returns>
		public DataTable GetDataTable() 
		{
			
				SqlDataAdapter da;
				DataTable dt;
				dt = new DataTable() ;
				da = new SqlDataAdapter();
				da.SelectCommand = Command;
				da.Fill(dt);
            if (Command.Connection.State == ConnectionState.Open)
               Command.Connection.Close();

				return dt;			
		}

      /// <summary>
      /// Get dataset using setted command
      /// </summary>
      /// <returns></returns>
      public DataSet GetDataSet()
      {

         DataSet dt = new DataSet();
         SqlDataAdapter da = new SqlDataAdapter();
         da.SelectCommand = Command;
         da.Fill(dt);
         if (Command.Connection.State == ConnectionState.Open)
            Command.Connection.Close();

         return dt;
      }

		public bool Exists() 
		{
			
			DataTable dt = GetDataTable();
			if (dt.Rows.Count > 0 ) 
			{
				return true;
			}
			else
			{
				return false;
			}
	
			
		}

      public DataRow GetOneRow() 
      {
         
            SqlDataAdapter da;
            DataTable dt;
            DataRow dr;
            dt = new DataTable() ;
            da = new SqlDataAdapter();
            da.SelectCommand = Command;
            da.Fill(dt);
            dr = dt.Rows [0];
            if (Command.Connection.State == ConnectionState.Open)
               Command.Connection.Close();

            return dr;
         
      }


		// Why do we need this function? It won't work for web applications. DF 2004-10-06
      //private void CheckNulls()
      //{
      //   string strMessage = "";
      //   if (Command == null)
      //      strMessage = " Command is null /n/r" ;
      //   if (SqlAdapter == null)
      //      strMessage = strMessage + " SqlAdapter is null /n/r" ;
      //   if (strMessage.Length > 2)
      //      System.Windows.Forms.MessageBox.Show(strMessage);
      //}

		/// <summary>
		/// Fill dataset using tableName
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="tableName"></param>
		public void Fill(DataSet dataSet, string tableName ) 
		{
			
			SqlAdapter = new SqlDataAdapter();
			SqlAdapter.SelectCommand = Command;
			SqlAdapter.Fill(dataSet,tableName);
         if (Command.Connection.State == ConnectionState.Open)
            Command.Connection.Close();

		}

		/// <summary>
		/// Fill datatable
		/// </summary>
		/// <param name="table"></param>
      public void Fill(DataTable table)
      {
         SqlAdapter = new SqlDataAdapter();
         Command.CommandTimeout = 3600;
         SqlAdapter.SelectCommand = Command;
         SqlAdapter.Fill(table);
         if (Command.Connection.State == ConnectionState.Open)
            Command.Connection.Close();
      }

		/// <summary>
    /// Add parameter for Command
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="dbType"></param>
		/// <param name="size"></param>
		/// <param name="direction"></param>
		/// <param name="parameterValue"></param>
		/// <param name="isNullable"></param>
		/// <param name="precision"></param>
		/// <param name="scale"></param>
		/// <param name="sourceColumn"></param>
		/// <param name="sourceVersion"></param>
	
		public void AddParameter( string parameterName, SqlDbType dbType  ,   int size,   ParameterDirection direction , 
			object parameterValue,  bool isNullable ,  byte precision,  byte scale,  string sourceColumn ,   DataRowVersion sourceVersion )
		{
			Command.Parameters.Add(new SqlParameter(parameterName, dbType, size, direction, isNullable, precision, scale, "", DataRowVersion.Default, parameterValue));
		}

		/// <summary>
		/// Execute SqlCommand without query data
		/// </summary>
		public void ExecuteNonQuery()
		{
         if (Command.CommandType == CommandType.StoredProcedure)
         {
            ExecuteNonQuery(STOREDPROCEDURE_TIMEOUT);
         }
         else
         {
            ExecuteNonQuery(1200);
         }
			
		}

		public void ExecuteNonQuery(int TimeOut)
		{
         if (Command.CommandType == CommandType.StoredProcedure)
         {
            Command.UpdatedRowSource = UpdateRowSource.OutputParameters;
         }
			try 
			{
				Command.CommandTimeout = TimeOut;
				Command.Connection.Open();  
				Command.ExecuteNonQuery();
				
			}
			catch ( Exception ex )
			{
				Command.CommandTimeout = 1200;
            if (!ex.ToString().ToLower().Contains("connectionstring"))
				   throw ex;
			}
			finally
			{
				if	( !IsTransaction )
				{
               if (Command.Connection.State == ConnectionState.Open)
                  Command.Connection.Close();

				}
			}
		}

		/// <summary>
		/// Execute Command to get one value
		/// </summary>
		/// <returns></returns>
		public object ExecuteScalar() 
		{
			try
			{
				Command.Connection.Open();  
				Object o= Command.ExecuteScalar();
				return o;
			}
			catch( Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				throw;
			}
			finally
			{
            if (Command.Connection.State == ConnectionState.Open)
               Command.Connection.Close();
         }
		}

		/// <summary>
		/// Set command parameter value
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="Value"></param>
 
		public void SetParameter(string parameterName,object Value)
		{
			if (Value != null)
				Command.Parameters [parameterName].Value = Value;
			else
				Command.Parameters [parameterName].Value = DBNull.Value;
		}

		/// <summary>
		/// Get command parameter value
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public object GetParameter(string parameterName)
		{
			return Command.Parameters[parameterName].Value;
		}
	
		/// <summary>
		/// Generate unique key instead of using Identity
		/// </summary>
		/// <param name="KeyName"></param>
		/// <returns></returns>
		public static int GetKey(string KeyName) 
		{
			SqlCommand  Command;
			SqlConnection conn = new SqlConnection (DBConnectionStr );
			Command = new SqlCommand ("GetKey",conn);
			 
			Command.CommandType = CommandType.StoredProcedure;
			conn.Open ();
			Command.Parameters.Add(new SqlParameter( "@KeyName", SqlDbType.NVarChar, 20, ParameterDirection.Input, true, 0, 0, "",  DataRowVersion.Default , KeyName));
			Command.Parameters.Add(new SqlParameter( "@KeyValue", SqlDbType.Int , 4, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Default, 0));
			Command.ExecuteNonQuery (); 
			conn.Close ();
			int Value ;
			Value = (int) Command.Parameters["@KeyValue"].Value ;
         if (Command.Connection.State == ConnectionState.Open)
            Command.Connection.Close();

			return Value ;
		}

		//  Execute Sql Directly
		public static void ExecuteSql(string sqlStatement, params object[] values) 
		{
			string SqlValue;
			for (int i=0;i<values.Length;i++)
			{
				if ( values[i] is System.DateTime )
					SqlValue = SqlComp.ToDate ( (DateTime) values[i]);
				else if (values[i] is String)
					SqlValue = "'" +  (string) values[i] + "'";
				else 
					SqlValue =   values[i].ToString() ;
				
				sqlStatement = sqlStatement.Replace(":" + (i+1),SqlValue); 
			}

      DBOperation dbop = new DBOperation(sqlStatement);
				dbop.ExecuteNonQuery(3600);
			dbop.Close ();
			return ;
		}
		
    /// <summary>
		/// Close connection
		/// </summary>
		public void Close()
		{
			Dispose(true);
		}
	
		/// <summary>
		/// Finalize
		/// </summary>
		~DBOperation()
		{
			Dispose(false);
		}
		
		/// <summary>
		/// Dispose of this object's resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			
		}

		/// <summary>
		///		Free the instance variables of this object.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (DBConnection != null)
					DBConnection.Close ();
			
			if (! disposing)
				return; // we're being collected, so let the GC take care of this object

			if (SaveCommand != null)
			{
				SaveCommand.Dispose();
				SaveCommand = null;
			}
			if (DeleteCommand != null)
			{
				DeleteCommand.Dispose();
				DeleteCommand = null;
			}

			if (Command != null)
			{
				Command.Dispose();
				Command =null;  
			}

			if (SqlAdapter != null)
			{
				SqlAdapter.Dispose();
				SqlAdapter = null;
			}

			GC.SuppressFinalize(this);
		}

      public static string  GlobalDBConnectionStr
      {
         get
         {
            return DBConnectionStr;
         }
         set
         {
            DBConnectionStr = value;
        
         }
      }
      public static string  GlobalMasterDBConnectionStr
      {
         get
         {
            return MasterDBConnectionStr;
         }
         set
         {
            MasterDBConnectionStr = value;
        
         }
      }
      public static string  GlobalMSDBConnectionStr
      {
         get
         {
            return MSDBConnectionStr;
         }
         set
         {
            MSDBConnectionStr = value;
        
         }
      }
      
    #region  Generate parameters for stored procedure
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Command"></param>
    /// <param name="SPName"></param>
    private void CreateParameters(SqlCommand Command, string SPName)
    {
			
    }
    #endregion
	}
}
#endif
