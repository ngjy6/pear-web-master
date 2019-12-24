using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class AdminManageAccountViewModel
    {
        public ApplicationUser User { get; set; }

        public string id { get; set; }

        public string userName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Secret Question")]
        public string secretQuestion { get; set; }
        [Display(Name = "Secret Answer")]
        public string secretAnswer { get; set; }

        [Display(Name = "Preferred Name")]
        public string preferredName { get; set; }
        [Display(Name = "First Name")]
        public string firstName { get; set; }
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        public string nric { get; set; }
        [Display(Name = "Gender")]
        public string gender { get; set; }
        public DateTime dob { get; set; }
        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "Locked")]
        public int isLocked { get; set; }
        [Display(Name = "Locked")]
        public bool lockBool { get; set; }

        [StringLength(8, ErrorMessage = "{0} must be {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Handphone No")]
        public string handphoneNo { get; set; }
        [StringLength(8, ErrorMessage = "{0} must be {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Office No")]
        public string officeNo { get; set; }
        public int userTypeID { get; set; }

        [Display(Name = "Reason for deleting record: ")]
        public string reason { get; set; }

        public IEnumerable<UserType> UserTypes { get; set; }
        public List<UserInformationViewModel> UserInformation { get; set; }
        public string Code { get; set; }
    }

    public class UserInformationViewModel
    {
        public string Id { get; set; }
        public int userID { get; set; }
        public string preferredName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public int userTypeID { get; set; }
        [Display(Name = "Locked")]
        public int accountLock { get; set; }
        public string reason { get; set; }
        public int noOfPatient { get; set; }
        public int isLocked { get; set; }
        public int isDeleted { get; set; }
        public DateTime? lastLogin { get; set; }
    }

    public class UserAccountViewModel
    {
        public string id { get; set; }
        public ApplicationUser User { get; set; }
        [Display(Name = "User ID")]
        public int userID { get; set; }

        [Display(Name = "Upload Profile Image")]
        public string imageUrl { get; set; }

        [Display(Name = "User Type")]
        public string userType { get; set; }

        [Display(Name = "Username")]
        public string userName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string currentPassword { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Current Secret Question")]
        public string currentSecretQuestion { get; set; }
        [Display(Name = "New Secret Question")]
        public string newSecretQuestion { get; set; }
        [Display(Name = "New Secret Answer")]
        public string secretAnswer { get; set; }

        [Display(Name = "Preferred Name")]
        public string preferredName { get; set; }
        [Display(Name = "First Name")]
        public string firstName { get; set; }
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "Last Login Session")]
        public string lastLogin { get; set; }
        [Display(Name = "NRIC")]
        public string nric { get; set; }
        [Display(Name = "Gender")]
        public string gender { get; set; }
        [Display(Name = "Date of Birth")]
        public string date { get; set; }
        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "Allow Notification")]
        public int allowNotification { get; set; }
        [Display(Name = "Allow Notification")]
        public bool notificationBool { get; set; }

        [StringLength(8, ErrorMessage = "{0} must be {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Handphone No")]
        public string handphoneNo { get; set; }
        [StringLength(8, ErrorMessage = "{0} must be {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Office No")]
        public string officeNo { get; set; }
    }

    public class DeleteAccountViewModel
    {
        [Display(Name = "Reason for deleting record: ")]
        public string reason { get; set; }
    }

    public class DropListViewModel
    {
        public List<ListAttributeViewModel> ListAtttribute { get; set; }
    }

    public class ListAttributeViewModel
    {
        public int index { get; set; }
        public string name { get; set; }
        public int check { get; set; }
        public int notCheck { get; set; }
        public string lastCheckedDate { get; set; }
        public string newValue { get; set; }
    }

    public class SelectedListViewModel
    {
        public string name { get; set; }
        public int check { get; set; }
        public int notCheck { get; set; }
        public string view { get; set; }
        public string action { get; set; }
        public string text { get; set; }

        public List<System.Web.Mvc.SelectListItem> checkedList { get; set; }
        public List<System.Web.Mvc.SelectListItem> uncheckedList { get; set; }
    }

    public class UserResetPasswordViewModel
    {
        public string id { get; set; }
        public string code { get; set;
        }
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetLinkViewModel
    {
        public string email { get; set; }

        public string username { get; set; }
        public string secretAnswer { get; set; }
    }
}