using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.App_Code
{
	public class Schedule
	{
		private String scheduleDesc;
		private String timeStart;
		private String timeEnd;
		private DateTime dateStart;
		private DateTime dateEnd;
		private int patientAllocationID;
		private int centreActivityID;
		private int routineID; // changed to string
		private int isClash;
		private int duration;

		//to be removed, just to display the title
		private String title;
		private String desc;
		public Schedule()
		{

		}

		//This constructor is for displaying the schedule activity's content (title, desc)
		public Schedule(String scheduleDesc, String timeStart, String timeEnd, DateTime dateStart, DateTime dateEnd, int patientAllocationID, int centreActivityID, int routineID, int isClash, int duration, String title, String desc)
		{
			this.scheduleDesc = scheduleDesc;
			this.timeStart = timeStart;
			this.timeEnd = timeEnd;
			this.dateStart = dateStart;
			this.dateEnd = dateEnd;
			this.patientAllocationID = patientAllocationID;
			this.centreActivityID = centreActivityID;
			this.routineID = routineID;
			this.isClash = isClash;
			this.duration = duration;
			this.title = title;
			this.desc = desc;
		}

		public Schedule(String scheduleDesc, String timeStart, String timeEnd, DateTime dateStart, DateTime dateEnd, int patientAllocationID, int centreActivityID, int routineID, int isClash, int duration)
		{
			this.scheduleDesc = scheduleDesc;
			this.timeStart = timeStart;
			this.timeEnd = timeEnd;
			this.dateStart = dateStart;
			this.dateEnd = dateEnd;
			this.patientAllocationID = patientAllocationID;
			this.centreActivityID = centreActivityID;
			this.routineID = routineID;
			this.isClash = isClash;
			this.duration = duration;
		}
		public String getTitle()
		{
			return title;
		}
		public void setTitle(String title)
		{
			this.title = title;
		}

		public String getDesc()
		{
			return desc;
		}
		public void setDesc(String desc)
		{
			this.desc = desc;
		}
		public int getDuration()
		{
			return duration;
		}
		public void setDuration(int duration)
		{
			this.duration = duration;
		}
		public String getScheduleDesc()
		{
			return scheduleDesc;
		}
		public void setScheduleDesc(String scheduleDesc)
		{
			this.scheduleDesc = scheduleDesc;
		}

		public String getTimeStart()
		{
			return timeStart;
		}
		public void setTimeStart(String timeStart)
		{
			this.timeStart = timeStart;
		}

		public String getTimeEnd()
		{
			return timeEnd;
		}
		public void setTimeEnd(String timeEnd)
		{
			this.timeEnd = timeEnd;
		}

		public DateTime getDateStart()
		{
			return dateStart;
		}
		public void setDateStart(DateTime dateStart)
		{
			this.dateStart = dateStart;
		}

		public DateTime getDateEnd()
		{
			return dateEnd;
		}
		public void setDateEnd(DateTime dateEnd)
		{
			this.dateEnd = dateEnd;
		}

		public int getPatientAllocationID()
		{
			return patientAllocationID;
		}
		public void setPatientAllocationID(int patientAllocationID)
		{
			this.patientAllocationID = patientAllocationID;
		}
		public int getCentreActivityID()
		{
			return centreActivityID;
		}
		public void setCentreActivityID(int centreActivityID)
		{
			this.centreActivityID = centreActivityID;
		}

		public int getRoutineID()
		{
			return routineID;
		}
		public void setRoutineID(int routineID)
		{
			this.routineID = routineID;
		}

		public int getIsClash()
		{
			return isClash;
		}
		public void setIsClash(int isClash)
		{
			this.isClash = isClash;
		}
	}
}