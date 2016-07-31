using System;

namespace Insignia
{
	/// <summary>
	/// Summary description for SqlComp.
	/// </summary>
	public class SqlComp
	{
		public SqlComp()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		
		public static string Add()
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return "+";
			else
				return "||";
		}

		public static string Char()
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return "char";
			else
				return "chr";
		}

		public static string IsNull()
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return "isnull";
			else
				return "NVL";
		}

		public static string SqlValue(string OriginalValue)
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return OriginalValue.Replace("'","''") ;
			else
				return OriginalValue.Replace("'","''");
		}

		public static string ToChar(string strFormat, string strVal, string strType, string strStyle )
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				if (strFormat == "" )
					return "ltrim(rtrim(str(" + strVal + ")))";
				else
					return "convert(" + strType + ", " + strVal + ", " + strStyle + ")";
			else
				if (strFormat =="")
					return "To_Char(" + strVal  + ")";
				else
					return "To_Char(" + strVal + ", '" + strFormat + "')";

		}

		public static string ToTimeChar(string strVal )
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return "convert(char(8), " + strVal + ", 108)";
			else
				return "To_Char(" + strVal + ", 'hh:mm:ss')";

		}

		public static string ToChar(string strVal)
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
					return "ltrim(rtrim(convert(char(20), " + strVal + ")))";
			else
				return "To_Char(" + strVal  + ")";

		}

		public static string Decode(string columnName,params string[] values)
		{

			//Sql
			string Sql;

			if (DBOperation.DBType == DatabaseType.SQLServer )
			{
				Sql = "Case " + columnName  ;
				for (int i=0;i<values.Length ;i++)
				{
					if (i == values.Length -1)
						Sql = Sql + " else  " + values[i];  
					else
					{
						Sql = Sql + " when " + values [i];
						i++;
						Sql = Sql + " then " + values [i];
					};
				}
				Sql = Sql+" end ";
				return Sql;
			}

			else

				//Oracle

			{
					Sql = "Decode(" + columnName  ;
				for (int i=0;i<values.Length ;i++)
				{
					Sql = Sql + "," + values [i];
				}
				Sql = Sql+")";
				return Sql;
			}
		}



		public static string OuterJoin(string LeftColumn, string RightColumn, bool IsLeftJoin )
		{
			string sqlString ;
			if (DBOperation.DBType == DatabaseType.SQLServer )
			{
				if (IsLeftJoin)
					sqlString = LeftColumn + " *= " + RightColumn;
				else
					sqlString = LeftColumn + " =* " + RightColumn;
			}
			else
			{
				if (IsLeftJoin)
					sqlString = LeftColumn + " = " + RightColumn + "(+)";
				else
					sqlString = LeftColumn + "(+) = " + RightColumn;
			}
			return sqlString;

		}

		public static string OracleTop(int Rows)
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return "";
			else
				return " and RowNum <= " + Rows;
		}

		public static string SQLServerTop(int Rows)
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return " top " + Rows;
			else
				return "";
		}
		public static string MSSQLDBOwner()
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return "dbo.";
			else
				return "";
		}
		public static string CurrentDate()
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return "getDate() " ;
			else
				return "SYSDATE";
		}
		public static string Substring()
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return "Substring" ;
			else
				return "Substr";
		}

		public static void dbtypeforDebug(DatabaseType strDB)

		{
			DBOperation.DBType = strDB ;
		}


		public static string ToDate(DateTime Value)
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return  "'" + Value.ToString("yyyy-MM-dd") + "'" ;
			else
				return "TO_Date('" + Value.ToString("yyyy-MM-dd") + "','YYYY-MM-DD')";
		}

		public static string ToDateTime(DateTime Value)
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return  "'" + Value.ToString("yyyy-MM-dd HH:mm") + "'" ;
			else
				return "TO_Date('" + Value.ToString("yyyy-MM-dd HH:mm") + "','YYYY-MM-DD HH24:MI')";
		}

		public static string ToDateEnd(DateTime Value)
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return  "'" + Value.ToString("yyyy-MM-dd") + " 23:59:00'" ;
			else
				return "TO_Date('" + Value.ToString("yyyy-MM-dd") + " 23:59:59'" + ",'YYYY-MM-DD HH24:MI:SS')";
		}

		public static string Contains(string ColumnName, string SearchValue)
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return  "Contains(" + ColumnName + ",' " + SearchValue + " ')";
			/*
				if (SearchValue.IndexOf(" ") > -1)
					if (HasBooleanWord(SearchValue) )
						return  "Contains(" + ColumnName + ",' " + SearchValue + " ')";
					else
					{
						SearchValue = CreateNewSearchValue( SearchValue);
						return  "Contains(" + ColumnName + ",' " + SearchValue + " ')";
					}

				else
				   return  "Contains(" + ColumnName + ",' FORMSOF(INFLECTIONAL," + SearchValue + ") ')";
					*/
			else
				return  "Contains(" + ColumnName + ",'" + SearchValue + "',1) > 0";
		}

		private static string CreateNewSearchValue(string SearchValue)
		{
			while (SearchValue.IndexOf("  ") > -1  )
			{
				SearchValue = SearchValue.Replace ("  "," ");
			}
			SearchValue = SearchValue.Replace(" "," AND ");
			return SearchValue;
		}
		private static bool HasBooleanWord(string SearchValue)
		{
			SearchValue = SearchValue.ToUpper ();
			if ((SearchValue.IndexOf(" OR ") > -1) || (SearchValue.IndexOf(" NEAR ") > -1))
				return  true;
			else if (SearchValue.IndexOf(" AND ") > -1)
				return true;
			else if ((SearchValue.IndexOf("(") > -1) || (SearchValue.IndexOf(")") > -1))
				return true;
			else if (SearchValue.IndexOf("\"") > -1) 
				return true;

			return false;
		}

		public static string TextTruncate()
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return  "*";
			else
				return  "%";
		}

		public static string DaysBetween(string FromDate , string ToDate )
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return  "DateDiff(dd," +  FromDate + "," + ToDate + ")" ;
			else
				return  "TO_CHAR(" + ToDate + ",'J') - TO_CHAR(" + FromDate + ",'J')";
		}

		public static string AddMonths(int noOfMonths , string baseDate )
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return  "DateAdd(m," +  noOfMonths + "," + baseDate + ")" ;
			else
				return  "ADD_MONTHS(" + baseDate + "," + noOfMonths + ")";
		}

		public static string AddDays(int noOfDays , string baseDate )
		{
			if (DBOperation.DBType == DatabaseType.SQLServer )
				return  "DateAdd(d," +  noOfDays + "," + baseDate + ")" ;
			else
				return   baseDate + "+" + noOfDays ;
		}
	}
}
