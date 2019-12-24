using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.App_Code
{
	public class ActivityAvaliability
	{
		private String day;
		private DateTime timeStart;
		private DateTime timeEnd;
		public ActivityAvaliability()
		{
		}
		public ActivityAvaliability(String day, DateTime timeStart, DateTime timeEnd)
		{
			this.day = day;
			this.timeStart = timeStart;
			this.timeEnd = timeEnd;
		}
		public String getDay()
		{
			return day;
		}
		public void setDay(String day)
		{
			this.day = day;
		}
		public DateTime getTimeStart()
		{
			return timeStart;
		}
		public void setTimeStart(DateTime timeStart)
		{
			this.timeStart = timeStart;
		}
		public DateTime getTimeEnd()
		{
			return timeEnd;
		}
		public void setTimeEnd(DateTime timeEnd)
		{
			this.timeEnd = timeEnd;
		}
	}
}