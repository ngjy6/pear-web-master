using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace NTU_FYP_REBUILD_17.Controllers.Synchronization
{
    public class SearchMethod
    {
        private ApplicationDbContext _context;
        public SearchMethod()
        {
            _context = new ApplicationDbContext();
        }

        public List<SearchResultViewModel> searchPatientName(List<SearchResultViewModel> searchResult, string searchWords)
        {
            DateTime date = DateTime.Today;

            List<Patient> patientList = _context.Patients.Where(x => (x.isApproved == 1 && x.isDeleted != 1)).ToList();
            foreach (var patient in patientList)
            {
                string patientName = (patient.firstName + patient.lastName).ToLower();
                if (patientName.Contains(searchWords.ToLower().Replace(" ", "")))
                {
                    patientName = patient.firstName + " " + patient.lastName;
                    string status = "Active";

                    if (DateTime.Compare(patient.startDate, date) > 0)
                        status = "Not yet admitted";
                    else if (patient.endDate != null && DateTime.Compare((DateTime)patient.endDate, date) < 0)
                        status = "Terminated";
                    else if (patient.inactiveDate != null && DateTime.Compare((DateTime)patient.inactiveDate, date) <= 0)
                        status = "Inactive";

                    searchResult.Add(new SearchResultViewModel
                    {
                        headerName = patientName,
                        name = patient.firstName + " " + patient.lastName,
                        status = status,
                        type = "Patient name",
                        href = "/Doctor/ViewPatient?patientID=" + patient.patientID,
                        message = " has the contained words of " + searchWords
                    });
                }
            }
            searchResult = searchResult.OrderBy(x => x.name).ThenBy(x => x.headerName).ToList();
            return searchResult;
        }

        public List<SearchResultViewModel> searchDementiaName(List<SearchResultViewModel> searchResult, string searchWords)
        {
            List<DementiaType> list = _context.DementiaTypes.Where(x => (x.isApproved == 1 && x.isDeleted != 1)).ToList();
            DateTime date = DateTime.Today;

            foreach (var item in list)
            {
                if (item.dementiaType.ToLower().Replace(" ", "").Contains(searchWords.ToLower().Replace(" ", "")))
                {
                    List<PatientAssignedDementia> patientAssignedDementia = _context.PatientAssignedDementias.Where(x => (x.dementiaID == item.dementiaID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    foreach (var patient in patientAssignedDementia)
                    {
                        if (patient.PatientAllocation.Patient.isApproved == 0 || patient.PatientAllocation.Patient.isDeleted == 1)
                            continue;

                        string patientName = patient.PatientAllocation.Patient.firstName + " " + patient.PatientAllocation.Patient.lastName;
                        string status = "Active";

                        if (DateTime.Compare(patient.PatientAllocation.Patient.startDate, date) > 0)
                            status = "Not yet admitted";
                        else if (patient.PatientAllocation.Patient.endDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.endDate, date) < 0)
                            status = "Terminated";
                        else if (patient.PatientAllocation.Patient.inactiveDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.inactiveDate, date) <= 0)
                            status = "Inactive";

                        searchResult.Add(new SearchResultViewModel
                        {
                            headerName = patientName,
                            name = item.dementiaType,
                            status = status,
                            type = "Name of dementia",
                            href = "/Doctor/ViewPatient?patientID=" + patient.PatientAllocation.patientID,
                            message = " has the contained words of '" + searchWords + "'"
                        });
                    }
                }
            }
            searchResult = searchResult.OrderBy(x => x.name).ThenBy(x => x.headerName).ToList();
            return searchResult;
        }

        public List<SearchResultViewModel> searchDrugName(List<SearchResultViewModel> searchResult, string searchWords)
        {
            List<List_Prescription> list = _context.ListPrescriptions.Where(x => (x.isDeleted != 1)).ToList();
            DateTime date = DateTime.Today;

            foreach (var item in list)
            {
                if (item.value.ToLower().Replace(" ", "").Contains(searchWords.ToLower().Replace(" ", "")))
                {
                    List<string> result = new List<string>();
                    bool found = false;
                    List<Prescription> prescription = _context.Prescriptions.Where(x => (x.drugNameID == item.list_prescriptionID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    foreach (var patient in prescription)
                    {
                        string patientName = patient.PatientAllocation.Patient.firstName + " " + patient.PatientAllocation.Patient.lastName;
                        string status = "Active";

                        if (DateTime.Compare(patient.PatientAllocation.Patient.startDate, date) > 0)
                            status = "Not yet admitted";
                        else if (patient.PatientAllocation.Patient.endDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.endDate, date) < 0)
                            status = "Terminated";
                        else if (patient.PatientAllocation.Patient.inactiveDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.inactiveDate, date) <= 0)
                            status = "Inactive";

                        if (result.Count == 0)
                        {
                            searchResult.Add(new SearchResultViewModel
                            {
                                headerName = patientName,
                                name = item.value,
                                status = status,
                                type = "Drug name",
                                href = "/Doctor/ViewPatient?patientID=" + patient.PatientAllocation.patientID,
                                message = " has the contained words of '" + searchWords + "'"
                            });

                            result.Add(patientName);
                        }
                        else
                        {
                            for (int i = 0; i < result.Count; i++)
                            {
                                if (result[i] == patientName)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                searchResult.Add(new SearchResultViewModel
                                {
                                    headerName = patientName,
                                    name = item.value,
                                    status = status,
                                    type = "Drug name",
                                    href = "/Doctor/ViewPatient?patientID=" + patient.PatientAllocation.patientID,
                                    message = " has the contained words of '" + searchWords + "'"
                                });
                                result.Add(patientName);
                            }
                            else
                                found = false;
                        }
                    }
                }
            }
            searchResult = searchResult.OrderBy(x => x.name).ThenBy(x => x.headerName).ToList();
            return searchResult;
        }

        public List<SearchResultViewModel> searchAllergyName(List<SearchResultViewModel> searchResult, string searchWords)
        {
            List<List_Allergy> list = _context.ListAllergy.Where(x => (x.isDeleted != 1)).ToList();
            DateTime date = DateTime.Today;

            foreach (var item in list)
            {
                if (item.value.ToLower().Replace(" ", "").Contains(searchWords.ToLower().Replace(" ", "")))
                {
                    List<string> result = new List<string>();
                    bool found = false;
                    List<Allergy> allergy = _context.Allergies.Where(x => (x.allergyListID == item.list_allergyID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    foreach (var patient in allergy)
                    {
                        string patientName = patient.PatientAllocation.Patient.firstName + " " + patient.PatientAllocation.Patient.lastName;
                        string status = "Active";

                        if (DateTime.Compare(patient.PatientAllocation.Patient.startDate, date) > 0)
                            status = "Not yet admitted";
                        else if (patient.PatientAllocation.Patient.endDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.endDate, date) < 0)
                            status = "Terminated";
                        else if (patient.PatientAllocation.Patient.inactiveDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.inactiveDate, date) <= 0)
                            status = "Inactive";

                        if (result.Count == 0)
                        {
                            searchResult.Add(new SearchResultViewModel
                            {
                                headerName = patientName,
                                name = item.value,
                                status = status,
                                type = "Drug name",
                                href = "/Doctor/ViewPatient?patientID=" + patient.PatientAllocation.patientID,
                                message = " has the contained words of '" + searchWords + "'"
                            });

                            result.Add(patientName);
                        }
                        else
                        {
                            for (int i = 0; i < result.Count; i++)
                            {
                                if (result[i] == patientName)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                searchResult.Add(new SearchResultViewModel
                                {
                                    headerName = patientName,
                                    name = item.value,
                                    status = status,
                                    type = "Allergy item name",
                                    href = "/Doctor/ViewPatient?patientID=" + patient.PatientAllocation.patientID,
                                    message = " has the contained words of '" + searchWords + "'"
                                });
                                result.Add(patientName);
                            }
                            else
                                found = false;
                        }
                    }
                }
            }
            searchResult = searchResult.OrderBy(x => x.name).ThenBy(x => x.headerName).ToList();
            return searchResult;
        }

        public List<SearchResultViewModel> searchMobilityName(List<SearchResultViewModel> searchResult, string searchWords)
        {
            List<List_Mobility> list = _context.ListMobility.Where(x => (x.isDeleted != 1)).ToList();
            DateTime date = DateTime.Today;

            foreach (var item in list)
            {
                if (item.value.ToLower().Replace(" ", "").Contains(searchWords.ToLower().Replace(" ", "")))
                {
                    List<Mobility> mobility = _context.Mobility.Where(x => (x.mobilityListID == item.list_mobilityID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    foreach (var patient in mobility)
                    {
                        string patientName = patient.PatientAllocation.Patient.firstName + " " + patient.PatientAllocation.Patient.lastName;
                        string status = "Active";

                        if (DateTime.Compare(patient.PatientAllocation.Patient.startDate, date) > 0)
                            status = "Not yet admitted";
                        else if (patient.PatientAllocation.Patient.endDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.endDate, date) < 0)
                            status = "Terminated";
                        else if (patient.PatientAllocation.Patient.inactiveDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.inactiveDate, date) <= 0)
                            status = "Inactive";

                        searchResult.Add(new SearchResultViewModel
                        {
                            headerName = patientName,
                            name = item.value,
                            status = status,
                            type = "Mobility aid name",
                            href = "/Doctor/ViewPatient?patientID=" + patient.PatientAllocation.patientID,
                            message = " has the contained words of '" + searchWords + "'"
                        });
                    }
                }
            }
            searchResult = searchResult.OrderBy(x => x.name).ThenBy(x => x.headerName).ToList();
            return searchResult;
        }

        public List<SearchResultViewModel> searchActivityName(List<SearchResultViewModel> searchResult, string searchWords)
        {
            List<CentreActivity> list = _context.CentreActivities.Where(x => (x.isApproved == 1 && x.isDeleted != 1)).ToList();
            DateTime date = DateTime.Today;

            foreach (var item in list)
            {
                if (item.activityTitle.ToLower().Replace(" ", "").Contains(searchWords.ToLower().Replace(" ", "")))
                {
                    searchResult.Add(new SearchResultViewModel
                    {
                        headerName = null,
                        name = item.activityTitle,
                        status = null,
                        type = "Activity title",
                        href = null,
                        message = " has the contained words of '" + searchWords + "'"
                    });
                }
            }
            searchResult = searchResult.OrderBy(x => x.name).ThenBy(x => x.headerName).ToList();
            return searchResult;
        }

        public List<SearchResultViewModel> searchPatientPreferences(List<SearchResultViewModel> searchResult, string searchWords)
        {
            DateTime date = DateTime.Today;

            List<string> possibleWord = new List<string> { "Like", "Dislike", "Neutral" };

            foreach (var item in possibleWord)
            {
                if (item.ToLower().Replace(" ", "").Contains(searchWords.ToLower().Replace(" ", "")))
                {
                    List<ActivityPreference> list = new List<ActivityPreference>();

                    if (item == "Like")
                        list = _context.ActivityPreferences.Where(x => (x.isLike == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();

                    else if (item == "Dislike")
                        list = _context.ActivityPreferences.Where(x => (x.isDislike == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();

                    else if (item == "Neutral")
                        list = _context.ActivityPreferences.Where(x => (x.isNeutral == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();

                    else if (item == "All")
                        list = _context.ActivityPreferences.Where(x => (x.isApproved == 1 && x.isDeleted != 1)).ToList();

                    foreach (var patient in list)
                    {
                        string patientName = patient.PatientAllocation.Patient.firstName + " " + patient.PatientAllocation.Patient.lastName;
                        string status = "Active";

                        if (DateTime.Compare(patient.PatientAllocation.Patient.startDate, date) > 0)
                            status = "Not yet admitted";
                        else if (patient.PatientAllocation.Patient.endDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.endDate, date) < 0)
                            status = "Terminated";
                        else if (patient.PatientAllocation.Patient.inactiveDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.inactiveDate, date) <= 0)
                            status = "Inactive";

                        string preferenceString = patient.isLike == 1 ? "(Like)" : patient.isDislike == 1 ? "(Dislike)" : "(Neutral)";

                        searchResult.Add(new SearchResultViewModel
                        {
                            headerName = patientName,
                            name = item,
                            status = status,
                            type = "Patient Preferences",
                            href = "/Doctor/ViewPatient?patientID=" + patient.PatientAllocation.patientID,
                            message = " has the contained words of '" + searchWords + "'. The activity related is " + patient.CentreActivity.activityTitle + " " + preferenceString
                        });

                    }
                }
            }
            searchResult = searchResult.OrderBy(x => x.name).ThenBy(x => x.headerName).ToList();
            return searchResult;
        }

        public List<SearchResultViewModel> searchExclusion(List<SearchResultViewModel> searchResult, string searchWords)
        {
            DateTime date = DateTime.Today;

            List<string> possibleWord = new List<string> { "Routine", "Centre activity", "All" };

            foreach (var item in possibleWord)
            {
                if (item.ToLower().Replace(" ", "").Contains(searchWords.ToLower().Replace(" ", "")))
                {
                    List<ActivityExclusion> list = new List<ActivityExclusion>();

                    if (item == "Routine")
                        list = _context.ActivityExclusions.Where(x => (x.routineID != null && x.isApproved == 1 && x.isDeleted != 1)).ToList();

                    else if (item == "Centre activity")
                        list = _context.ActivityExclusions.Where(x => (x.centreActivityID != null && x.isDeleted != 1)).ToList();

                    else if (item == "All")
                        list = _context.ActivityExclusions.Where(x => (x.isApproved == 1 && x.isDeleted != 1)).ToList();

                    foreach (var patient in list)
                    {
                        string patientName = patient.PatientAllocation.Patient.firstName + " " + patient.PatientAllocation.Patient.lastName;
                        string status = "Active";

                        if (DateTime.Compare(patient.PatientAllocation.Patient.startDate, date) > 0)
                            status = "Not yet admitted";
                        else if (patient.PatientAllocation.Patient.endDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.endDate, date) < 0)
                            status = "Terminated";
                        else if (patient.PatientAllocation.Patient.inactiveDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.inactiveDate, date) <= 0)
                            status = "Inactive";

                        string activityName = null;
                        string title = null;

                        if (patient.routineID != null)
                        {
                            activityName = patient.Routine.eventName;
                            title = "routine title";
                        }
                        else
                        {
                            activityName = patient.CentreActivity.activityTitle;
                            title = "centre activity title";
                        }

                        DateTime exclusionDate = new DateTime();

                        string message = null;
                        if (DateTime.Compare(patient.dateTimeStart, date) > 0)
                        {
                            message = patientName + " will be excluded from the " + title + " of activity name " + activityName + " starting from " + patient.dateTimeStart.ToString("dd/MM/yyyy");
                            exclusionDate = patient.dateTimeStart;
                        }
                        else if (DateTime.Compare(patient.dateTimeEnd, date) < 0)
                        {
                            message = patientName + " has been excluded from the " + title + " of activity name " + activityName + " ending at " + patient.dateTimeEnd.ToString("dd/MM/yyyy");
                            exclusionDate = patient.dateTimeEnd;
                        }
                        else
                        {
                            message = patientName + " is excluded from the " + title + " of activity name " + activityName + " that will ends at " + patient.dateTimeEnd.ToString("dd/MM/yyyy");
                            exclusionDate = patient.dateTimeEnd;
                        }

                        searchResult.Add(new SearchResultViewModel
                        {
                            headerName = patientName,
                            name = activityName,
                            status = status,
                            type = "Activity exclusion",
                            href = "/Doctor/ViewPatient?patientID=" + patient.PatientAllocation.patientID,
                            message = message,
                            date = exclusionDate
                        });
                    }
                }
            }
            searchResult = searchResult.OrderByDescending(x => x.date).ThenBy(x => x.name).ThenBy(x => x.headerName).ToList();
            return searchResult;
        }

        public List<SearchResultViewModel> searchRecommendation(List<SearchResultViewModel> searchResult, string searchWords)
        {
            DateTime date = DateTime.Today;

            List<string> possibleWord = new List<string> { "Yes", "No", "All" };

            foreach (var item in possibleWord)
            {
                List<ActivityPreference> list = new List<ActivityPreference>();

                if (item.ToLower().Replace(" ", "").Contains(searchWords.ToLower().Replace(" ", "")))
                {
                    if (item == "Yes")
                        list = _context.ActivityPreferences.Where(x => (x.doctorRecommendation == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();

                    else if (item == "No")
                        list = _context.ActivityPreferences.Where(x => (x.doctorRecommendation == 0 && x.isApproved == 1 && x.isDeleted != 1)).ToList();

                    else if (item == "All")
                        list = _context.ActivityPreferences.Where(x => (x.isApproved == 1 && x.isDeleted != 1)).ToList();

                    foreach (var patient in list)
                    {
                        string patientName = patient.PatientAllocation.Patient.firstName + " " + patient.PatientAllocation.Patient.lastName;
                        string status = "Active";

                        if (DateTime.Compare(patient.PatientAllocation.Patient.startDate, date) > 0)
                            status = "Not yet admitted";
                        else if (patient.PatientAllocation.Patient.endDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.endDate, date) < 0)
                            status = "Terminated";
                        else if (patient.PatientAllocation.Patient.inactiveDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.inactiveDate, date) <= 0)
                            status = "Inactive";

                        string recommendationString = patient.doctorRecommendation == 1 ? "Yes" : "No";

                        searchResult.Add(new SearchResultViewModel
                        {
                            headerName = patientName,
                            name = item,
                            status = status,
                            type = "Activity Recommendation",
                            href = "/Doctor/ViewPatient?patientID=" + patient.PatientAllocation.patientID,
                            message = " has the contained words of '" + searchWords + "'. The activity related is " + patient.CentreActivity.activityTitle + "(" + recommendationString + ")"
                        });
                    }
                }
            }
            searchResult = searchResult.OrderBy(x => x.name).ThenBy(x => x.headerName).ToList();
            return searchResult;
        }

        public List<SearchResultViewModel> searchProblemName(List<SearchResultViewModel> searchResult, string searchWords)
        {
            List<List_ProblemLog> list = _context.ListProblemLogs.Where(x => (x.isDeleted != 1)).ToList();
            DateTime date = DateTime.Today;

            foreach (var item in list)
            {
                if (item.value.ToLower().Replace(" ", "").Contains(searchWords.ToLower().Replace(" ", "")))
                {
                    List<ProblemLog> problemLog = _context.ProblemLogs.Where(x => (x.problemLogID == item.list_problemLogID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    foreach (var patient in problemLog)
                    {
                        string patientName = patient.PatientAllocation.Patient.firstName + " " + patient.PatientAllocation.Patient.lastName;
                        string status = "Active";

                        if (DateTime.Compare(patient.PatientAllocation.Patient.startDate, date) > 0)
                            status = "Not yet admitted";
                        else if (patient.PatientAllocation.Patient.endDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.endDate, date) < 0)
                            status = "Terminated";
                        else if (patient.PatientAllocation.Patient.inactiveDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.inactiveDate, date) <= 0)
                            status = "Inactive";

                        searchResult.Add(new SearchResultViewModel
                        {
                            headerName = patientName,
                            name = item.value,
                            status = status,
                            type = "Problem Category",
                            href = "/Doctor/ViewPatient?patientID=" + patient.PatientAllocation.patientID,
                            message = " has the contained words of '" + searchWords + "'"
                        });
                    }
                }
            }
            searchResult = searchResult.OrderBy(x => x.name).ThenBy(x => x.headerName).ToList();
            return searchResult;
        }

        public List<SearchResultViewModel> searchGameName(List<SearchResultViewModel> searchResult, string searchWords)
        {
            List<SearchResultViewModel> tempResult = new List<SearchResultViewModel>();
            List<Game> list = _context.Games.Where(x => (x.isApproved == 1 && x.isDeleted != 1)).ToList();
            DateTime date = DateTime.Today;

            foreach (var item in list)
            {
                if (item.gameName.ToLower().Replace(" ", "").Contains(searchWords.ToLower().Replace(" ", "")))
                {
                    List<AssignedGame> assginedGame = _context.AssignedGames.Where(x => (x.gameID == item.gameID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    foreach (var patient in assginedGame)
                    {
                        string patientName = patient.PatientAllocation.Patient.firstName + " " + patient.PatientAllocation.Patient.lastName;
                        string status = "Active";

                        if (DateTime.Compare(patient.PatientAllocation.Patient.startDate, date) > 0)
                            status = "Not yet admitted";
                        else if (patient.PatientAllocation.Patient.endDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.endDate, date) < 0)
                            status = "Terminated";
                        else if (patient.PatientAllocation.Patient.inactiveDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.inactiveDate, date) <= 0)
                            status = "Inactive";

                        searchResult.Add(new SearchResultViewModel
                        {
                            headerName = patientName,
                            name = item.gameName,
                            status = status,
                            type = "Game title for patient",
                            href = "/Doctor/ViewPatient?patientID=" + patient.PatientAllocation.patientID,
                            message = " has the contained words of '" + searchWords + "'"
                        });
                    }

                    List<GameAssignedDementia> gameAssignedDementia = _context.GameAssignedDementias.Where(x => (x.gameID == item.gameID && x.isDeleted != 1)).ToList();
                    foreach (var patient in gameAssignedDementia)
                    {
                        string approvedStatus = patient.isApproved == 1 ? "(Approved)" : patient.isApproved == 0 ? "(Rejected)" : "(Pending)";

                        string[] tokens = patient.DementiaType.dementiaType.Split(' ');
                        string dementiaName = "";

                        for (int j = 0; j < tokens.Length - 2; j++)
                        {
                            dementiaName += tokens[j];

                            if (j + 1 < tokens.Length - 2)
                                dementiaName += " ";
                        }

                        tempResult.Add(new SearchResultViewModel
                        {
                            headerName = null,
                            name = item.gameName,
                            status = null,
                            type = "Game title for dementia",
                            href = null,
                            message = " has the contained words of '" + searchWords + "'. The associated dementia is " + dementiaName + " " + approvedStatus
                        });
                    }
                }
            }
            searchResult = searchResult.OrderBy(x => x.name).ThenBy(x => x.headerName).ToList();
            tempResult = tempResult.OrderBy(x => x.name).ToList();

            foreach (var result in tempResult)
                searchResult.Add(result);

            return searchResult;
        }

        public List<SearchResultViewModel> searchGameCategoryName(List<SearchResultViewModel> searchResult, string searchWords)
        {
            List<SearchResultViewModel> tempResult = new List<SearchResultViewModel>();
            List<Category> list = _context.Categories.Where(x => (x.isApproved == 1 && x.isDeleted != 1)).ToList();
            DateTime date = DateTime.Today;

            foreach (var item in list)
            {
                if (item.categoryName.ToLower().Replace(" ", "").Contains(searchWords.ToLower().Replace(" ", "")))
                {
                    List<GameCategory> gameCategory = _context.GameCategories.Where(x => (x.categoryID == item.categoryID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    foreach (var game in gameCategory)
                    {
                        List<AssignedGame> assignedGame = _context.AssignedGames.Where(x => (x.gameID == game.gameID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                        foreach (var patient in assignedGame)
                        {
                            string patientName = patient.PatientAllocation.Patient.firstName + " " + patient.PatientAllocation.Patient.lastName;
                            string status = "Active";

                            if (DateTime.Compare(patient.PatientAllocation.Patient.startDate, date) > 0)
                                status = "Not yet admitted";
                            else if (patient.PatientAllocation.Patient.endDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.endDate, date) < 0)
                                status = "Terminated";
                            else if (patient.PatientAllocation.Patient.inactiveDate != null && DateTime.Compare((DateTime)patient.PatientAllocation.Patient.inactiveDate, date) <= 0)
                                status = "Inactive";

                            searchResult.Add(new SearchResultViewModel
                            {
                                headerName = patientName,
                                name = item.categoryName,
                                status = status,
                                type = "Game category for patient",
                                href = "/Doctor/ViewPatient?patientID=" + patient.PatientAllocation.patientID,
                                message = " has the contained words of '" + searchWords + "'"
                            });
                        }
                    }
                    List<GameCategoryAssignedDementia> gameCategoryAssignedDementia = _context.GameCategoryAssignedDementia.Where(x => (x.categoryID == item.categoryID && x.isDeleted != 1)).ToList();
                    foreach (var patient in gameCategoryAssignedDementia)
                    {
                        string approvedStatus = patient.isApproved == 1 ? "(Approved)" : patient.isApproved == 0 ? "(Rejected)" : "(Pending)";

                        string[] tokens = patient.DementiaType.dementiaType.Split(' ');
                        string dementiaName = "";

                        for (int j = 0; j < tokens.Length - 2; j++)
                        {
                            dementiaName += tokens[j];

                            if (j + 1 < tokens.Length - 2)
                                dementiaName += " ";
                        }

                        tempResult.Add(new SearchResultViewModel
                        {
                            headerName = null,
                            name = item.categoryName,
                            status = null,
                            type = "Game category for dementia",
                            href = null,
                            message = " has the contained words of '" + searchWords + "'. The associated dementia is " + dementiaName + " " + approvedStatus
                        });
                    }
                }
            }
            searchResult = searchResult.OrderBy(x => x.name).ThenBy(x => x.headerName).ToList();
            tempResult = tempResult.OrderBy(x => x.name).ToList();

            foreach (var result in tempResult)
                searchResult.Add(result);

            return searchResult;
        }
    }
}