using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

	public class Variance
	{
		public string PropertyName { get; set; }
		public object valA { get; set; }
		public object valB { get; set; }
	}

	public class Person
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int Age { get; set; }
		public string HairColor { get; set; }
	}

	public class objectNewOld
	{
		public string oldData { get; set; }
		public string newData { get; set; }
	}

	public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string userName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class PreRegisterViewModel
    {
        public string patientName { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string nric { get; set; }
    }

    public class RegisterViewModel
    {
        [Display(Name = "Username")]
        public string userName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Required]
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

        public string userType { get; set; }

        [Display(Name = "Preferred Name")]
        public string preferredName { get; set; }
        [Display(Name = "First Name")]
        public string firstName { get; set; }
        [Display(Name = "Last Name")]
        public string lastName { get; set; }
        [StringLength(9, ErrorMessage = "{0} must be {2} characters long.", MinimumLength = 9)]
        [Display(Name = "NRIC")]
        public string nric { get; set; }

        [Display(Name = "Gender")]
        public string gender { get; set; }

        [Display(Name = "Date Of Birth")]
        public DateTime DOB { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [StringLength(8, ErrorMessage = "{0} must be {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Handphone No")]
        public string handphoneNo { get; set; }
        [StringLength(8, ErrorMessage = "{0} must be {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Office No")]
        public string officeNo { get; set; }
    
        [Display(Name = "User Type")]
        public string roleID { get; set; }
        
        public int isApproved { get; set; }
        public int isDeleted { get; set; }

        public IEnumerable<UserType> UserTypes { get; set; }
        public IEnumerable<List_SecretQuestion> SecretQuestions { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
