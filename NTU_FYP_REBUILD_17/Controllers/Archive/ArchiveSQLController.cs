using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NTU_FYP_REBUILD_17.App_Code;
using System.Configuration;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using ClosedXML.Excel;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web.UI.WebControls;
using NTU_FYP_REBUILD_17.ViewModels;
using NTU_FYP_REBUILD_17.Models;
using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Newtonsoft.Json;
using System.Web.Routing;

namespace NTU_FYP_REBUILD_17.Controllers.Archive
{
	public class ArchiveSQLController : Controller
	{
		private App_Code.SOLID shortcutMethod = new App_Code.SOLID();
		private ApplicationDbContext _context;
		private const string currentdatabasename = "fypcom_peardb";
		private const string currentdatabasepass = "6Tnl78v^";
		public ArchiveSQLController()
		{
			_context = new ApplicationDbContext();
		}

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class NoDirectAccessAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Request.UrlReferrer == null ||
                            filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
                {
                    filterContext.Result = new RedirectToRouteResult(new
                                   RouteValueDictionary(new { controller = "Home", action = "Index", area = "" }));
                }
            }
        }

        [NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult home()
		{
			return View();
		}


		//Must be same Object
		public objectNewOld compareOldandNewObject(Object oldObject, Object newObject)
		{
			shortcutMethod.printf(JsonConvert.SerializeObject(oldObject));
			shortcutMethod.printf(JsonConvert.SerializeObject(newObject));
			var o1 = oldObject;
			var o2 = newObject;
			var differences = o1.CompareObj(o2);
			JObject oldDatajOBJ = new JObject();
			JObject newDatajOBJ = new JObject();
			for (int i = 0; i < differences.Count(); i++)
			{
				string typeA = differences[i].valA.GetType().ToString();
				string typeB = differences[i].valB.GetType().ToString();
				if (typeA.Contains("Int") || typeB.Contains("Int"))
				{
					oldDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valA.ToString()));
					newDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valB.ToString()));
				}
				else
				{
					oldDatajOBJ.Add(differences[i].PropertyName, differences[i].valA.ToString());
					newDatajOBJ.Add(differences[i].PropertyName, differences[i].valB.ToString());
				}
			}
			objectNewOld oNO = new objectNewOld();
			oNO.newData = oldDatajOBJ.ToString();
			oNO.oldData = newDatajOBJ.ToString();
			shortcutMethod.printf("OldData=" + oldDatajOBJ.ToString());
			shortcutMethod.printf("NewData=" + newDatajOBJ.ToString());
			return oNO;
		}


		//Database Details. Edit the password here.
		[Authorize(Roles = RoleName.isAdmin)]
		public string getSQLconnection_string(string dbname)
		{
			string connection_string =
			"Data Source= 124.6.61.66;" +
			"Initial Catalog = " + dbname + ";" +
			"User ID = fypcom_fypcom;" +
			"Password = "+currentdatabasepass+";" +
			"Connect Timeout = 15;";
			return connection_string;
		}

        // Upload script function
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult Index()
		{
			ArchiveSQLViewModel model = new ArchiveSQLViewModel();
			return View(viewBagItems(model));
		}

		[Authorize(Roles = RoleName.isAdmin)]
		public int checkToSeeIfSnapShotisAllowed()
		{

			int canSnapShot = 0;
			string currentYear = DateTime.Now.Year.ToString();
			shortcutMethod.printf("Current Year" + currentYear);
			string query = "Select * from log where createDateTime < '" + currentYear + "'";
			string query2 = "Select * from schedule where createDateTime < '" + currentYear + "'";
			System.Data.DataTable dt = executeSELECTSQL(getSQLconnection_string(currentdatabasename), query);
			System.Data.DataTable dt2 = executeSELECTSQL(getSQLconnection_string(currentdatabasename), query2);

			if (dt.Rows.Count > 0 || dt2.Rows.Count > 0) // Contains records lesser than currentYear. Can Archive.
			{
				canSnapShot = 0;
				string query3 = "Select * from log where createDateTime < '" + currentYear + "' and approved=0 and reject=0 AND isDeleted=0";
				System.Data.DataTable dt3 = executeSELECTSQL(getSQLconnection_string(currentdatabasename), query3);
				if (dt3.Rows.Count > 0)
					canSnapShot = 1;
			}
			else
				canSnapShot = 2;
			return canSnapShot;
		}

		[Authorize(Roles = RoleName.isAdmin)]
		public void removePastYearTransactionRecords()
		{
			string currentYear = DateTime.Now.Year.ToString();
			string query = "delete from log where createDateTime < '"+currentYear+"'";
			string query2 = "delete from gameRecord where createDateTime < '" + currentYear + "'";
			string query3 = "delete from Schedule where createDateTime < '" + currentYear + "'";

			string sqlConnectionString = getSQLconnection_string(currentdatabasename);
			using (SqlConnection connection = new SqlConnection(sqlConnectionString))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand(query, connection);
				cmd.CommandTimeout = 0;
				cmd.ExecuteNonQuery();
				SqlCommand cmd2 = new SqlCommand(query2, connection);
				cmd2.CommandTimeout = 0;
				cmd2.ExecuteNonQuery();
				SqlCommand cmd3 = new SqlCommand(query3, connection);
				cmd3.CommandTimeout = 0;
				cmd3.ExecuteNonQuery();
				connection.Close();
			}
		}

		[Authorize(Roles = RoleName.isAdmin)]
		public void doasnapshotToExcelWs()
		{
			string year = DateTime.Now.Year.ToString();
			List<databaseObj> dbobj = new List<databaseObj>();
			DataSet ds = new DataSet();
			dbobj = getAllTableNameFieldNameOfADatabase(currentdatabasename);
			
			for(int i=0; i< dbobj.Count; i++)
			{
				string query = "select * from " + dbobj[i].databaseTableName;
				System.Data.DataTable dt = new System.Data.DataTable();
				dt = executeSELECTSQL(getSQLconnection_string(currentdatabasename), query);
				dt.TableName = dbobj[i].databaseTableName;
				ds.Tables.Add(dt);
			}

			List<excelZip> excelZipList = new List<excelZip>();
			excelZip excelZip = new excelZip();
			excelZip.datacontent = ds;
			excelZip.filename = "Snapshot_Of_DementiaDBfyp3_Taken";
			excelZipList.Add(excelZip);
			createExcelthenPassZip(excelZipList);
		}

		// Upload script function
		[Authorize(Roles = RoleName.isAdmin)]
		[HttpPost]
		public ActionResult Index(HttpPostedFileBase file, ArchiveSQLViewModel model)
		{
			model.post = 0;
			List<int> readSETANSI = new List<int>();
			if (model.newDBname == null)
			{
				ViewBag.Message = "Please select a empty database to import.";
				return View(viewBagItems(model));
			}

			if (file != null && file.ContentLength > 0)
				try
				{
					List<string> lines = new List<string>();
					using (StreamReader reader = new StreamReader(file.InputStream))
					{
						while (!reader.EndOfStream)
						{
							lines.Add(reader.ReadLine());
						}
					}
					shortcutMethod.printf("DONE");

					for (int i = 0; i < lines.Count(); i++)
					{
						if (lines[i].Equals("SET ANSI_NULLS ON"))
						{
							readSETANSI.Add(i);
							break;
						}
					}
					lines[lines.Count() - 2] = "ALTER DATABASE [" + model.newDBname + "] SET  READ_ONLY";
					lines.RemoveRange(0, readSETANSI[0]);

					for (int i = 0; i < lines.Count(); i++)
					{
						if (lines[i].Equals("GO"))
						{
							lines.RemoveAt(i);
							i--;
						}
					}

					string combindedString = string.Join("\n", lines.ToArray());
					runSQLscript(combindedString, model.newDBname);
					doasnapshotToExcelWs();
					removePastYearTransactionRecords();
					ViewBag.Message = "File uploaded successfully";
					
				}
				catch (Exception ex)
				{
					ViewBag.Message = "ERROR:" + ex.Message.ToString();
				}
			else
			{
				ViewBag.Message = "You have not specified a file.";
			}
			return View(viewBagItems(model));
		}

		// Used for upload SQL script after db is created.
		// Execute SQL "script".
		[Authorize(Roles = RoleName.isAdmin)]
		public void runSQLscript(string script, string emptyDB)
		{
			string sqlConnectionString = getSQLconnection_string(emptyDB);

			using (SqlConnection connection = new SqlConnection(sqlConnectionString))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand(script, connection);
				cmd.CommandTimeout = 0;
				cmd.ExecuteNonQuery();
				connection.Close();
			}
		}

		// Upload script function
		// Find available database and look for empty database.
		[Authorize(Roles = RoleName.isAdmin)]
		public ArchiveSQLViewModel viewBagItems(ArchiveSQLViewModel model)
		{
			string conn_string = getSQLconnection_string(currentdatabasename);
			List<String> availableDB = new List<String>();
			List<String> emptyDB = new List<String>();
			using (SqlConnection connection = new SqlConnection(conn_string))
			{
				connection.Open();
				string query = "SELECT name FROM sys.databases where name LIKE 'fypcom_%' and name NOT like '%snapshot2017_dementiafypdb3%' and name NOT like '%snapshot2016_dementiafypdb3%' ORDER BY create_date DESC";
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							availableDB.Add(reader.GetString(0));
						}
					}
				}
			}

			using (SqlConnection connection = new SqlConnection(conn_string))
			{
				connection.Open();
				for (int i = 0; i < availableDB.Count(); i++)
				{
					string query = "USE " + availableDB[i] + " SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
					using (SqlCommand command = new SqlCommand(query, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								if (reader.GetInt32(0) == 0)
								{
									emptyDB.Add(availableDB[i]);
								}
							}
						}
					}
				}
			}
			ViewBag.DatabaseAvailable = availableDB;
			ViewBag.EmptyDB = emptyDB;
			model.post = 0;
			if (checkToSeeIfSnapShotisAllowed() == 2)
			{
				ViewBag.Message = "There are no past year transaction records to perform snapshot; Upload button has been removed.";
				model.post = 1;
			}

			else if (checkToSeeIfSnapShotisAllowed() == 1)
			{
				ViewBag.Message = "There are still transcation records that are needed to be approved by supervisor. Upload button has been removed.";
				model.post = 1;
			}
			return model;
		}

		[Authorize(Roles = RoleName.isAdmin)]
		public ArchiveSQLViewModel displayfilterView(ArchiveSQLViewModel model)
		{
			List<dbTableObjViewModel> tableInfo = new List<dbTableObjViewModel>();
			List<string> alltablename = getAllTablenameSQL(currentdatabasename);
			for (int i =0; i<alltablename.Count();i++)
			{
				dbTableObjViewModel dbTable = new dbTableObjViewModel();
				dbTable.tablename = alltablename[i];
				dbTable.checktablename = true;
				if (alltablename[i].Equals("schedule") || alltablename[i].Equals("log") || alltablename[i].Equals("gameRecord"))
					dbTable.checktablename = false;
				dbTable.abbreviation = "";
				tableInfo.Add(dbTable);
			}
			model.allTablename = tableInfo;
			ViewBag.Compare = compareList();
			return model;
		}

		// Display the UI page for search page
		[Authorize(Roles = RoleName.isAdmin)]
		public ArchiveSQLViewModel displaysearchView()
		{
			ArchiveSQLViewModel model = new ArchiveSQLViewModel();
			string conn_string = getSQLconnection_string(currentdatabasename);
			List<availableDBObj> availableDB = new List<availableDBObj>();
			using (SqlConnection connection = new SqlConnection(conn_string))
			{
				connection.Open();
				string queryAllAvailableDB = "SELECT name FROM sys.databases where name LIKE 'fypcom_%' and name NOT like '%snapshot2017_dementiafypdb3%' and name NOT like '%snapshot2016_dementiafypdb3%' ORDER BY create_date DESC";
				using (SqlCommand command = new SqlCommand(queryAllAvailableDB, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							availableDBObj dbobj = new availableDBObj();
							dbobj.dbname = reader.GetString(0);
							dbobj.checkdbname = true;
							if (dbobj.dbname.Equals(currentdatabasename))
								dbobj.abbreviation = "Current" + "("+ dbobj.dbname+")";
							else
								dbobj.abbreviation = Regex.Match(dbobj.dbname, @"\d+").Value + "(" + dbobj.dbname + ")";
							availableDB.Add(dbobj);
						}
					}
				}
				for (int i = 0; i < availableDB.Count(); i++)
				{
					queryAllAvailableDB = "USE " + availableDB[i].dbname + " SELECT COUNT(*) AS count FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
					int count = executeSELECTSQL(getSQLconnection_string(availableDB[i].dbname), queryAllAvailableDB).AsEnumerable().Select(r => r.Field<int>("count")).SingleOrDefault(); ;
					if (count == 0)
						availableDB.Remove(availableDB[i]);
				}

				model.allAvailableDB = availableDB;
				model.post = 0;
			}
			ViewBag.Compare = compareList();

			string queryListMedication = "select value from list_prescription";
			DataTable medicationDTresult = executeSELECTSQL(conn_string, queryListMedication);
			var list = medicationDTresult.AsEnumerable().Select(r => r.Field<string>("value")).ToList(); //linq
			ViewBag.Medication = list;
			return model;
		}

		[Authorize(Roles = RoleName.isAdmin)]
		public List<string> compareList()
		{
			List<string> charlist = new List<string>();
			charlist.Add("Later than");
			charlist.Add("Earlier than");
			charlist.Add("Equal to");
			return charlist;
		}

		[Authorize(Roles = RoleName.isAdmin)]
		// Execute Select SQL query. run sql select. run select.
		public DataTable executeSELECTSQL(string conn_string, string query)
		{
			DataTable dt = new DataTable();
			var columnNames = new List<string>();
			using (SqlConnection connection = new SqlConnection(conn_string))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						dt.Load(reader);
					}
				}
			}
			//foreach (DataRow row in dt.Rows)
			//{
			//	foreach (DataColumn column in dt.Columns)
			//	{
			//		shortcutMethod.printf("returnedSELECTSQLresult:Column:" + column.ColumnName + ":Row:" + row[column].ToString());
			//	}
			//}
			return dt;
		}

		[Authorize(Roles = RoleName.isAdmin)]
		public ActionResult search()
		{
			return View(displaysearchView());
		}

		[HttpPost]
		[Authorize(Roles = RoleName.isAdmin)]
		public ActionResult search(ArchiveSQLViewModel model)
		{
			List<DataTable> listSelectResult = new List<DataTable>();
			List<yearObj> searchYear = new List<yearObj>();
			List<int> divider = new List<int>();
			List<getPatientIDandYear> listGetPatientIDandYear = new List<getPatientIDandYear>();
			int x = 0;
			for (int i = 0; i < model.allAvailableDB.Count(); i++)
			{
				shortcutMethod.printf(model.allAvailableDB[i].abbreviation + "DBname:" + model.allAvailableDB[i].dbname + "checked:" + model.allAvailableDB[i].checkdbname);
				if (model.allAvailableDB[i].checkdbname)
				{
					divider.Add(x);
					List<DataTable> dt = searchforpatient(model, model.allAvailableDB[i].dbname);
					listSelectResult.AddRange(dt);
					x = x + dt.Count();
					yearObj yo = new yearObj();
					yo.abbreviation = model.allAvailableDB[i].abbreviation;
					yo.name = model.allAvailableDB[i].dbname;
					searchYear.Add(yo);
				}
			}

			ArchiveSQLViewModel displayViewModel = displaysearchView();
			displayViewModel.post = 1;
			if (listSelectResult.Count() == 0)
				displayViewModel.post = 2;
			displayViewModel.listSelectResult = listSelectResult;
			displayViewModel.searchYear = searchYear;
			displayViewModel.divider = divider;
			return View(displayViewModel);
		}

		[Authorize(Roles = RoleName.isAdmin)]
		public string checkEmpty(string field)
		{
			if (field == null)
				return "-1";
			return field;
		}

		[Authorize(Roles = RoleName.isAdmin)]
		public List<DataTable> searchforpatient(ArchiveSQLViewModel model, string curretDBname)
		{
			//Data From View
			string nric = checkEmpty(model.nric);
			string name = checkEmpty(model.name);
			string hpno = checkEmpty(model.hpno);
			string email = checkEmpty(model.email);
			List<availableDBObj> allAvailableDB = model.allAvailableDB;
			string allergy = checkEmpty(model.allergy);
			string medication = checkEmpty(model.medication);
			string keywordSearch = checkEmpty(model.keywordsearchAllDB);
			bool approvedYes = model.approvedYes;
			bool approvedNo = model.approvedNo;
			bool deletedYes = model.deletedYes;
			bool deletedNo = model.deletedNo;
			//DateTime createdDate = model.createdDate;
			//createdDate = DateTime.Parse("001/1/1 12:00:00 AM");
			//if (createdDate.ToString().Equals("1/1/0001 12:00:00 AM"))
			//	createdDate = DateTime.Parse("001/1/1 12:00:00 AM");

			if (medication.Equals("Others"))
				medication = checkEmpty(model.medicationOthers);

			//used here
			List<DataTable> allPatientIDResult = new List<DataTable>();
			DataTable returnSQL = new DataTable();
			List<int> patientIDlist = new List<int>();
			List<int> patientID = new List<int>();

			string search = "";
			if (deletedYes)
				search = search + " isDeleted";

			string searchbyQuery = "select patientID from patient where nric LIKE '%" + nric + "%' OR firstname LIKE '%" + name + "%' OR lastname LIKE '%" + name + "%' OR handphoneNo LIKE '%" + hpno + "%' OR guardianEmail LIKE '%" + email + "%'";
			string dbconnectionstring = getSQLconnection_string(curretDBname);
			returnSQL = executeSELECTSQL(dbconnectionstring, searchbyQuery);


			patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
			shortcutMethod.printf(patientID.Count() + "Count total patient");
			patientIDlist.AddRange(patientID);



			List<int> allergyPatient = new List<int>();
			for (int i = 0; i < patientIDlist.Count(); i++)
			{
				string allergyQuery = "select patientID from allergy where allergy LIKE '%" + allergy + "%' and patientID LIKE '%" + patientIDlist[i] + "%'";
				returnSQL = executeSELECTSQL(dbconnectionstring, allergyQuery);
				patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
				allergyPatient.AddRange(patientID);
			}

			if (patientIDlist.Count() == 0)
			{
				string allergyQuery = "select patientID from allergy where allergy LIKE '%" + allergy + "%'";
				returnSQL = executeSELECTSQL(dbconnectionstring, allergyQuery);
				patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
				patientIDlist.AddRange(patientID);
			}
			else
			{
				if (!allergy.Equals("-1"))
				{
					patientIDlist.Clear();
					patientIDlist.AddRange(allergyPatient);
				}

			}

			List<int> prescriptionPatient = new List<int>();
			for (int i=0; i< patientIDlist.Count();i++)
			{
				string medicationQuery = "select patientID from prescription where drugName LIKE '%" + medication + "%' and patientID LIKE '%"+ patientIDlist[i]+ "%'";
				returnSQL = executeSELECTSQL(dbconnectionstring, medicationQuery);
				patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
				prescriptionPatient.AddRange(patientID);
			}
			if (patientIDlist.Count() == 0)
			{
				string medicationQuery = "select patientID from prescription where drugName LIKE '%" + medication + "%'";
				returnSQL = executeSELECTSQL(dbconnectionstring, medicationQuery);
				patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
				patientIDlist.AddRange(patientID);
			}
			else
			{
				if (!medication.Equals("-1"))
				{
					patientIDlist.Clear();
					patientIDlist.AddRange(prescriptionPatient);
				}
			}


			if (!keywordSearch.Equals("-1"))
			{
				List<databaseObj> databaseObjList = new List<databaseObj>();
				databaseObjList = getAllTableNameFieldNameOfADatabase(curretDBname);
				for(int i=0;i< databaseObjList.Count();i++)
				{
					for(int j=0; j<databaseObjList[i].databaseFieldName.Count(); j++)
					{
						if(databaseObjList[i].databaseFieldName[j].Contains("patientID"))
						{
							for(int l=0; l< databaseObjList[i].databaseFieldName.Count(); l++)
							{
								List<int?> patientID2 = new List<int?>();
								string query = "Select patientID from " + databaseObjList[i].databaseTableName + " where " + databaseObjList[i].databaseFieldName[l] + " LIKE '%" + keywordSearch + "%'";
								returnSQL = executeSELECTSQL(dbconnectionstring, query);
								shortcutMethod.printf("Error check:" + databaseObjList[i].databaseTableName);
								patientID2 = returnSQL.AsEnumerable().Select(r => r.Field<int?>("patientID")).ToList();
								for(int m=0; m<patientID2.Count();m++)
								{
									patientID.Add(patientID2[m].GetValueOrDefault(0));
									patientID.Remove(0);
								}
								patientIDlist.AddRange(patientID);
							}
							break;
						}

						if (databaseObjList[i].databaseFieldName[j].Contains("patientAllocationID"))
						{
							List<int> patientAllocationID = new List<int>();
							for (int l = 0; l < databaseObjList[i].databaseFieldName.Count(); l++)
							{	
								string query = "Select patientAllocationID from " + databaseObjList[i].databaseTableName + " where " + databaseObjList[i].databaseFieldName[l] + " LIKE '%" + keywordSearch + "%'";
								returnSQL = executeSELECTSQL(dbconnectionstring, query);
								patientAllocationID.AddRange(returnSQL.AsEnumerable().Select(r => r.Field<int>("patientAllocationID")));
							}

							for(int k=0;k<patientAllocationID.Count(); k++)
							{
								string query2 = "Select patientID from patientAllocation where patientAllocationID = " + patientAllocationID[k];
								returnSQL = executeSELECTSQL(dbconnectionstring, query2);
								patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
								patientIDlist.AddRange(patientID);
							}
							break;
						}

						if (databaseObjList[i].databaseFieldName[j].Contains("socialHistoryID"))
						{

							List<int> socialHistoryID = new List<int>();
							for (int l = 0; l < databaseObjList[i].databaseFieldName.Count(); l++)
							{
								string query = "Select socialHistoryID from " + databaseObjList[i].databaseTableName + " where " + databaseObjList[i].databaseFieldName[l] + " LIKE '%" + keywordSearch + "%'";
								shortcutMethod.printf(query+"abcdef");
								returnSQL = executeSELECTSQL(dbconnectionstring, query);
								socialHistoryID.AddRange(returnSQL.AsEnumerable().Select(r => r.Field<int>("socialHistoryID")));
							}

							for (int k = 0; k < socialHistoryID.Count(); k++)
							{
								shortcutMethod.printf("IN socialHistoryID keyword" + socialHistoryID[k]);
								string query2 = "Select patientID from socialHistory where socialHistoryID = " + socialHistoryID[k];
								returnSQL = executeSELECTSQL(dbconnectionstring, query2);
								patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
								patientIDlist.AddRange(patientID);
							}
							break;
						}
					}
				}
			}

			//if (approvedYes && createdDate.ToString().Equals("001/1/1 12:00:00 AM") && patientIDlist.Count==0)
			if (approvedYes && patientIDlist.Count==0)
			{
				string approvedYesPatientquery = "select patientID from patient where isApproved=1";
				returnSQL = executeSELECTSQL(dbconnectionstring, approvedYesPatientquery);
				patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
				patientIDlist.AddRange(patientID);
			}

			//else if(approvedYes && createdDate.ToString().Equals("001/1/1 12:00:00 AM") && patientIDlist.Count != 0)
			else if (approvedYes && patientIDlist.Count != 0)
			{
				List<int> approvedPatient = new List<int>();
				for (int i =0;i<patientIDlist.Count();i++)
				{
					string approvedYesPatientquery = "select patientID from patient where isApproved=1 and patientID="+patientIDlist[i];
					returnSQL = executeSELECTSQL(dbconnectionstring, approvedYesPatientquery);
					patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
					approvedPatient.AddRange(patientID);
				}
				patientIDlist.Clear();
				patientIDlist.AddRange(approvedPatient);
			}
			//if (approvedNo && createdDate.ToString().Equals("001/1/1 12:00:00 AM") && patientIDlist.Count == 0)
			if (approvedNo && patientIDlist.Count == 0)
			{
				string approvedNoPatientquery = "select patientID from patient where isApproved=0";
				returnSQL = executeSELECTSQL(dbconnectionstring, approvedNoPatientquery);
				patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
				patientIDlist.AddRange(patientID);
			}
			//else if (approvedNo && createdDate.ToString().Equals("001/1/1 12:00:00 AM") && patientIDlist.Count != 0)
			else if (approvedNo && patientIDlist.Count != 0)
			{
				List<int> NotapprovedPatient = new List<int>();
				for (int i = 0; i < patientIDlist.Count(); i++)
				{
					string approvedYesPatientquery = "select patientID from patient where isApproved=0 and patientID=" + patientIDlist[i];
					returnSQL = executeSELECTSQL(dbconnectionstring, approvedYesPatientquery);
					patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
					NotapprovedPatient.AddRange(patientID);
				}
				patientIDlist.Clear();
				patientIDlist.AddRange(NotapprovedPatient);
			}

			//if (deletedYes && createdDate.ToString().Equals("001/1/1 12:00:00 AM") && patientIDlist.Count == 0)
			if (deletedYes && patientIDlist.Count == 0)
			{
				string deletedNoPatientquery = "select patientID from patient where isDeleted=1";
				returnSQL = executeSELECTSQL(dbconnectionstring, deletedNoPatientquery);
				patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
				patientIDlist.AddRange(patientID);
			}
			//else if (deletedYes && createdDate.ToString().Equals("001/1/1 12:00:00 AM") && patientIDlist.Count != 0)
			else if (deletedYes && patientIDlist.Count != 0)
			{
				List<int> deletedPatient = new List<int>();
				for (int i = 0; i < patientIDlist.Count(); i++)
				{
					string deletedNoPatientquery = "select patientID from patient where isDeleted=1 and patientID=" + patientIDlist[i];
					returnSQL = executeSELECTSQL(dbconnectionstring, deletedNoPatientquery);
					patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
					deletedPatient.AddRange(patientID);
				}
				patientIDlist.Clear();
				patientIDlist.AddRange(deletedPatient);
			}

			//if (deletedNo && createdDate.ToString().Equals("001/1/1 12:00:00 AM") && patientIDlist.Count == 0)
			if (deletedNo  && patientIDlist.Count == 0)
			{
				string deletedYesPatientquery = "select patientID from patient where isDeleted=0";
				returnSQL = executeSELECTSQL(dbconnectionstring, deletedYesPatientquery);
				patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
				patientIDlist.AddRange(patientID);
			}

			//else if (deletedNo && createdDate.ToString().Equals("001/1/1 12:00:00 AM") && patientIDlist.Count != 0)
			else if (deletedNo && patientIDlist.Count != 0)
			{
				List<int> deletedPatient = new List<int>();
				for (int i = 0; i < patientIDlist.Count(); i++)
				{
					string deletedNoPatientquery = "select patientID from patient where isDeleted=0 and patientID=" + patientIDlist[i];
					returnSQL = executeSELECTSQL(dbconnectionstring, deletedNoPatientquery);
					patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
					deletedPatient.AddRange(patientID);
				}
				patientIDlist.Clear();
				patientIDlist.AddRange(deletedPatient);
			}

			//if (!createdDate.ToString().Equals("001/1/1 12:00:00 AM"))
			//{
			//	char comparesymbol = '=';
			//	if (model.compare.Equals("Later than"))
			//		comparesymbol = '>';
			//	else if (model.compare.Equals("Earlier than"))
			//		comparesymbol = '<';

			//	if (approvedYes)
			//	{
			//		string approvedYesPatientquery = "select patientID from patient where isApproved=1 and createDateTime " + comparesymbol + " '" + createdDate + "'";
			//		returnSQL = executeSELECTSQL(dbconnectionstring, approvedYesPatientquery);
			//		patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
			//		patientIDlist.AddRange(patientID);
			//	}
			//	if (approvedNo)
			//	{
			//		string approvedNoPatientquery = "select patientID from patient where isApproved=0 and createDateTime " + comparesymbol + " '" + createdDate + "'";
			//		returnSQL = executeSELECTSQL(dbconnectionstring, approvedNoPatientquery);
			//		patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
			//		patientIDlist.AddRange(patientID);
			//	}
			//	if (deletedYes)
			//	{
			//		string deletedNoPatientquery = "select patientID from patient where isDeleted=1 and createDateTime " + comparesymbol + " '" + createdDate + "'";
			//		returnSQL = executeSELECTSQL(dbconnectionstring, deletedNoPatientquery);
			//		patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
			//		patientIDlist.AddRange(patientID);
			//	}
			//	if (deletedNo)
			//	{
			//		string deletedYesPatientquery = "select patientID from patient where isDeleted=0 and createDateTime " + comparesymbol + " '" + createdDate + "'";
			//		returnSQL = executeSELECTSQL(dbconnectionstring, deletedYesPatientquery);
			//		patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
			//		patientIDlist.AddRange(patientID);
			//	}
			//	else if(!approvedYes && !approvedNo && !deletedNo && !deletedYes)
			//	{
			//		string createDatePatientquery = "select patientID from patient where createDateTime " + comparesymbol + " '" + createdDate + "'";
			//		returnSQL = executeSELECTSQL(dbconnectionstring, createDatePatientquery);
			//		patientID = returnSQL.AsEnumerable().Select(r => r.Field<int>("patientID")).ToList();
			//		patientIDlist.AddRange(patientID);
			//	}
			//}


			patientIDlist = patientIDlist.Distinct().ToList();
			for (int i = 0; i < patientIDlist.Count(); i++)
			{
				allPatientIDResult.Add(executeSELECTSQL(dbconnectionstring, "Select * from patient where patientID=" + patientIDlist[i]));
			}
			return allPatientIDResult;
		}

		[Authorize(Roles = RoleName.isAdmin)]
		[HttpGet]
		public ActionResult filter(ArchiveSQLViewModel model)
		{
			model = displayfilterView(model);
			List<dtPatientObj> filterPageResult = new List<dtPatientObj>();
			for(int i=0; i<model.listGetPatientIDandYear.Count(); i++)
			{
				if(model.listGetPatientIDandYear[i].checkboxValue == true)
				{
					dtPatientObj dtPatientObj = new dtPatientObj();
					dtPatientObj = searchEverythingRegardingThisPatient(model, model.listGetPatientIDandYear[i].name, model.listGetPatientIDandYear[i].patientID);
					filterPageResult.Add(dtPatientObj);
				}
			}
			model.filterPageResult = filterPageResult;
			return View(model);
		}

		[Authorize(Roles = RoleName.isAdmin)]
		[HttpPost]
		public ActionResult Filter(ArchiveSQLViewModel model)
		{
			//Each object/item in filterPageResult = 1 Patient
			List<dtPatientObj> filterPageResult = new List<dtPatientObj>();
			for (int i = 0; i < model.listGetPatientIDandYear.Count(); i++)
			{
					dtPatientObj dtPatientObj = new dtPatientObj();
					dtPatientObj = searchEverythingRegardingThisPatient(model, model.listGetPatientIDandYear[i].name, model.listGetPatientIDandYear[i].patientID);
					filterPageResult.Add(dtPatientObj);
			}
			model = displayfilterView(model);
			model.filterPageResult = filterPageResult;

			if (Request.Form["submitbutton1"] != null)
			{
				shortcutMethod.printf("submitbutton1");
			}
			else if (Request.Form["submitButton2"] != null)
			{
				shortcutMethod.printf("submitbutton2");
				List<excelZip> dsList = new List<excelZip>();
				
				for(int i=0; i< filterPageResult.Count();i++)
				{
					excelZip eachPatientContent = new excelZip();
					DataSet ds = new DataSet();
					DataTable dt = new DataTable();
					for(int j=0; j<filterPageResult[i].patientListviaPatientID.Count;j++)
					{
						ds.Tables.Add(filterPageResult[i].patientListviaPatientID[j]);
					}
					for (int k = 0; k < filterPageResult[i].patientListviaAllocationID.Count; k++)
					{
						ds.Tables.Add(filterPageResult[i].patientListviaAllocationID[k]);
					}
					for (int l = 0; l < filterPageResult[i].patientListviaSocialHistoryID.Count; l++)
					{
						ds.Tables.Add(filterPageResult[i].patientListviaSocialHistoryID[l]);
					}
					eachPatientContent.datacontent = ds;
					eachPatientContent.filename = filterPageResult[i].patientfirstname + "_" + filterPageResult[i].patientlastname + "_" + filterPageResult[i].nric + "_" + filterPageResult[i].dbname;
					dsList.Add(eachPatientContent);
				}
				createExcelthenPassZip(dsList);
				shortcutMethod.printf("End of submitbutton2");
			}

			return View(model);
		}

		[Authorize(Roles = RoleName.isAdmin)]
		public dtPatientObj searchEverythingRegardingThisPatient(ArchiveSQLViewModel model, string DBname, string patientID)
		{
			shortcutMethod.printf("Method: searchEverythingRegardingThisPatient");
			dtPatientObj dtpObj = new dtPatientObj();
			string conn_string = getSQLconnection_string(DBname);
			List<string> patientIDTablelist = getAllTableNameContainingAField(DBname, "patientID");
			List<string> patientAllocationIDTablelist = getAllTableNameContainingAField(DBname, "patientAllocationID");
			List<string> socialHistoryIDTableList = getAllTableNameContainingAField(DBname, "socialHistoryID");
			
			List<DataTable> patientListviaPatientID = new List<DataTable>();
			List<DataTable> patientListviaAllocationID = new List<DataTable>();
			List<DataTable> patientListviaSocialHistoryID = new List<DataTable>();
			List<string> dtpObjtablename1 = new List<string>(); List<string> dtpObjtablename2 = new List<string>(); List<string> dtpObjtablename3 = new List<string>();

			//Search patientAllocationID from patientID && socialHistory;
			string querysocialhistory = "select socialHistoryID from socialHistory where patientID =" + Int32.Parse(patientID);
			string queryAllocation = "select patientAllocationID from patientAllocation where patientID = " + Int32.Parse(patientID);
			int patientAllocationID = executeSELECTSQL(conn_string, queryAllocation).AsEnumerable().Select(r => r.Field<int>("patientAllocationID")).DefaultIfEmpty(-1).Single();
			int socialHistroyID = executeSELECTSQL(conn_string, querysocialhistory).AsEnumerable().Select(r => r.Field<int>("socialHistoryID")).DefaultIfEmpty(-1).Single();
			shortcutMethod.printf(patientAllocationIDTablelist.Count() + " and " + socialHistoryIDTableList.Count());
			dtpObj.patientID = Int32.Parse(patientID); dtpObj.SocialHistoryID = Int32.Parse(patientID); dtpObj.patientAllocationID = Int32.Parse(patientID);
			for (int i = 0; i < model.allTablename.Count(); i++)
			{
				string addOnQuery = "";
				if (model.deletedYes)
					addOnQuery = addOnQuery + " and isDeleted=1";
				if (model.deletedNo)
					addOnQuery = addOnQuery + " and isDeleted=0";

				if (model.allTablename[i].checktablename)
				{
					shortcutMethod.printf("Table name:" + model.allTablename[i].tablename + "PatientID:" + patientID);
					if (patientIDTablelist.Any(model.allTablename[i].tablename.Contains))
					{
						dtpObjtablename1.Add(model.allTablename[i].tablename);
						shortcutMethod.printf("filterPage: Add to patientIDlist");
						if(model.allTablename[i].tablename.Equals("log"))
						{
							if (model.approvedYes)
								addOnQuery = addOnQuery + " and approved=1";
							if (model.approvedNo)
								addOnQuery = addOnQuery + " and approved=0";
						}
						else
						{
							if (model.approvedYes)
								addOnQuery = addOnQuery + " and isApproved=1";
							if (model.approvedNo)
								addOnQuery = addOnQuery + " and isApproved=0";
						}
						string query = "select * from " + model.allTablename[i].tablename + " where patientID=" + patientID +" " + addOnQuery;
						DataTable dt = executeSELECTSQL(conn_string, query);
						dt.TableName = model.allTablename[i].tablename;
						if (dt!=null)
							if (dt.Rows.Count > 0)
								patientListviaPatientID.Add(dt);
							else
								patientListviaPatientID.Add(dt);
					}
					else if (patientAllocationIDTablelist.Any(model.allTablename[i].tablename.Contains) && patientAllocationID != -1)
					{
						dtpObjtablename2.Add(model.allTablename[i].tablename);
						shortcutMethod.printf("filterPage: Add to patientListviaAllocationID");
						string query = "select * from " + model.allTablename[i].tablename + " where patientAllocationID=" + patientAllocationID + " " + addOnQuery;
						DataTable dt = executeSELECTSQL(conn_string, query);
						dt.TableName = model.allTablename[i].tablename;
						if (dt != null)
							if (dt.Rows.Count > 0)
								patientListviaAllocationID.Add(dt);
							else
								patientListviaAllocationID.Add(dt);
					}
					else if (socialHistoryIDTableList.Any(model.allTablename[i].tablename.Contains) && socialHistroyID != -1)
					{
						dtpObjtablename3.Add(model.allTablename[i].tablename);
						shortcutMethod.printf("filterPage: Add to patientListviaSocialHistoryID");
						string query = "select * from " + model.allTablename[i].tablename + " where socialHistoryID=" + socialHistroyID + " " + addOnQuery;
						DataTable dt = executeSELECTSQL(conn_string, query);
						dt.TableName = model.allTablename[i].tablename;
						if (dt != null)
							if (dt.Rows.Count > 0)
								patientListviaSocialHistoryID.Add(dt);
							else
								patientListviaSocialHistoryID.Add(dt);
					}
				}
			}
			dtpObj.patientListviaAllocationID = patientListviaAllocationID;
			dtpObj.patientListviaPatientID = patientListviaPatientID;
			dtpObj.patientListviaSocialHistoryID = patientListviaSocialHistoryID;
			dtpObj.patientListviaAllocationIDtablename = dtpObjtablename2;
			dtpObj.patientListviaSocialHistoryIDtablename = dtpObjtablename3;
			dtpObj.patientListviaPatientIDtablename = dtpObjtablename1;
			dtpObj.patientID = Int32.Parse(patientID); 
			dtpObj.nric = executeSELECTSQL(conn_string, "Select nric from patient where patientID="+patientID).AsEnumerable().Select(r => r.Field<string>("nric")).Single();
			dtpObj.dbname = DBname;
			dtpObj.patientfirstname = executeSELECTSQL(conn_string, "Select firstName from patient where patientID=" + patientID).AsEnumerable().Select(r => r.Field<string>("firstName")).Single();
			dtpObj.patientlastname = executeSELECTSQL(conn_string, "Select lastName from patient where patientID=" + patientID).AsEnumerable().Select(r => r.Field<string>("lastName")).Single();
			return dtpObj;
		}

		//Find all table from a database
		[Authorize(Roles = RoleName.isAdmin)]
		public List<string> getAllTablenameSQL(string dbname)
		{
			List<string> tableList = new List<string>();
			string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = '"+ currentdatabasename + "' AND TABLE_NAME !='__MigrationHistory' AND TABLE_NAME !='activityAvailability' AND TABLE_NAME not like '%list%' AND TABLE_NAME not in ('AspNetRoles','AspNetUserClaims','AspNetUserLogins','AspNetUserRoles','AspNetUsers','user', 'userType','logCategory','patientSpecInfo','gameSpecInfo','albumCategory','centreActivity','performanceMetricName','performanceMetricOrder', 'category','dementiaType','gameAssignedDementia','gameCategory') ORDER BY TABLE_NAME ASC";
			string conn_string = getSQLconnection_string(dbname);
			tableList = executeSELECTSQL(conn_string, query).AsEnumerable().Select(r => r.Field<string>("TABLE_NAME")).ToList();
			return tableList;
		}

		[Authorize(Roles = RoleName.isAdmin)]
		public List<string> getAllTableNameContainingAField(string dbname, string field)
		{
			List<string> tableList = new List<string>();
			string query = "SELECT      c.name  AS 'ColumnName' ,t.name AS 'TABLE_NAME' FROM        sys.columns c JOIN        sys.tables  t   ON c.object_id = t.object_id WHERE       c.name LIKE '%"+ field + "%' ORDER BY    Table_Name ,ColumnName;";
			string conn_string = getSQLconnection_string(dbname);
			tableList = executeSELECTSQL(conn_string, query).AsEnumerable().Select(r => r.Field<string>("TABLE_NAME")).ToList();
			return tableList;
		}

		[Authorize(Roles = RoleName.isAdmin)]
		public List<databaseObj> getAllTableNameFieldNameOfADatabase(string dbname)
		{
			List<string> tableList = new List<string>();
			tableList = getAllTablenameSQL(dbname);
			List<databaseObj> dbObj = new List<databaseObj>();
			for (int i=0; i<tableList.Count(); i++)
			{
				List<string> fieldList = new List<string>();
				databaseObj databaseObj = new databaseObj();
				databaseObj.databaseName = dbname;
				databaseObj.databaseTableName = tableList[i];
				string query = "select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '"+tableList[i]+ "'  AND TABLE_NAME not like '%list%' AND TABLE_NAME not in ('__MigrationHistory','activityAvailability','activityAvailability','AspNetRoles','AspNetUserClaims','AspNetUserLogins','AspNetUserRoles','AspNetUsers','user', 'userType','logCategory','patientSpecInfo','gameSpecInfo','albumCategory','centreActivity','performanceMetricName','performanceMetricOrder', 'category','dementiaType','gameAssignedDementia','gameCategory')";
				string conn_string = getSQLconnection_string(dbname);
				fieldList = executeSELECTSQL(conn_string, query).AsEnumerable().Select(r => r.Field<string>("COLUMN_NAME")).ToList();
				databaseObj.databaseFieldName = fieldList;
				if(fieldList.Count() > 0)
					dbObj.Add(databaseObj);
			}
			return dbObj;
		}

		// Only Admin can access this method
		[Authorize(Roles = RoleName.isAdmin)]
		public void createExcelthenPassZip(List<excelZip> dsList)
		{
			DateTime DateTimeNow = DateTime.Now;
			// Initialize Zip file library
			Ionic.Zip.ZipFile zipFile = new Ionic.Zip.ZipFile();

			for (int i = 0; i < dsList.Count(); i++)
			{
				using (XLWorkbook wb = new XLWorkbook())
				{
					// Add patient details to Excel worksheet
					wb.Worksheets.Add(dsList[i].datacontent);
					wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
					wb.Style.Font.Bold = true;
					// Set password for zip file
					zipFile.Password = "123";//currentdatabasepass;
					// Add Excel Worksheet to zip file
					zipFile.AddEntry(""+dsList[i].filename + ".xlsx", (name, stream) => wb.SaveAs(stream));
				}
			}
			Response.ClearContent();
			Response.ClearHeaders();
			Response.ContentType = "application/zip";
			Response.AppendHeader("content-disposition", "attachment; filename=Report_"+ DateTimeNow+".zip");
			zipFile.Save(Response.OutputStream);
			Response.End();
		}

	}
}