using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.App_Code
{
	public class Prescription
	{
		private int prescriptionID;
		private string drugName;
		private string dosage;
		private int frequency;

		private DateTime startDate;
		private DateTime endDate;
		private int beforeMeal;
		private int afterMeal;

		private string notes;
		private int patientID;
		public Prescription()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public Prescription(int prescriptionID, string drugName, string dosage, int frequency, DateTime startDate, DateTime endDate, int beforeMeal, int afterMeal, string notes, int patientID)
		{
			this.prescriptionID = prescriptionID;
			this.drugName = drugName;
			this.dosage = dosage;
			this.frequency = frequency;
			this.startDate = startDate;
			this.endDate = endDate;
			this.beforeMeal = beforeMeal;
			this.afterMeal = afterMeal;
			this.notes = notes;
			this.patientID = patientID;
		}
		public int getPresciptionID()
		{
			return prescriptionID;
		}
		public void setPrescriptionID(int prescriptionID)
		{
			this.prescriptionID = prescriptionID;
		}
		public string getDrugName()
		{
			return drugName;
		}
		public void setDrugName(string drugName)
		{
			this.drugName = drugName;
		}
		public string getDosage()
		{
			return dosage;
		}
		public void setDosage(string dosage)
		{
			this.dosage = dosage;
		}
		public int getFrequency()
		{
			return frequency;
		}
		public void setFrequency(int frequency)
		{
			this.frequency = frequency;
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
		public int getBeforeMeal()
		{
			return beforeMeal;
		}
		public void setBeforeMeal(int beforeMeal)
		{
			this.beforeMeal = beforeMeal;
		}

		public int getAfterMeal()
		{
			return afterMeal;
		}
		public void setAfterMeal(int afterMeal)
		{
			this.afterMeal = afterMeal;
		}

		public string getNotes()
		{
			return notes;
		}
		public void setNotes(string notes)
		{
			this.notes = notes;
		}
		public int getPatientID()
		{
			return patientID;
		}
		public void setPatientID(int patientID)
		{
			this.patientID = patientID;
		}
	}
}