using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace NTU_FYP_REBUILD_17.App_Code
{
	public class account
	{
		public int userID;
		public string email;
		public string password;
		public string fName;
		public string lName;
		public string address;
		public string officeNo;
		public string handphoneNo;
		public char gender; // 1 for male, 0 for female
		public string dob;
		public string userType;
		public DateTime createDateTime;
		public int patientID;

		SqlDataAdapter adapt;
		DataSet ds;

		public bool Authenticate()
		{
			bool success = false;
			string connection = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString.ToString();
			using (SqlConnection conn = new SqlConnection(connection))
			{
				try
				{
					conn.Open();
					ds = new DataSet();
					SqlCommand cmd = new SqlCommand("Select dbo.[user].userID, dbo.[user].email, dbo.[user].password, dbo.[user].firstname, dbo.[user].lastname, dbo.[user].address, dbo.[user].officeno, dbo.[user].handphoneNo, dbo.[user].gender, dbo.[user].DOB, dbo.[user].createDateTime, dbo.[userType].userTypeName FROM dbo.[user] inner join userType on dbo.[user].userTypeID = dbo.[usertype].userTypeID WHERE dbo.[user].isDeleted = 0 AND dbo.[user].isApproved = 1 AND email = @email and password = @password", conn);
					cmd.Parameters.Add(new SqlParameter("@email", email));
					cmd.Parameters.Add(new SqlParameter("@password", password));
					adapt = new SqlDataAdapter(cmd);
					adapt.Fill(ds, "login");
					if (ds.Tables["login"].Rows.Count != 0)
					{
						userID = Convert.ToInt32(ds.Tables["login"].Rows[0]["userID"].ToString());
						fName = ds.Tables["login"].Rows[0]["firstName"].ToString();
						lName = ds.Tables["login"].Rows[0]["lastName"].ToString();
						dob = ds.Tables["login"].Rows[0]["dob"].ToString();
						address = ds.Tables["login"].Rows[0]["address"].ToString();
						officeNo = ds.Tables["login"].Rows[0]["officeNo"].ToString();
						handphoneNo = ds.Tables["login"].Rows[0]["handphoneNo"].ToString();
						gender = Convert.ToChar(ds.Tables["login"].Rows[0]["gender"].ToString());
						userType = ds.Tables["login"].Rows[0]["userTypeName"].ToString();
						createDateTime = Convert.ToDateTime(ds.Tables["login"].Rows[0]["createDateTime"].ToString());

						success = true;
					}
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
			return success;
		}

		public bool forgetPwd()
		{
			bool success = false;
			string connection = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString.ToString();
			using (SqlConnection conn = new SqlConnection(connection))
			{
				try
				{
					conn.Open();

					ds = new DataSet();

					SqlCommand cmd = new SqlCommand("Select * FROM dbo.[user] WHERE dbo.[user].isDeleted = 0 AND dbo.[user].isApproved = 1 AND email = @email", conn);
					cmd.Parameters.Add(new SqlParameter("email", email));

					adapt = new SqlDataAdapter(cmd);
					adapt.Fill(ds, "forgetPwd");
					if (ds.Tables["forgetPwd"].Rows.Count != 0)
					{
						userID = Convert.ToInt32(ds.Tables["forgetPwd"].Rows[0]["userID"].ToString());
						fName = ds.Tables["forgetPwd"].Rows[0]["firstName"].ToString();
						lName = ds.Tables["forgetPwd"].Rows[0]["lastName"].ToString();
						password = ds.Tables["forgetPwd"].Rows[0]["password"].ToString();

						success = true;
					}
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
			return success;
		}

		public bool CreateAccount(int adminID)
		{
			bool success = true;
			string connection = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString.ToString();
			using (SqlConnection conn = new SqlConnection(connection))
			{
				try
				{
					conn.Open();

					ds = new DataSet();

					//Check if email is used before
					SqlCommand cmdEmail = new SqlCommand("SELECT Email from dbo.[user] WHERE email = @email", conn);
					cmdEmail.Parameters.Add(new SqlParameter("@email", email));
					adapt = new SqlDataAdapter(cmdEmail);
					adapt.Fill(ds, "dsEmail");

					if (ds.Tables["dsEmail"].Rows.Count != 0)
					{
						success = false;
					}
					else
					{
						//get userTypeID
						DataSet dsUserTypeID = new DataSet();
						SqlCommand cmdUserTypeID = new SqlCommand("SELECT userTypeID FROM usertype WHERE isDeleted = 0 AND userTypeName = @usertype", conn);
						cmdUserTypeID.Parameters.Add(new SqlParameter("@usertype", userType));

						adapt = new SqlDataAdapter(cmdUserTypeID);
						adapt.Fill(dsUserTypeID, "dsUserTypeID");

						String userTypeID = "";

						if (dsUserTypeID.Tables["dsUserTypeID"].Rows.Count == 0)
						{
							success = false;

						}
						else
						{
							userTypeID = dsUserTypeID.Tables["dsUserTypeID"].Rows[0]["userTypeID"].ToString();
						}
						if (success)
						{
							//Add new record if email is unique
							SqlCommand cmdInsert = new SqlCommand("INSERT INTO dbo.[user](email, password, firstName, lastName, address, officeNo, handphoneNo, gender, DOB, userTypeID, isApproved) OUTPUT INSERTED.UserID VALUES(@email, @password, @fName, @lName, @address, @officeNo, @hpNo, @gender, @DOB, @userTypeID, 1)", conn);
							cmdInsert.Parameters.Add(new SqlParameter("@email", email));
							cmdInsert.Parameters.Add(new SqlParameter("@password", password));
							cmdInsert.Parameters.Add(new SqlParameter("@fName", fName));
							cmdInsert.Parameters.Add(new SqlParameter("@lName", lName));
							cmdInsert.Parameters.Add(new SqlParameter("@address", address));
							cmdInsert.Parameters.Add(new SqlParameter("@officeNo", officeNo));
							cmdInsert.Parameters.Add(new SqlParameter("@hpNo", handphoneNo));
							cmdInsert.Parameters.Add(new SqlParameter("@gender", gender));
							cmdInsert.Parameters.Add(new SqlParameter("@DOB", Convert.ToDateTime(dob)));
							cmdInsert.Parameters.Add(new SqlParameter("@userTypeID", userTypeID));

							int newId = Convert.ToInt32(cmdInsert.ExecuteScalar());


							if (patientID != 0)
							{
								SqlCommand cmdInsertGuardian = new SqlCommand("INSERT INTO dbo.[guardianAllocation](patientID, guardianID, isDeleted, isApproved) OUTPUT INSERTED.guardianAllocationID VALUES(@patientID, @guardianID, 0, 1)", conn);
								cmdInsertGuardian.Parameters.Add(new SqlParameter("@patientID", patientID));
								cmdInsertGuardian.Parameters.Add(new SqlParameter("@guardianID", newId));

								int newGuardianId = Convert.ToInt32(cmdInsertGuardian.ExecuteScalar());

								string logDataGuardian = "patientID:" + patientID + ";guardianID:" + newId;

								//Insert into log
								SqlCommand cmdInsertLogGuardian = new SqlCommand("INSERT INTO Log (logData, logDesc, logCategoryID, userIDInit, userIDApproved,tableAffected, columnAffected, rowAffected, approved) Values (@logData, 'Create new Guardian Allocation" + " - " + fName + " " + lName + "', 2, @adminID, @adminID, 'guardianAllocation', 'all', '" + newGuardianId + "', 1)", conn);
								cmdInsertLogGuardian.Parameters.Add(new SqlParameter("@adminID", adminID));
								cmdInsertLogGuardian.Parameters.Add(new SqlParameter("@logData", logDataGuardian));
								cmdInsertLogGuardian.ExecuteNonQuery();
							}

							if (dsUserTypeID.Tables["dsUserTypeID"].Rows.Count == 0)
							{
								success = false;

							}

							
								//Changes: 
								//date time format change from 5/9/2010 12:00:00 AM  YYYY-mm-dd  
								//add in new column info =  rowaffected(userID), 
							

							string convertedDT = Convert.ToDateTime(dob).ToString("yyyy-MM-dd");

							string logData = "firstName:" + fName + ";lastName:" + lName + ";address:" + address + ";email:" + email + ";handphone:" + handphoneNo + ";officeNo:" + officeNo + ";gender:" + gender + ";dob:" + convertedDT + ";userType:" + userType;

							//Insert into log
							SqlCommand cmdInsertLog = new SqlCommand("INSERT INTO Log (logData, logDesc, logCategoryID, userIDInit, userIDApproved,tableAffected, columnAffected, rowAffected, approved) Values (@logData, 'Create new " + userType + " - " + fName + " " + lName + "', 6, @adminID, @adminID, 'user', 'all', '" + newId + "', 1)", conn);
							cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
							cmdInsertLog.Parameters.Add(new SqlParameter("@logData", logData));
							cmdInsertLog.ExecuteNonQuery();
						}



					}
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
			return success;
		}

		public bool UpdateAccount(int adminID, string previousDetails)
		{
			bool success = true;
			dataclass dl = new dataclass();
			string connection = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString.ToString();
			using (SqlConnection conn = new SqlConnection(connection))
			{
				try
				{
					conn.Open();

					ds = new DataSet();

					//get userTypeID
					DataSet dsUserTypeID = new DataSet();
					SqlCommand cmdUserTypeID = new SqlCommand("SELECT userTypeID FROM usertype WHERE isDeleted = 0 AND userTypeName = @usertype", conn);
					cmdUserTypeID.Parameters.Add(new SqlParameter("@usertype", userType));

					adapt = new SqlDataAdapter(cmdUserTypeID);
					adapt.Fill(dsUserTypeID, "dsUserTypeID");

					String userTypeID = "";

					if (dsUserTypeID.Tables["dsUserTypeID"].Rows.Count == 0)
					{
						success = false;

					}
					else
					{
						userTypeID = dsUserTypeID.Tables["dsUserTypeID"].Rows[0]["userTypeID"].ToString();
					}

					if (success)
					{
						//update user information record
						string sql, result;
						SqlCommand cmdUpdate, cmdInsertLog, cmdInsertGuardian;

						// check for password update
						sql = "SELECT password FROM dbo.[user] WHERE userID = '" + userID + "'";
						result = dl.GetSingleValue(sql);
						if (result != password)
						{
							cmdUpdate = new SqlCommand("UPDATE dbo.[user] SET password=@password where userID = @userID", conn);
							cmdUpdate.Parameters.Add(new SqlParameter("@password", password));
							cmdUpdate.Parameters.Add(new SqlParameter("@userID", userID));
							cmdUpdate.ExecuteNonQuery();

							cmdInsertLog = new SqlCommand("INSERT INTO Log (oldLogData, logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@oldLogData, @logData, 'Update user information - " + fName + " " + lName + "', 8, @adminID, @adminID, 'user', 'password', '" + userID + "', 1)", conn);
							cmdInsertLog.Parameters.Add(new SqlParameter("@oldLogData", result));
							cmdInsertLog.Parameters.Add(new SqlParameter("@logData", password));
							cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
							cmdInsertLog.ExecuteNonQuery();
						}

						// check for firstName update
						sql = "SELECT firstName FROM dbo.[user] WHERE userID = '" + userID + "'";
						result = dl.GetSingleValue(sql);
						if (result != fName)
						{
							cmdUpdate = new SqlCommand("UPDATE dbo.[user] SET firstName=@firstName where userID = @userID", conn);
							cmdUpdate.Parameters.Add(new SqlParameter("@firstName", fName));
							cmdUpdate.Parameters.Add(new SqlParameter("@userID", userID));
							cmdUpdate.ExecuteNonQuery();

							cmdInsertLog = new SqlCommand("INSERT INTO Log (oldLogData, logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@oldLogData, @logData, 'Update user information - " + fName + " " + lName + "', 8, @adminID, @adminID, 'user', 'firstName', '" + userID + "', 1)", conn);
							cmdInsertLog.Parameters.Add(new SqlParameter("@oldLogData", result));
							cmdInsertLog.Parameters.Add(new SqlParameter("@logData", fName));
							cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
							cmdInsertLog.ExecuteNonQuery();
						}

						// check for lastName update
						sql = "SELECT lastName FROM dbo.[user] WHERE userID = '" + userID + "'";
						result = dl.GetSingleValue(sql);
						if (result != lName)
						{
							cmdUpdate = new SqlCommand("UPDATE dbo.[user] SET lastName=@lastName where userID = @userID", conn);
							cmdUpdate.Parameters.Add(new SqlParameter("@lastName", lName));
							cmdUpdate.Parameters.Add(new SqlParameter("@userID", userID));
							cmdUpdate.ExecuteNonQuery();

							cmdInsertLog = new SqlCommand("INSERT INTO Log (oldLogData, logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@oldLogData, @logData, 'Update user information - " + fName + " " + lName + "', 8, @adminID, @adminID, 'user', 'lastName', '" + userID + "', 1)", conn);
							cmdInsertLog.Parameters.Add(new SqlParameter("@oldLogData", result));
							cmdInsertLog.Parameters.Add(new SqlParameter("@logData", lName));
							cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
							cmdInsertLog.ExecuteNonQuery();
						}

						// check for address update
						sql = "SELECT address FROM dbo.[user] WHERE userID = '" + userID + "'";
						result = dl.GetSingleValue(sql);
						if (result != address)
						{
							cmdUpdate = new SqlCommand("UPDATE dbo.[user] SET address=@address where userID = @userID", conn);
							cmdUpdate.Parameters.Add(new SqlParameter("@address", address));
							cmdUpdate.Parameters.Add(new SqlParameter("@userID", userID));
							cmdUpdate.ExecuteNonQuery();

							cmdInsertLog = new SqlCommand("INSERT INTO Log (oldLogData, logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@oldLogData, @logData, 'Update user information - " + fName + " " + lName + "', 8, @adminID, @adminID, 'user', 'address', '" + userID + "', 1)", conn);
							cmdInsertLog.Parameters.Add(new SqlParameter("@oldLogData", result));
							cmdInsertLog.Parameters.Add(new SqlParameter("@logData", address));
							cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
							cmdInsertLog.ExecuteNonQuery();
						}

						// check for officeNo update
						sql = "SELECT officeNo FROM dbo.[user] WHERE userID = '" + userID + "'";
						result = dl.GetSingleValue(sql);
						if (result != officeNo)
						{
							cmdUpdate = new SqlCommand("UPDATE dbo.[user] SET officeNo=@officeNo where userID = @userID", conn);
							cmdUpdate.Parameters.Add(new SqlParameter("@officeNo", officeNo));
							cmdUpdate.Parameters.Add(new SqlParameter("@userID", userID));
							cmdUpdate.ExecuteNonQuery();

							cmdInsertLog = new SqlCommand("INSERT INTO Log (oldLogData, logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@oldLogData, @logData, 'Update user information - " + fName + " " + lName + "', 8, @adminID, @adminID, 'user', 'officeNo', '" + userID + "', 1)", conn);
							cmdInsertLog.Parameters.Add(new SqlParameter("@oldLogData", result));
							cmdInsertLog.Parameters.Add(new SqlParameter("@logData", officeNo));
							cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
							cmdInsertLog.ExecuteNonQuery();
						}

						// check for handphoneNo update
						sql = "SELECT handphoneNo FROM dbo.[user] WHERE userID = '" + userID + "'";
						result = dl.GetSingleValue(sql);
						if (result != handphoneNo)
						{
							cmdUpdate = new SqlCommand("UPDATE dbo.[user] SET handphoneNo=@handphoneNo where userID = @userID", conn);
							cmdUpdate.Parameters.Add(new SqlParameter("@handphoneNo", handphoneNo));
							cmdUpdate.Parameters.Add(new SqlParameter("@userID", userID));
							cmdUpdate.ExecuteNonQuery();

							cmdInsertLog = new SqlCommand("INSERT INTO Log (oldLogData, logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@oldLogData, @logData, 'Update user information - " + fName + " " + lName + "', 8, @adminID, @adminID, 'user', 'handphoneNo', '" + userID + "', 1)", conn);
							cmdInsertLog.Parameters.Add(new SqlParameter("@oldLogData", result));
							cmdInsertLog.Parameters.Add(new SqlParameter("@logData", handphoneNo));
							cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
							cmdInsertLog.ExecuteNonQuery();
						}

						// check for gender update
						sql = "SELECT gender FROM dbo.[user] WHERE userID = '" + userID + "'";
						result = dl.GetSingleValue(sql);
						if (result != gender.ToString())
						{
							cmdUpdate = new SqlCommand("UPDATE dbo.[user] SET gender=@gender where userID = @userID", conn);
							cmdUpdate.Parameters.Add(new SqlParameter("@gender", gender));
							cmdUpdate.Parameters.Add(new SqlParameter("@userID", userID));
							cmdUpdate.ExecuteNonQuery();

							cmdInsertLog = new SqlCommand("INSERT INTO Log (oldLogData, logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@oldLogData, @logData, 'Update user information - " + fName + " " + lName + "', 8, @adminID, @adminID, 'user', 'gender', '" + userID + "', 1)", conn);
							cmdInsertLog.Parameters.Add(new SqlParameter("@oldLogData", result));
							cmdInsertLog.Parameters.Add(new SqlParameter("@logData", gender));
							cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
							cmdInsertLog.ExecuteNonQuery();
						}

						// check for DOB update
						sql = "SELECT DOB FROM dbo.[user] WHERE userID = '" + userID + "'";
						result = dl.GetSingleValue(sql);
						string convertedDT = Convert.ToDateTime(dob).ToString("yyyy-MM-dd");
						if (result != Convert.ToDateTime(dob).ToString())
						{
							cmdUpdate = new SqlCommand("UPDATE dbo.[user] SET DOB=@DOB where userID = @userID", conn);
							cmdUpdate.Parameters.Add(new SqlParameter("@DOB", Convert.ToDateTime(dob)));
							cmdUpdate.Parameters.Add(new SqlParameter("@userID", userID));
							cmdUpdate.ExecuteNonQuery();

							cmdInsertLog = new SqlCommand("INSERT INTO Log (oldLogData, logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@oldLogData, @logData, 'Update user information - " + fName + " " + lName + "', 8, @adminID, @adminID, 'user', 'DOB', '" + userID + "', 1)", conn);
							cmdInsertLog.Parameters.Add(new SqlParameter("@oldLogData", result));
							cmdInsertLog.Parameters.Add(new SqlParameter("@logData", convertedDT));
							cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
							cmdInsertLog.ExecuteNonQuery();
						}

						// check for userTypeID update
						sql = "SELECT userTypeID FROM dbo.[user] WHERE userID = '" + userID + "'";
						result = dl.GetSingleValue(sql);
						if (result != userTypeID)
						{
							cmdUpdate = new SqlCommand("UPDATE dbo.[user] SET userTypeID=@userTypeID where userID = @userID", conn);
							cmdUpdate.Parameters.Add(new SqlParameter("@userTypeID", userTypeID));
							cmdUpdate.Parameters.Add(new SqlParameter("@userID", userID));
							cmdUpdate.ExecuteNonQuery();

							cmdInsertLog = new SqlCommand("INSERT INTO Log (oldLogData, logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@oldLogData, @logData, 'Update user information - " + fName + " " + lName + "', 8, @adminID, @adminID, 'user', 'userTypeID', '" + userID + "', 1)", conn);
							cmdInsertLog.Parameters.Add(new SqlParameter("@oldLogData", result));
							cmdInsertLog.Parameters.Add(new SqlParameter("@logData", userTypeID));
							cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
							cmdInsertLog.ExecuteNonQuery();
						}

						// check for patientID update
						//if result = 0, it does not have guardianAllocation
						sql = "SELECT count(*) FROM dbo.[guardianAllocation] WHERE guardianID = '" + userID + "' AND isDeleted='0' AND isApproved='1'";
						result = dl.GetSingleValue(sql);

						if (result != "0")
						{
							sql = "SELECT patientID FROM dbo.[guardianAllocation] WHERE guardianID = '" + userID + "' AND isDeleted='0' AND isApproved='1'";
							result = dl.GetSingleValue(sql);
						}

						if (result != patientID.ToString())
						{
							//if (result == "0")
							//{
							//    result = "";
							//}

							if (patientID != 0 && result == "0")
							{
								cmdInsertGuardian = new SqlCommand("INSERT INTO dbo.[guardianAllocation] (patientID, guardianID, isDeleted, isApproved) OUTPUT INSERTED.guardianAllocationID Values (@patientID, @guardianID, 0, 1)", conn);
								cmdInsertGuardian.Parameters.Add(new SqlParameter("@patientID", patientID));
								cmdInsertGuardian.Parameters.Add(new SqlParameter("@guardianID", userID));
								int newGuardianId = Convert.ToInt32(cmdInsertGuardian.ExecuteScalar());

								String logData = "patientID:" + patientID + ";guardianID:" + userID;
								cmdInsertLog = new SqlCommand("INSERT INTO Log (logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@logData, 'Create new Guardian Allocation - " + fName + " " + lName + "', 2, @adminID, @adminID, 'guardianAllocation', 'all', '" + newGuardianId + "', 1)", conn);
								cmdInsertLog.Parameters.Add(new SqlParameter("@logData", logData));
								cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
								cmdInsertLog.ExecuteNonQuery();
							}
							else if (patientID == 0 && result != "0")
							{
								cmdUpdate = new SqlCommand("DECLARE @IDs TABLE(ID INT); UPDATE dbo.[guardianAllocation] SET isDeleted=1 OUTPUT INSERTED.guardianAllocationID INTO @IDs(ID) where guardianID = @userID AND isDeleted='0' AND isApproved='1' SELECT ID FROM @IDs;", conn);
								cmdUpdate.Parameters.Add(new SqlParameter("@userID", userID));
								int rowAffected = Convert.ToInt32(cmdUpdate.ExecuteScalar());


								String logData = "patientID:" + result + ";guardianID:" + userID;
								cmdInsertLog = new SqlCommand("INSERT INTO Log (logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@logData, 'Delete Guardian Allocation - " + fName + " " + lName + "', 12, @adminID, @adminID, 'guardianAllocation', 'All', '" + rowAffected + "', 1)", conn);
								cmdInsertLog.Parameters.Add(new SqlParameter("@logData", logData));
								cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
								cmdInsertLog.ExecuteNonQuery();
							}
							else
							{
								cmdUpdate = new SqlCommand("DECLARE @IDs TABLE(ID INT); UPDATE dbo.[guardianAllocation] SET patientID = @patientID OUTPUT INSERTED.guardianAllocationID INTO @IDs(ID) where guardianID = @userID AND isDeleted='0' AND isApproved='1' SELECT ID FROM @IDs;", conn);
								cmdUpdate.Parameters.Add(new SqlParameter("@patientID", patientID));
								cmdUpdate.Parameters.Add(new SqlParameter("@userID", userID));
								int rowAffected = Convert.ToInt32(cmdUpdate.ExecuteScalar());

								cmdInsertLog = new SqlCommand("INSERT INTO Log (oldLogData,logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@oldLogData, @logData, 'Update Guardian Allocation - " + fName + " " + lName + "', 4, @adminID, @adminID, 'guardianAllocation', 'patientID', '" + rowAffected + "', 1)", conn);
								cmdInsertLog.Parameters.Add(new SqlParameter("@oldLogData", result));
								cmdInsertLog.Parameters.Add(new SqlParameter("@logData", patientID));
								cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
								cmdInsertLog.ExecuteNonQuery();
							}
						}
					}
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

			return success;
		}

		public bool DeleteAccount(int adminID)
		{
			bool success = true;
			dataclass dl = new dataclass();
			string connection = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString.ToString();
			using (SqlConnection conn = new SqlConnection(connection))
			{
				try
				{
					conn.Open();
					SqlCommand cmdDelete = new SqlCommand("UPDATE dbo.[user] SET isDeleted= '1' where userID = @userID", conn);
					cmdDelete.Parameters.Add(new SqlParameter("@userID", userID));

					cmdDelete.ExecuteNonQuery();

					//Insert into log
					string convertedDT = Convert.ToDateTime(dob).ToString("yyyy-MM-dd");
					string logData = "firstName:" + fName + ";lastName:" + lName + ";address:" + address + ";email:" + email + ";handphone:" + handphoneNo + ";officeNo:" + officeNo + ";gender:" + gender + ";dob:" + convertedDT + ";userType:" + userType;
					if (patientID != 0)
					{


						SqlCommand cmdUpdate = new SqlCommand("DECLARE @IDs TABLE(ID INT); UPDATE dbo.[guardianAllocation] SET isDeleted=1 OUTPUT INSERTED.guardianAllocationID INTO @IDs(ID) where guardianID = @userID AND isDeleted='0' AND isApproved='1'  SELECT ID FROM @IDs;", conn);
						cmdUpdate.Parameters.Add(new SqlParameter("@userID", userID));
						int rowAffected = Convert.ToInt32(cmdUpdate.ExecuteScalar());

						String logDataGuardian = "patientID:" + patientID + ";guardianID:" + userID;
						SqlCommand cmdInsertLogGuardian = new SqlCommand("INSERT INTO Log (logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@logData, 'Delete Guardian Allocation - " + fName + " " + lName + "', 12, @adminID, @adminID, 'guardianAllocation', 'All', '" + rowAffected + "', 1)", conn);
						cmdInsertLogGuardian.Parameters.Add(new SqlParameter("@logData", logDataGuardian));
						cmdInsertLogGuardian.Parameters.Add(new SqlParameter("@adminID", adminID));
						cmdInsertLogGuardian.ExecuteNonQuery();
					}
					SqlCommand cmdInsertLog = new SqlCommand("INSERT INTO Log (logData, logDesc, logCategoryID, userIDInit, userIDApproved, tableAffected, columnAffected, rowAffected, approved) Values (@logData, 'Delete user - " + fName + " " + lName + "', 7, @adminID, @adminID, 'user', 'All', '" + userID + "', 1)", conn);
					cmdInsertLog.Parameters.Add(new SqlParameter("@adminID", adminID));
					cmdInsertLog.Parameters.Add(new SqlParameter("@logData", logData));
					cmdInsertLog.ExecuteNonQuery();

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

			return success;
		}
	}
}