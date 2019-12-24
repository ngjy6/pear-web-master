using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.App_Code
{
	public class CentreActivity
	{
		private int centreActivityID;
		private string activityTitle;
		private string activityDesc;
		private int interval;
		private int minDuration;
		private int maxDuration;
		private int minPeopleReq;
		private int isCompulsory;
		private int isFixed;
		private int isGroup;
		//private DateTime timeStart;
		//private DateTime timeEnd;

		public CentreActivity()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public CentreActivity(int centreActivityID, string activityTitle, string activityDesc, int interval, int minDuration, int maxDuration, int minPeopleReq)
		{
			this.centreActivityID = centreActivityID;
			this.activityTitle = activityTitle;
			this.activityDesc = activityDesc;
			this.interval = interval;
			this.minDuration = minDuration;
			this.maxDuration = maxDuration;
			this.minPeopleReq = minPeopleReq;
		}
		public CentreActivity(int centreActivityID, string activityTitle, string activityDesc, int interval, int minDuration, int maxDuration, int minPeopleReq, int isCompulsory, int isFixed, int isGroup)
		{
			this.centreActivityID = centreActivityID;
			this.activityTitle = activityTitle;
			this.activityDesc = activityDesc;
			this.interval = interval;
			this.minDuration = minDuration;
			this.maxDuration = maxDuration;
			this.minPeopleReq = minPeopleReq;
			this.isCompulsory = isCompulsory;
			this.isFixed = isFixed;
			this.isGroup = isGroup;
		}
		public int getCentreActivityID()
		{
			return centreActivityID;
		}
		public void setCentreActivityID(int centreActivityID)
		{
			this.centreActivityID = centreActivityID;
		}
		public string getActivityTitle()
		{
			return activityTitle;
		}
		public void setActivityTitle(string activityTitle)
		{
			this.activityTitle = activityTitle;
		}
		public string getActivityDesc()
		{
			return activityDesc;
		}
		public void setActivityDesc(string activityDesc)
		{
			this.activityDesc = activityDesc;
		}
		public int getInterval()
		{
			return interval;
		}
		public void setInterval(int interval)
		{
			this.interval = interval;
		}
		public int getMinDuration()
		{
			return minDuration;
		}
		public void setMinDuration(int minDuration)
		{
			this.minDuration = minDuration;
		}
		public int getMaxDuration()
		{
			return maxDuration;
		}
		public void setMaxDuration(int maxDuration)
		{
			this.maxDuration = maxDuration;
		}
		public int getMinPeopleReq()
		{
			return minPeopleReq;
		}
		public void setMinPeopleReq(int minPeopleReq)
		{
			this.minPeopleReq = minPeopleReq;
		}
		public int getIsCompulsory()
		{
			return isCompulsory;
		}
		public void setIsCompulsory(int isCompulsory)
		{
			this.isCompulsory = isCompulsory;
		}
		public int getIsFixed()
		{
			return isFixed;
		}
		public void setIsFixed(int isFixed)
		{
			this.isFixed = isFixed;
		}
		public int getIsGroup()
		{
			return isGroup;
		}
		public void setIsGroup(int isGroup)
		{
			this.isGroup = isGroup;
		}
	}
}