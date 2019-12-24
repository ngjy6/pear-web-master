using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace NTU_FYP_REBUILD_17.App_Code
{
	public class dataclass
	{

		//OleDbDataAdapter  adapt;
		SqlDataAdapter adapt;
		DataSet ds;
		public dataclass()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public static string RequesttStr(string inputStr)
		{
			inputStr = inputStr.ToLower().Replace(",", "");
			inputStr = inputStr.ToLower().Replace("<", "");
			inputStr = inputStr.ToLower().Replace(">", "");
			inputStr = inputStr.ToLower().Replace("%", "");
			inputStr = inputStr.ToLower().Replace(".", "");
			inputStr = inputStr.ToLower().Replace(":", "");
			inputStr = inputStr.ToLower().Replace("#", "");
			inputStr = inputStr.ToLower().Replace("&", "");
			inputStr = inputStr.ToLower().Replace("$", "");
			inputStr = inputStr.ToLower().Replace("^", "");
			inputStr = inputStr.ToLower().Replace("*", "");
			inputStr = inputStr.ToLower().Replace("`", "");
			inputStr = inputStr.ToLower().Replace(" ", "");
			inputStr = inputStr.ToLower().Replace("~", "");
			inputStr = inputStr.ToLower().Replace("or", "");
			inputStr = inputStr.ToLower().Replace("and", "");
			inputStr = inputStr.ToLower().Replace("'", "");

			return inputStr;

		}

		public void ExecuteSQL(string strSQL)
		{
			string connection = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString.ToString();

			using (SqlConnection conn = new SqlConnection(connection))
			{
				try
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand(strSQL, conn);
					System.Diagnostics.Debug.WriteLine(strSQL);
					cmd.ExecuteNonQuery();
					conn.Close();
				}
				catch (Exception err)
				{
					throw new Exception(err.Message, err);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
		}


		public String CreateRandomPassword(int passwordLength)
		{
			string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
			char[] chars = new char[passwordLength];
			Random rd = new Random();

			for (int i = 0; i < passwordLength; i++)
			{
				chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
			}

			return new string(chars);
		}

		public bool IsValidEmail(string email)
		{
			//regular expression pattern for valid email
			string pattern = @".*@.*\..*";
			//Regular expression object
			System.Text.RegularExpressions.Regex check = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);
			bool valid = false;

			if (string.IsNullOrEmpty(email))
			{
				valid = true;
			}
			else
			{
				valid = check.IsMatch(email);
			}
			return valid;
		}

		public DataTable GetDataSet(string strSql, string TableName)
		{
			ds = new DataSet();
			string connection = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString.ToString();

			using (SqlConnection conn = new SqlConnection(connection))
			{
				try
				{
					conn.Open();
					adapt = new SqlDataAdapter(strSql, conn);
					adapt.Fill(ds, TableName);
					return ds.Tables[TableName];

				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message, ex);

				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
		}

		public string GetSingleValue(String strSql)
		{
			string connection = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString.ToString();

			using (SqlConnection conn = new SqlConnection(connection))
			{
				string result;
				try
				{
					conn.Open();
					SqlCommand cmdstr = new SqlCommand(strSql, conn);

					result = cmdstr.ExecuteScalar().ToString();

					return result;
				}
				catch (Exception err)
				{
					throw new Exception(err.Message, err);
				}
				finally
				{
					conn.Dispose();
					conn.Close();
				}
			}
		}
		public static string FormatString(string str)
		{
			str = str.Replace(" ", "&nbsp;&nbsp");
			str = str.Replace("<", "&lt;");
			str = str.Replace(">", "&glt;");
			str = str.Replace('\n'.ToString(), "<br>");
			return str;
		}
		public static string ReplaceStr(string str)
		{
			if (str == null || str == "")
				return null;
			str = str.Replace(":", "");
			str = str.Replace("'", "");
			str = str.Replace("&", "");
			str = str.Replace("%20", "");
			str = str.Replace("-", "");
			str = str.Replace("==", "");
			str = str.Replace("<", "");
			str = str.Replace(">", "");
			str = str.Replace("%", " ");
			return str;
		}

		public static string DelSQLStr(string str)
		{
			if (str == null || str == "")

				return "";
			str = str.Replace(";", "");
			str = str.Replace("'", "");
			str = str.Replace("&", "");
			str = str.Replace("%20", "");
			str = str.Replace("--", "");
			str = str.Replace("==", "");
			str = str.Replace("<", "");
			str = str.Replace(">", "");
			str = str.Replace("%", "");
			str = str.Replace("+", "");
			str = str.Replace("-", "");
			str = str.Replace("=", "");
			str = str.Replace(",", " ");
			return str;
		}
	}
}