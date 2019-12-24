using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.App_Code;

using Newtonsoft.Json.Linq;
using FireSharp.Config;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Web.Mvc;

namespace NTU_FYP_REBUILD_17.Controllers.Synchronization
{
    public class ListMethod
    {
        private ApplicationDbContext _context;
        SOLID shortcutMethod = new SOLID();

        public ListMethod()
        {
            _context = new ApplicationDbContext();
        }

        public DropListViewModel getDropListItem()
        {
            List<AlbumCategory> album = _context.AlbumCategories.Where(x => (x.isApproved == 1 && x.isDeleted != 1)).ToList();
            List<List_Allergy> allergy = _context.ListAllergy.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Country> country = _context.ListCountries.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Diet> diet = _context.ListDiets.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Dislike> dislike = _context.ListDislikes.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Education> education = _context.ListEducations.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Habit> habit = _context.ListHabits.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Hobby> hobby = _context.ListHobbies.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Language> language = _context.ListLanguages.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Like> like = _context.ListLikes.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_LiveWith> liveWith = _context.ListLiveWiths.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Mobility> mobility = _context.ListMobility.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Occupation> occupation = _context.ListOccupations.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Pet> pet = _context.ListPets.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Prescription> prescription = _context.ListPrescriptions.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_ProblemLog> problemLog = _context.ListProblemLogs.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Relationship> relationship = _context.ListRelationships.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_Religion> religion = _context.ListReligions.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            List<List_SecretQuestion> secretQuestion = _context.ListSecretQuestion.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();

            List<AlbumCategory> album2 = _context.AlbumCategories.Where(x => (x.isApproved == 0 && x.isDeleted != 1)).ToList();
            List<List_Allergy> allergy2 = _context.ListAllergy.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Country> country2 = _context.ListCountries.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Diet> diet2 = _context.ListDiets.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Dislike> dislike2 = _context.ListDislikes.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Education> education2 = _context.ListEducations.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Habit> habit2 = _context.ListHabits.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Hobby> hobby2 = _context.ListHobbies.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Language> language2 = _context.ListLanguages.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Like> like2 = _context.ListLikes.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_LiveWith> liveWith2 = _context.ListLiveWiths.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Mobility> mobility2 = _context.ListMobility.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Occupation> occupation2 = _context.ListOccupations.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Pet> pet2 = _context.ListPets.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Prescription> prescription2 = _context.ListPrescriptions.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_ProblemLog> problemLog2 = _context.ListProblemLogs.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Relationship> relationship2 = _context.ListRelationships.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_Religion> religion2 = _context.ListReligions.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            List<List_SecretQuestion> secretQuestion2 = _context.ListSecretQuestion.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();

            DropListViewModel dropList = new DropListViewModel();
            List<ListAttributeViewModel> listAttribute = new List<ListAttributeViewModel>();

            List<string> uncheckList = new List<string>();
            for (int i = 0; i < album2.Count; i++)
                uncheckList.Add(album2[i].albumCatName);
            string uncheckedList = string.Join(",", uncheckList);
            DateTime lastDate = (album.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            string lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Album", check = album.Count, notCheck = album2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < allergy2.Count; i++)
                uncheckList.Add(allergy2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (allergy.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Allergy", check = allergy.Count, notCheck = allergy2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < country2.Count; i++)
                uncheckList.Add(country2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (country.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Country", check = country.Count, notCheck = country2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < diet2.Count; i++)
                uncheckList.Add(diet2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (diet.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Diet", check = diet.Count, notCheck = diet2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < dislike2.Count; i++)
                uncheckList.Add(dislike2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (dislike.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Dislike", check = dislike.Count, notCheck = dislike2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < education2.Count; i++)
                uncheckList.Add(education2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (education.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Education", check = education.Count, notCheck = education2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < habit2.Count; i++)
                uncheckList.Add(habit2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (habit.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Habit", check = habit.Count, notCheck = habit2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < hobby2.Count; i++)
                uncheckList.Add(hobby2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (hobby.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Hobby", check = hobby.Count, notCheck = hobby2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < language2.Count; i++)
                uncheckList.Add(language2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (language.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Language", check = language.Count, notCheck = language2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < like2.Count; i++)
                uncheckList.Add(like2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (like.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Like", check = like.Count, notCheck = like2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < liveWith2.Count; i++)
                uncheckList.Add(liveWith2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (liveWith.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Live With", check = liveWith.Count, notCheck = liveWith2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < mobility2.Count; i++)
                uncheckList.Add(mobility2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (mobility.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Mobility", check = mobility.Count, notCheck = mobility2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < occupation2.Count; i++)
                uncheckList.Add(occupation2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (occupation.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Occupation", check = occupation.Count, notCheck = occupation2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < pet2.Count; i++)
                uncheckList.Add(pet2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (pet.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Pet", check = pet.Count, notCheck = pet2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < prescription2.Count; i++)
                uncheckList.Add(prescription2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (prescription.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Prescription", check = prescription.Count, notCheck = prescription2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < problemLog2.Count; i++)
                uncheckList.Add(problemLog2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (problemLog.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Problem Log", check = problemLog.Count, notCheck = problemLog2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < relationship2.Count; i++)
                uncheckList.Add(relationship2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (relationship.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Relationship", check = relationship.Count, notCheck = relationship2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < religion2.Count; i++)
                uncheckList.Add(religion2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (religion.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Religion", check = religion.Count, notCheck = religion2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });

            uncheckList = new List<string>();
            for (int i = 0; i < secretQuestion2.Count; i++)
                uncheckList.Add(secretQuestion2[i].value);
            uncheckedList = string.Join(",", uncheckList);
            lastDate = (secretQuestion.OrderByDescending(x => x.createDateTime).ToList())[0].createDateTime;
            lastCheckedDate = shortcutMethod.leadingZero(lastDate.Day.ToString()) + "/" + shortcutMethod.leadingZero(lastDate.Month.ToString()) + "/" + lastDate.Year.ToString();
            listAttribute.Add(new ListAttributeViewModel { name = "Secret Question", check = secretQuestion.Count, notCheck = secretQuestion2.Count, newValue = uncheckedList, lastCheckedDate = lastCheckedDate });
            /*
            List<ListAttributeViewModel> listAttribute2 = listAttribute.Where(x => x.notCheck > 0).ToList();
            List<ListAttributeViewModel> listAttribute3 = listAttribute.Where(x => x.notCheck == 0).ToList();
            for (int i = 0; i < listAttribute2.Count; i++)
                listAttribute2[i].index = i+1;
            for (int i = 0; i < listAttribute3.Count; i++)
                listAttribute3[i].index = i + listAttribute2.Count + 1;
            for (int i=0; i<listAttribute3.Count; i++)
                listAttribute2.Add(listAttribute3[i]);
            dropList.ListAtttribute = listAttribute2;*/

            listAttribute = listAttribute.OrderByDescending(x => x.notCheck).ThenBy(x => x.name).ToList();
            for (int i = 0; i < listAttribute.Count; i++)
                listAttribute[i].index = i + 1;

            dropList.ListAtttribute = listAttribute;
            return dropList;
        }

        public List<SelectListItem> getAlbumList(int approved, bool showNone, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            AlbumCategory profile = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatName == "Profile Picture" && x.isApproved == 1 && x.isDeleted != 1));
            var listItem = _context.AlbumCategories.Where(x => (x.albumCatID != profile.albumCatID && x.isApproved == approved && x.isDeleted != 1)).OrderBy(x => (x.albumCatName)).ToList();

            if (showNone)
                list.Add(new SelectListItem() { Value = "0", Text = "-- Select an album name --" });
            
            list.Add(new SelectListItem() { Value = profile.albumCatID.ToString(), Text = profile.albumCatName });
            
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.albumCatID.ToString(), Text = item.albumCatName });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getAllergyList(int check, bool showNone, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var none = _context.ListAllergy.SingleOrDefault(x => (x.value == "None"));
            var listItem = _context.ListAllergy.Where(x => (x.list_allergyID != none.list_allergyID && x.isChecked == check && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();

            if (showNone)
                list.Add(new SelectListItem() { Value = none.list_allergyID.ToString(), Text = none.value });

            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_allergyID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getCountryList(int check)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var listItem = _context.ListCountries.Where(x => (x.isChecked == check && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_countryID.ToString(), Text = item.value });

            return list;
        }

        public List<SelectListItem> getDementiaList(int check)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var listItem = _context.DementiaTypes.Where(x => (x.isApproved == check && x.isDeleted != 1)).OrderBy(x => (x.dementiaID));

            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.dementiaID.ToString(), Text = item.dementiaType });

            return list;
        }

        public List<SelectListItem> getDietList(int check, bool showNone, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var none = _context.ListDiets.SingleOrDefault(x => (x.value == "No special diet"));

            var listItem = _context.ListDiets.Where(x => (x.isChecked == check && x.list_dietID != none.list_dietID && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();

            if (showNone)
                list.Add(new SelectListItem() { Value = none.list_dietID.ToString(), Text = none.value });
            
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_dietID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getDislikeList(int check, bool showNone, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var none = _context.ListDislikes.SingleOrDefault(x => (x.value == "None"));

            var listItem = _context.ListDislikes.Where(x => (x.isChecked == check && x.list_dislikeID != none.list_dislikeID && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();

            if (showNone)
                list.Add(new SelectListItem() { Value = none.list_dislikeID.ToString(), Text = none.value });
            
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_dislikeID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getEducationList(int check, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var listItem = _context.ListEducations.Where(x => (x.isChecked == check && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_educationID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getGameList(int check, bool showAll, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var listItem = _context.Games.Where(x => (x.isApproved == check && x.isDeleted != 1)).OrderBy(x => (x.gameName));

            if (showAll)
                list.Add(new SelectListItem() { Value = "0", Text = "All Game" });

            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.gameID.ToString(), Text = item.gameName });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getGameCategoryList(int check, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var listItem = _context.Categories.Where(x => (x.isApproved == check && x.isDeleted != 1)).OrderBy(x => (x.categoryName));
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.categoryID.ToString(), Text = item.categoryName });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getHabitList(int check, bool showNone, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var none = _context.ListHabits.SingleOrDefault(x => (x.value == "None"));

            var listItem = _context.ListHabits.Where(x => (x.isChecked == check && x.list_habitID != none.list_habitID && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();

            if (showNone)
                list.Add(new SelectListItem() { Value = none.list_habitID.ToString(), Text = none.value });
            
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_habitID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getHobbyList(int check, bool showNone, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var none = _context.ListHobbies.SingleOrDefault(x => (x.value == "None"));

            var listItem = _context.ListHobbies.Where(x => (x.isChecked == check && x.list_hobbyID != none.list_hobbyID && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();
            if (showNone)
                list.Add(new SelectListItem() { Value = none.list_hobbyID.ToString(), Text = none.value });
            
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_hobbyID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getLanguageList(int check, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var listItem = _context.ListLanguages.Where(x => (x.isChecked == check && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_languageID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getLikeList(int check, bool showNone, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var none = _context.ListLikes.SingleOrDefault(x => (x.value == "None"));

            var listItem = _context.ListLikes.Where(x => (x.isChecked == check && x.list_likeID != none.list_likeID && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();
            if (showNone)
                list.Add(new SelectListItem() { Value = none.list_likeID.ToString(), Text = none.value });
            
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_likeID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getLiveWithList(int check, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var alone = _context.ListLiveWiths.SingleOrDefault(x => (x.value == "Alone"));

            var listItem = _context.ListLiveWiths.Where(x => (x.list_liveWithID != alone.list_liveWithID && x.isChecked == check && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();
            list.Add(new SelectListItem() { Value = alone.list_liveWithID.ToString(), Text = alone.value });

            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_liveWithID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getMobilityList(int check, bool showNone, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var none = _context.ListMobility.SingleOrDefault(x => (x.value == "Mobile"));

            var listItem = _context.ListMobility.Where(x => (x.isChecked == check && x.list_mobilityID != none.list_mobilityID && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();
            if (showNone)
                list.Add(new SelectListItem() { Value = none.list_mobilityID.ToString(), Text = none.value });
            
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_mobilityID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getOccupationList(int check, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var unemployed = _context.ListOccupations.SingleOrDefault(x => (x.value == "Unemployed"));

            var listItem = _context.ListOccupations.Where(x => (x.list_occupationID != unemployed.list_occupationID && x.isChecked == check && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();

            list.Add(new SelectListItem() { Value = unemployed.list_occupationID.ToString(), Text = unemployed.value });
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_occupationID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getPetList(int check, bool showNone, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var none = _context.ListPets.SingleOrDefault(x => (x.value == "None"));

            var listItem = _context.ListPets.Where(x => (x.list_petID != none.list_petID && x.isChecked == check && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();
            if (showNone)
                list.Add(new SelectListItem() { Value = none.list_petID.ToString(), Text = none.value });
            
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_petID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getPrescriptionList(int check, bool showNone, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var none = _context.ListPrescriptions.SingleOrDefault(x => (x.value == "None"));

            var listItem = _context.ListPrescriptions.Where(x => (x.list_prescriptionID != none.list_prescriptionID && x.isChecked == check && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();
            if (showNone)
                list.Add(new SelectListItem() { Value = none.list_prescriptionID.ToString(), Text = none.value });
            
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_prescriptionID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getProblemLogList(int check, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var listItem = _context.ListProblemLogs.Where(x => (x.isChecked == check && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_problemLogID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getRelationshipList(int check, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var listItem = _context.ListRelationships.Where(x => (x.isChecked == check && x.isDeleted != 1)).OrderBy(x => (x.list_relationshipID));
            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_relationshipID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getReligionList(int check, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var freeThinker = _context.ListReligions.SingleOrDefault(x => (x.value == "Free Thinker"));

            var listItem = _context.ListReligions.Where(x => (x.list_religionID != freeThinker.list_religionID && x.isChecked == check && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();
            list.Add(new SelectListItem() { Value = freeThinker.list_religionID.ToString(), Text = freeThinker.value });

            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_religionID.ToString(), Text = item.value });

            if (showOthers)
                list.Add(new SelectListItem() { Value = "-1", Text = "Others" });

            return list;
        }

        public List<SelectListItem> getDoctorSearchList()
        {
            List<SelectListItem> searchSelectListItem = new List<SelectListItem>();
            searchSelectListItem.Add(new SelectListItem() { Value = null, Text = null });
            searchSelectListItem.Add(new SelectListItem() { Value = "All", Text = "All" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Patient name", Text = "Patient name" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Name of dementia", Text = "Name of dementia" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Drug name", Text = "Drug name" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Allergic item name", Text = "Allergic item name" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Mobility aid name", Text = "Mobility aid name" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Activity title", Text = "Activity title" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Patient preferences", Text = "Patient preferences" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Activity exclusion", Text = "Activity exclusion" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Activity recommendation", Text = "Activity recommendation" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Problem category", Text = "Problem category" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Game title", Text = "Game title" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Game category", Text = "Game category" });
            return searchSelectListItem;
        }

        public List<SelectListItem> getGameTherapistSearchList()
        {
            List<SelectListItem> searchSelectListItem = new List<SelectListItem>();
            searchSelectListItem.Add(new SelectListItem() { Value = null, Text = null });
            searchSelectListItem.Add(new SelectListItem() { Value = "All", Text = "All" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Patient name", Text = "Patient name" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Name of dementia", Text = "Name of dementia" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Mobility aid name", Text = "Mobility aid name" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Game title", Text = "Game title" });
            searchSelectListItem.Add(new SelectListItem() { Value = "Game category", Text = "Game category" });
            return searchSelectListItem;
        }

        public List<SelectListItem> getSecretQuestionList(int check, bool showOthers)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var listItem = _context.ListSecretQuestion.Where(x => (x.isChecked == check && x.isDeleted != 1)).OrderBy(x => (x.value)).ToList();

            if (showOthers)
                list.Add(new SelectListItem() { Value = "0", Text = "-- Choose a Question --" });

            foreach (var item in listItem)
                list.Add(new SelectListItem() { Value = item.list_secretQuestionID.ToString(), Text = item.value });

            return list;
        }

        public List<SelectListItem> getUserTypeSelectListItem()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var roleList = _context.UserTypes.OrderBy(x => (x.userTypeName));
            list.Add(new SelectListItem() { Value = "0", Text = "-- Select User Type --" });
            foreach (var role in roleList)
                list.Add(new SelectListItem() { Value = role.userTypeID.ToString(), Text = role.userTypeName });

            return list;
        }

        /*
        public List<SelectListItem> getSecretQuestionSelectListItem()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var secretQuestion = _context.ListSecretQuestion.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).OrderBy(x => (x.value));
            list.Add(new SelectListItem() { Value = "0", Text = "-- Choose a Question --" });
            foreach (var question in secretQuestion)
                list.Add(new SelectListItem() { Value = question.value, Text = question.value });

            return list;
        }

        public List<SelectListItem> getCountrySelectListItem()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var countries = _context.ListCountries.Where(x => (x.isChecked == 1 && x.isDeleted != 1)).ToList();
            list.Add(new SelectListItem() { Value = "0", Text = "-- Select a Country --" });
            foreach (var country in countries)
                list.Add(new SelectListItem() { Value = country.list_countryID.ToString(), Text = country.value });

            return list;
        }*/

        public List<SelectListItem> getDaySelectListItem(int patientID)
        {
            Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID));
            int isRespiteCare = patient.isRespiteCare;

            List<SelectListItem> list = new List<SelectListItem>();
            List<string> day = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            for (int i = 0; i < day.Count; i++)
            {
                list.Add(new SelectListItem() { Value = day[i], Text = day[i] });
                if (isRespiteCare == 0 && i >= 4)
                    break;
            }
            return list;
        }

        public void addAlbum(int userID, string value, int isApproved)
        {
            DateTime date = DateTime.Now;

            AlbumCategory newItem = new AlbumCategory();
            newItem.albumCatName = value;
            newItem.isApproved = isApproved;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.AlbumCategories.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.AlbumCategories.Max(x => x.albumCatID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "albumCategory", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addAllergy(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Allergy newItem = new List_Allergy();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListAllergy.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListAllergy.Max(x => x.list_allergyID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_allergy", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addCountry(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Country newItem = new List_Country();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListCountries.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListCountries.Max(x => x.list_countryID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_country", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addDiet(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Diet newItem = new List_Diet();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListDiets.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListDiets.Max(x => x.list_dietID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_diet", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addDislike(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Dislike newItem = new List_Dislike();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListDislikes.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListDislikes.Max(x => x.list_dislikeID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_dislike", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addEducation(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Education newItem = new List_Education();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListEducations.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListEducations.Max(x => x.list_educationID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_education", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addHabit(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Habit newItem = new List_Habit();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListHabits.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListHabits.Max(x => x.list_habitID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_habit", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addHobby(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Hobby newItem = new List_Hobby();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListHobbies.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListHobbies.Max(x => x.list_hobbyID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_hobby", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addLanguage(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Language newItem = new List_Language();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListLanguages.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListLanguages.Max(x => x.list_languageID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_language", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addLike(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Like newItem = new List_Like();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListLikes.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListLikes.Max(x => x.list_likeID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_like", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addLiveWith(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_LiveWith newItem = new List_LiveWith();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListLiveWiths.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListLiveWiths.Max(x => x.list_liveWithID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_liveWith", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addMobility(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Mobility newItem = new List_Mobility();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListMobility.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListMobility.Max(x => x.list_mobilityID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_mobility", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addOccupation(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Occupation newItem = new List_Occupation();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListOccupations.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListOccupations.Max(x => x.list_occupationID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_occupation", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addPet(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Pet newItem = new List_Pet();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListPets.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListPets.Max(x => x.list_petID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_pet", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addPrescription(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Prescription newItem = new List_Prescription();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListPrescriptions.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListPrescriptions.Max(x => x.list_prescriptionID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_prescription", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addProblemLog(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_ProblemLog newItem = new List_ProblemLog();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListProblemLogs.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListProblemLogs.Max(x => x.list_problemLogID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_problemLog", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addRelationship(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Relationship newItem = new List_Relationship();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListRelationships.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListRelationships.Max(x => x.list_relationshipID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_relationship", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addReligion(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_Religion newItem = new List_Religion();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListReligions.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListReligions.Max(x => x.list_religionID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_religion", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void addSecretQuestion(int userID, string value, int isChecked)
        {
            DateTime date = DateTime.Now;

            List_SecretQuestion newItem = new List_SecretQuestion();
            newItem.value = value;
            newItem.isChecked = isChecked;
            newItem.isDeleted = 0;
            newItem.createDateTime = date;
            _context.ListSecretQuestion.Add(newItem);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newItem);
            string logDesc = "New list item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.ListReligions.Max(x => x.list_religionID);

            JObject newValue = new JObject();
            newValue["value"] = value;
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "list_secretQuestion", "ALL", null, logNewValue, rowAffected, 1, 1, null);
        }

        public void updateListLog(int userID, string oldLogData, string logData, string logOldValue, string tableAffected, int rowAffected, string value)
        {
            JObject newValue = new JObject();
            newValue["value"] = value;
            newValue["isChecked"] = 1;

            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);
            string logDesc = "Update list item";

            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, tableAffected, "ALL", logOldValue, logNewValue, rowAffected, 1, 1, null);
        }

        public void deleteListLog(int userID, string oldLogData, string logData, string logOldValue, string tableAffected, int rowAffected)
        {
            JObject newValue = new JObject();
            newValue["isChecked"] = 1;
            newValue["isDeleted"] = 1;

            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);
            string logDesc = "Delete list item";

            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, tableAffected, "ALL", logOldValue, logNewValue, rowAffected, 1, 1, null);
        }

        public int getAlbumUncheckedCount()
        {
            List<AlbumCategory> list = _context.AlbumCategories.Where(x => (x.isApproved == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getAllergyUncheckedCount()
        {
            List<List_Allergy> list = _context.ListAllergy.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getCountryUncheckedCount()
        {
            List<List_Country> list = _context.ListCountries.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getDietUncheckedCount()
        {
            List<List_Diet> list = _context.ListDiets.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getDislikeUncheckedCount()
        {
            List<List_Dislike> list = _context.ListDislikes.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getEducationUncheckedCount()
        {
            List<List_Education> list = _context.ListEducations.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getHabitUncheckedCount()
        {
            List<List_Habit> list = _context.ListHabits.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getHobbyUncheckedCount()
        {
            List<List_Hobby> list = _context.ListHobbies.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getLanguageUncheckedCount()
        {
            List<List_Language> list = _context.ListLanguages.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getLikeUncheckedCount()
        {
            List<List_Like> list = _context.ListLikes.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getLiveWithUncheckedCount()
        {
            List<List_LiveWith> list = _context.ListLiveWiths.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getMobilityUncheckedCount()
        {
            List<List_Mobility> list = _context.ListMobility.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getOccupationUncheckedCount()
        {
            List<List_Occupation> list = _context.ListOccupations.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getPetUncheckedCount()
        {
            List<List_Pet> list = _context.ListPets.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getPrescriptionUncheckedCount()
        {
            List<List_Prescription> list = _context.ListPrescriptions.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getProblemLogUncheckedCount()
        {
            List<List_ProblemLog> list = _context.ListProblemLogs.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getRelationshipUncheckedCount()
        {
            List<List_Relationship> list = _context.ListRelationships.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getReligionUncheckedCount()
        {
            List<List_Religion> list = _context.ListReligions.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public int getSecretQuestionUncheckedCount()
        {
            List<List_SecretQuestion> list = _context.ListSecretQuestion.Where(x => (x.isChecked == 0 && x.isDeleted != 1)).ToList();
            return list.Count;
        }

        public void updateLog(string tableAffected, string columnName, int checkedID, int uncheckedID)
        {
            List<Log> log = _context.Logs.Where(x => (x.tableAffected == tableAffected)).ToList();
            for (int j = 0; j < log.Count; j++)
            {
                if (log[j].oldLogData != null)
                {
                    JObject oldLogDataJObject = (JObject)JsonConvert.DeserializeObject(log[j].oldLogData);
                    if (oldLogDataJObject[columnName] != null)
                    {
                        int logOldValueID = (int)oldLogDataJObject[columnName];

                        if (logOldValueID == uncheckedID)
                            oldLogDataJObject[columnName] = checkedID;

                        log[j].oldLogData = oldLogDataJObject.ToString(Newtonsoft.Json.Formatting.None);
                    }
                }

                JObject logDataJObject = (JObject)JsonConvert.DeserializeObject(log[j].logData);
                if (logDataJObject[columnName] != null)
                {
                    int logDataID = (int)logDataJObject[columnName];
                    if (logDataID == uncheckedID)
                        logDataJObject[columnName] = checkedID;

                    log[j].logData = logDataJObject.ToString(Newtonsoft.Json.Formatting.None);
                }

                if (log[j].logOldValue != null && log[j].logOldValue != "")
                {
                    JObject logOldValueJObject = (JObject)JsonConvert.DeserializeObject(log[j].logOldValue);
                    if (logOldValueJObject[columnName] != null)
                    {
                        int logOldValueID = (int)logOldValueJObject[columnName];

                        if (logOldValueID == uncheckedID)
                            logOldValueJObject[columnName] = checkedID;

                        log[j].logOldValue = logOldValueJObject.ToString(Newtonsoft.Json.Formatting.None);
                    }
                }

                if (log[j].logNewValue != null && log[j].logNewValue != "")
                {
                    JObject logNewValueJObject = (JObject)JsonConvert.DeserializeObject(log[j].logNewValue);
                    if (logNewValueJObject[columnName] != null)
                    {
                        int logNewValueID = (int)logNewValueJObject[columnName];
                        if (logNewValueID == uncheckedID)
                            logNewValueJObject[columnName] = checkedID;

                        log[j].logNewValue = logNewValueJObject.ToString(Newtonsoft.Json.Formatting.None);
                    }
                }
            }
            _context.SaveChanges();
        }
    }
}