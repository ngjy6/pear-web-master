using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.App_Code
{
	public class Routine
	{
		private int routineID;
		private int patientID; //new 
		private string eventName;
		private DateTime startDate;
		private DateTime endDate;
		private string startTime;
		private string endTime;
		private int everyNum;
		private string everyLabel;

		public Routine()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public Routine(int routineID, int patientID, string eventName, DateTime startDate, DateTime endDate, string startTime, string endTime, int everyNum, string everyLabel)
		{
			this.routineID = routineID;
			this.patientID = patientID;
			this.eventName = eventName;
			this.startDate = startDate;
			this.endDate = endDate;
			this.startTime = startTime;
			this.endTime = endTime;
			this.everyNum = everyNum;
			this.everyLabel = everyLabel;
		}
		public int getRoutineID()
		{
			return routineID;
		}
		public void setRoutineID(int routineID)
		{
			this.routineID = routineID;
		}
		public int getPatientID()
		{
			return patientID;
		}
		public void setPatientID(int patientID)
		{
			this.patientID = patientID;
		}
		public string getEventName()
		{
			return eventName;
		}
		public void setEventName(string eventName)
		{
			this.eventName = eventName;
		}
		public DateTime getStartDate()
		{
			return startDate;
		}
		public void setStartDate(DateTime startDate)
		{
			this.startDate = startDate;
		}
		public DateTime getEndDate()
		{
			return endDate;
		}
		public void setEndDate(DateTime endDate)
		{
			this.endDate = endDate;
		}
		public string getStartTime()
		{
			return startTime;
		}
		public void setStartTime(string startTime)
		{
			this.startTime = startTime;
		}
		public string getEndTime()
		{
			return endTime;
		}
		public void setEndTime(string endTime)
		{
			this.endTime = endTime;
		}
		public int getEveryNum()
		{
			return everyNum;
		}
		public void setEveryNum(int everyNum)
		{
			this.everyNum = everyNum;
		}
		public string getEveryLabel()
		{
			return everyLabel;
		}
		public void setEveryLabel(string everyLabel)
		{
			this.everyLabel = everyLabel;
		}
	}
}