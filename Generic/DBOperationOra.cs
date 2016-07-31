
using System;
using System.Data; 
using System.Diagnostics;

#if Oracle
using Oracle.DataAccess.Client ; 
using Oracle.DataAccess.Types ;
#endif

#if Oracle
namespace Insignia
{


	/// <summary>
	/// Generic Class for process Oracle database operation	Dominic Fang 2001-12-18
	/// </summary>
	

	public class DBOperation : IDisposable  
	{
		public Oracle.DataAccess.Client.OracleConnection DBConnection; 
		public static string DBConnectionStr;
		public static string OleDbConnectionStr;
		public static DatabaseType DBType = DatabaseType.Oracle  ;

		private Oracle.DataAccess.Client.OracleDataAdapter   SqlAdapter ;
		private Oracle.DataAccess.Client.OracleTransaction DBTransaction; 
		private Oracle.DataAccess.Client.OracleCommand Command;
		private string Sql;
		private  bool IsTransaction = false;
		private OracleDataReader dataReader ;
		private bool NeedBuildAdapterCommand = true;

		private OracleCommand  SaveCommand;
		private OracleCommand  DeleteCommand;

		
		public DBOperation(string sql, CommandType commandType)
			: base()
		{
			DBConnection = new OracleConnection(DBConnectionStr);	
			Sql = sql;
			Command = new OracleCommand(Sql, DBConnection);
			Command.CommandType = commandType;
			if (commandType == CommandType.StoredProcedure )
			{
				CreateParameters(Command, Sql);
			}
		}

		public DBOperation(string sql)
			: base()
		{
			DBConnection = new OracleConnection(DBConnectionStr);	
			Sql = sql;
			Command = new OracleCommand(Sql, DBConnection);
			Command.CommandType = CommandType.Text  ;
			
		}

		public DBOperation (OracleCommand  saveCommand, OracleCommand  deleteCommand) : this()
		{
			//DBConnection = new OracleConnection(DBConnectionStr);
			SetAdapterCommand(saveCommand,deleteCommand);
		}

		public DBOperation(OracleCommand command)
			: this()
		{
			
			SetCommand(command);
		}

		public DBOperation(bool isTransaction )
			: this()
		{	

			IsTransaction = isTransaction;
			Command = new OracleCommand();
			Command.Connection = DBConnection ;

			if (IsTransaction ) 
			{
				DBConnection.Open() ;
				DBTransaction = DBConnection.BeginTransaction();
//				 Command.Transaction = DBTransaction;
			}
		}
		
		public DBOperation()
		{	
			DBConnection = new OracleConnection(DBConnectionStr);
		}

		// Generate a new command
		public void SetCommand(string sql)
		{	
			Sql = sql;
			if (Command == null)
			{
				Command = new OracleCommand(Sql);
				Command.Connection = DBConnection ; 
				Command.CommandType = CommandType.Text  ;
			}
			else
			{
				Command.CommandText  = Sql;
				Command.CommandType = CommandType.Text  ;
				Command.Parameters.Clear() ;
			}
		
		}

		public void SetCommand(OracleCommand command)
		{
			Command = command;
			Command.Connection = DBConnection;  
		}

		// Generate a new command for Sql adapter
		public void SetAdapterCommand(OracleCommand  saveCommand, OracleCommand  deleteCommand)
		{	
			SqlAdapter = new OracleDataAdapter() ;

			SaveCommand = saveCommand ;
			SaveCommand.Connection = DBConnection ; 
			SaveCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;

			DeleteCommand = deleteCommand ;
			DeleteCommand.Connection = DBConnection ;
			SqlAdapter.DeleteCommand = DeleteCommand ;
			SqlAdapter.UpdateCommand = SaveCommand;
			SqlAdapter.InsertCommand = SaveCommand;
			NeedBuildAdapterCommand = false;

		}


		
		public void Update(DataTable Table)
		{
			if (NeedBuildAdapterCommand)
			{
				OracleCommandBuilder CommandBuilder = new OracleCommandBuilder(SqlAdapter);
				SqlAdapter.Update(Table);
 			}
			else
			{
				// First process deletes.
				DataRow[] DataRows;
				SqlAdapter.Update(Table.Select(null, null, DataViewRowState.Deleted));

				// Next process updates.
				SqlAdapter.Update(Table.Select(null, null, DataViewRowState.ModifiedCurrent));

				// Finally, process inserts.
				DataRows = Table.Select(null, null, DataViewRowState.Added);
				SqlAdapter.Update(DataRows);
			}
		}


		//	Commit transaction
		public  void CommitTransaction()
		{
			DBTransaction.Commit();
			DBConnection.Close();
			DBTransaction = null;
		}


		public  void RollbackTransaction()
		{
			DBTransaction.Rollback();
			DBConnection.Close();
		}

		//	Get datatable using OracleDataAdapter
		public DataTable GetDataTable() 
		{
			OracleDataAdapter da;
			DataTable dt;
			dt = new DataTable() ;
			da = new OracleDataAdapter();
			da.SelectCommand = Command;
			da.Fill(dt);
			return dt;

		}

		public void Fill(DataSet dataSet, string tableName ) 
		{
			
			SqlAdapter = new OracleDataAdapter();
			SqlAdapter.SelectCommand = Command;
			SqlAdapter.Fill(dataSet,tableName);

		}

		public void Fill(DataTable  table ) 
		{
			
			SqlAdapter = new OracleDataAdapter();
			SqlAdapter.SelectCommand = Command;
			SqlAdapter.Fill(table);

		}

		//	Add parameter for Command
		public void AddParameter( string parameterName, OracleDbType dbType  ,   int size,   ParameterDirection direction , 
			object parameterValue,  bool isNullable ,  byte precision,  byte scale,  string sourceColumn ,   DataRowVersion sourceVersion )
		{
			Command.Parameters.Add(new OracleParameter(parameterName, dbType, size, direction, isNullable, precision, scale, "", DataRowVersion.Default, parameterValue));
		}

		public void CloseReader()
		{
			if (dataReader != null)
				dataReader.Close();

		}

		public void ExecuteNonQuery()
		{
			if (!IsTransaction )
			{
				Command.Connection.Open();
			}
			if (Command.CommandType == CommandType.StoredProcedure)
				Command.UpdatedRowSource = UpdateRowSource.OutputParameters;
			try 
			{
				Command.ExecuteNonQuery();
			}
			catch(Exception ex)
			{
				throw ex;
			}
			finally
			{
				if	( !IsTransaction )
				{
					Command.Connection.Close();
				}

			}


		}


		public object ExecuteScalar() 
		{	
			Command.Connection.Open();
			Object o= Command.ExecuteScalar();
			Command.Connection.Close();
			return o;
		}

		/// <summary>
		/// Get Sql Data Reader
		/// </summary>
		/// <returns>OracleDataReader</returns>
		public void ExecuteReader() 
		{
			if (Command.Connection.State == ConnectionState.Closed )
			Command.Connection.Open();
			dataReader = Command.ExecuteReader();
			
		}
		public bool Read()
		{
			return dataReader.Read ();
		}


	public string GetString(int  ordinal )
	{
		if (dataReader.IsDBNull(ordinal))
		{
			return "";
		}
		else
		{
			return dataReader.GetString(ordinal ); 
		}
	}
		public string GetString(string  columnName )
		{
			if (dataReader[columnName] is DBNull )
			{
				return "";
			}
			else
			{
				return (string) dataReader[columnName]; 
			}
		}
		/// <summary>
		/// Get boolean value from SqlDataReader using column index
		/// </summary>
		/// <param name="ordinal"></param>
		/// <returns></returns>
		public object GetBoolean(int ordinal)
		{
			return (bool) dataReader[ordinal]; 
		}

		

		public int GetInt32(int  ordinal )
		{
			if (dataReader.IsDBNull(ordinal))
			{
				return 0;
			}
			else
			{
				return dataReader.GetInt32 (ordinal ); 
				}
		}

		public int GetInt32(string  columnName )
		{
			if (dataReader[columnName] is DBNull )
			{
				return 0;
			}
			else
			{
				object val;
				val = dataReader[columnName];
				if (val is System.Int32  )
					return (int) val;
				else
					return (int) (long) val; 
			}
		}


		public object GetValue(int  ordinal )
		{
				return dataReader.GetValue (ordinal ); 
		}

		public object GetValue(string  columnName )
		{
			return dataReader[columnName]; 
		}

		public object GetBoolean(string  columnName )
		{
			object x;
			x = dataReader[columnName]; 
			//Debug.WriteLine(x.GetType().ToString());     
			if ( ((short) dataReader[columnName]) >= 1)
				return true;
			else
				return false;
		}

		// Set command parameter value
		public void SetParameter(string parameterName,object Value)
		{
			parameterName = parameterName.Replace ("@","").ToUpper ();
			if ( Value is bool)
				Command.Parameters [parameterName].Value = ((bool)Value)?1:0;
			else
				Command.Parameters [parameterName].Value = Value;
			/*
			switch (Command.Parameters [parameterName].OracleDbType)
			{
				case OracleDbType.Int32:
					switch (Value.GetType().Name)
					{
						case "Int32":
						case "Int16":
							break;
						default:
							throw new Exception ("Wrong Data Type");	
					}
					break;

				default:
					break;
			}
		*/

		}

		// Get command parameter value
		public object GetParameter(string parameterName)
		{
			parameterName = parameterName.Replace("@","").ToUpper();
			Object   paramValue = Command.Parameters[parameterName].Value;
			if ( paramValue is Oracle.DataAccess.Types.OracleString )
				return ((OracleString)paramValue).ToString ();
			else if ( paramValue is Oracle.DataAccess.Types.OracleDecimal )
				return ((OracleDecimal)paramValue).ToSingle() ;
			else if ( paramValue is Oracle.DataAccess.Types.OracleDate )
				return (DateTime ) (OracleDate ) paramValue;
			else if (paramValue is DBNull && 
					(Command.Parameters[parameterName].OracleDbType == OracleDbType.Varchar2 || 
						Command.Parameters[parameterName].OracleDbType == OracleDbType.Char ))
				return "";
			else
				return paramValue;
		}

	
		public static String GetConnectStr(string server, string database, string LoginID, string pwd)
		{
			return "Data Source=" + database.Trim() + ";User Id=" + LoginID + ";Password=" + pwd;
 		}

		/*
		public static void InitializeConnection()
		{
			DBConnection = new OracleConnection(DBConnectionStr);
		}
		*/

//		//  Generate unique key instead of using Identity
		public static int GetKey(string KeyName) 
		{
			OracleCommand  Command;
			OracleConnection conn = new OracleConnection(DBConnectionStr) ;
			int Value ;

			Command = new OracleCommand ("GetKey",conn);
			 
			Command.CommandType = CommandType.StoredProcedure;
			Command.Parameters.Add(new OracleParameter( "KeyName", OracleDbType.Varchar2 , 20, ParameterDirection.Input, true, 0, 0, "",  DataRowVersion.Default , KeyName));
			Command.Parameters.Add(new OracleParameter( "KeyValue", OracleDbType.Int32  , 4, ParameterDirection.InputOutput, true, 0, 0, "", DataRowVersion.Default, 0));
			conn.Open ();
			Command.ExecuteNonQuery (); 
			Value = (int) Command.Parameters["KeyValue"].Value ; 
			conn.Close();  
			return Value ;
		}

		

		//  Execute Sql Directly
		public static void ExecuteSql(string sqlStatement, params object[] values) 
		{
			string SqlValue;
			for (int i=0;i<values.Length;i++)
			{
				if ( values[i] is System.DateTime )
				{
					SqlValue = SqlComp.ToDate ( (DateTime) values[i]);
				}
				else if (values[i] is String)
				{
					SqlValue = "'" +  (string) values[i] + "'";
				}
				else 
				{
					SqlValue =  values[i].ToString() ;
				}
				
				sqlStatement = sqlStatement.Replace(":" + (i+1),SqlValue); 

			}
			DBOperation dbop = new DBOperation(sqlStatement);
			dbop.ExecuteNonQuery();
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
	
		~DBOperation()
		{
			Dispose(false);
		}

		public static bool TestConnect(String strConn, bool ShowMessage)
		{
			try
			{
				OracleConnection conn = new OracleConnection (strConn);
				conn.Open();
				conn.Close();
				return true;
			}
			catch(Exception ex)
			{
				if (ShowMessage)
					System.Windows.Forms.MessageBox.Show (ex.Message, 
						"Test Connection", System.Windows.Forms.MessageBoxButtons.OK,
						System.Windows.Forms.MessageBoxIcon.Exclamation );
				return false;
			}
		}

		
		/// <summary>
		///     Dispose of this object's resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(true); // as a service to those who might inherit from us
		}

		/// <summary>
		///		Free the instance variables of this object.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{

			if (dataReader !=null)
			{
				dataReader.Close ();
			}
			if (DBConnection != null)
			{
				if (DBConnection.State == ConnectionState.Open )
					DBConnection.Close();
				DBConnection = null;
			}

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
#region  Generate parameters for stored procedure

		private void CreateParameters(OracleCommand Command, string SPName)
		{
	
		}
#endregion
		 
	}

}

#endif
