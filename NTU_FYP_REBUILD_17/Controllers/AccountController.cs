using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NTU_FYP_REBUILD_17.Controllers.Api;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;
using System.Web.Script.Serialization;
using NTU_FYP_REBUILD_17.Controllers.Synchronization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Routing;
//using System.Net;
//using System.Net.Mail;
using EASendMail;

namespace NTU_FYP_REBUILD_17.Controllers
{
    [Authorize]
    [System.Runtime.InteropServices.Guid("8B1B8546-85BA-4FF3-9E3D-962E8C93E324")]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext _context;

        App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        Controllers.Synchronization.ListMethod list = new Controllers.Synchronization.ListMethod();
        Controllers.Synchronization.AlbumMethod album = new Controllers.Synchronization.AlbumMethod();
        Controllers.Synchronization.AccountMethod account = new Controllers.Synchronization.AccountMethod();
        Controllers.Synchronization.CentreMethod centre = new Controllers.Synchronization.CentreMethod();

        public AccountController()
        {
            _context = new ApplicationDbContext();
        }
        /*
        public partial class TimeoutControl : System.Web.UI.UserControl
        {
            public string TimeOutUrl = "";

            public int PopupShowDelay
            {
                get { return 60000 * (Session.Timeout - 1); }
            }

            protected string QuotedTimeOutUrl
            {
                get { return '"' + ResolveClientUrl(TimeOutUrl).Replace("\"", "\\\"") + '"'; }
            }
        }*/

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        // GET: Role
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult RoleIndex()
        {
            List<RoleViewModel> list = new List<RoleViewModel>();

            var roles = _context.UserTypes.OrderBy(x => (x.userTypeName));
            foreach (var role in roles)
                list.Add(new RoleViewModel(role));

            ViewBag.Modal = TempData["Modal"];
            return View(list);
        }

        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public async Task<ActionResult> RoleCreate(RoleViewModel model)
        {
            var role = new ApplicationRole() { Name = model.Name };
            await RoleManager.CreateAsync(role);

            UserType userType = new UserType
            {
                userTypeName = model.Name,
                isDeleted = 0,
                createDateTime = DateTime.Now
            };

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            string logDesc = "New user type";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            int rowAffected = _context.UserTypes.Max(x => (x.userTypeID));

            // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
            shortcutMethod.addLogAccount(userID, null, null, null, logDesc, logCategoryID, null, "AspNetRoles", null, null, null, null, null);
            shortcutMethod.addLogAccount(userID, null, null, null, logDesc, logCategoryID, null, "userType", null, rowAffected, null, null, null);
            _context.UserTypes.Add(userType);
            _context.SaveChanges();

            TempData["Message"] = "Created new role: " + model.Name;
            TempData["Modal"] = "true";

            return RedirectToAction("RoleIndex");
        }

        /*
        public async Task<ActionResult> RoleEdit(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            return View(new RoleViewModel(role));
        }

        [HttpPut]
        public async Task<ActionResult> RoleEdit(RoleViewModel model)
        {
            var role = new ApplicationRole() { Id = model.Id, Name = model.Name };
            await RoleManager.UpdateAsync(role);
            return RedirectToAction("RoleIndex");
        }

        public async Task<ActionResult> RoleDelete(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            await RoleManager.DeleteAsync(role);
            return View(new RoleViewModel(role));
        }

        public async Task<ActionResult> RoleDetails(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            return View(new RoleViewModel(role));
        }

        public async Task<ActionResult> RoleDeleteConfirmed(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            await RoleManager.DeleteAsync(role);
            return RedirectToAction("Index");
        }*/

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            var checkLoggedin = Session["logged"];

            if (checkLoggedin != null)
            {
                System.Diagnostics.Debug.Write("checkLoggedin=" + checkLoggedin.ToString());
            }

            if (checkLoggedin != null && checkLoggedin.ToString().Equals("True"))
            {
                System.Diagnostics.Debug.Write("Block Access to login page");
                return Redirect("~");
            }
            ViewBag.ReturnUrl = returnUrl;
            System.Diagnostics.Debug.Write("returnURL=" + returnUrl);
            if (returnUrl == null)
            {
                return View();
            }
            // 2nd layer check
            else if (returnUrl.Contains("Account"))
            {
                return Redirect("~");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            System.Diagnostics.Debug.WriteLine("Login Method" + returnUrl);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // int userID = Convert.ToInt32(User.Identity.GetUserID2());
            string userName = model.userName;
            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.UserName == userName && x.isApproved == 1 && x.isDeleted != 1));
            int userID = user.userID;
            string userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID && x.isDeleted != 1)).userTypeName;

            if (user.isLocked == 1)
            {
                ViewData["Error"] = "Your account has been locked out. Please contact your system administrator for details";
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            // var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            var result = await SignInManager.PasswordSignInAsync(model.userName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var checkLoggedin = Session["logged"] = true;
                    System.Diagnostics.Debug.Write("checkLoggedin=" + checkLoggedin.ToString());
                    System.Diagnostics.Debug.Write("Success");

                    if (userType == "Caregiver")
                    {
                        ViewData["Error"] = "Caregivers do not have permission to access PEAR web";
                        return View(model);
                    }

                    account.logAccountEntry(userID, "Successful login", "web login");
                    string token = account.getToken(userID);
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    System.Diagnostics.Debug.Write("LockedOut");
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    System.Diagnostics.Debug.Write("RequiresVerification");
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                case SignInStatus.Failure:
                default:
                    System.Diagnostics.Debug.Write("Invalid login attempt");
                    ModelState.AddModelError("", "Invalid login attempt.");

                    string remarks = "invalid login for: " + model.userName;
                    account.logAccountEntry(userID, "Invalid login", remarks);

                    return View(model);
            }
        }

        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        // GET: /Account/CheckUserType
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult CheckUserType()
        {
            List<SelectListItem> userTypeList = list.getUserTypeSelectListItem();
            ViewBag.UserType = new SelectList(userTypeList, "Value", "Text");

            return View();
        }

        // POST: /Account/CheckUserType
        [HttpPost]
        //[NoDirectAccess]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult CheckUserType(PreRegisterViewModel model)
        {
            int selectedUser = Convert.ToInt32(Request.Form["UserType"]);

            List<SelectListItem> userTypeList = list.getUserTypeSelectListItem();
            ViewBag.UserType = new SelectList(userTypeList, "Value", "Text", selectedUser.ToString());

            if (ModelState.IsValid)
            {
                DateTime date = DateTime.Now;

                if (selectedUser == 0)
                {
                    ViewBag.Error = "Select User Type";
                    return View(model);
                }
                else if (selectedUser != 5)
                {
                    TempData["userType"] = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == selectedUser)).userTypeName;
                    return RedirectToAction("Register", "Account");
                }
                else
                {
                    if (_context.Users.FirstOrDefault(x => x.Email == model.email && x.isApproved == 1 && x.isDeleted != 1) != null)
                    {
                        ViewBag.Error = "The email is already used!";
                        return View(model);
                    }
                    else if (_context.Users.FirstOrDefault(x => x.nric == model.nric && x.isApproved == 1 && x.isDeleted != 1) != null)
                    {
                        ViewBag.Error = "The NRIC is already taken!";
                        return View(model);
                    }
                    else if (shortcutMethod.checkNric(model.nric) == false)
                    {
                        ViewBag.Error = "Invalid NRIC!";
                        return View(model);
                    }
                    else
                    {
                        int userID = Convert.ToInt32(User.Identity.GetUserID2());
                        PatientGuardian patientGuardian = _context.PatientGuardian.FirstOrDefault(x => (x.guardianName == model.firstName && x.guardianNRIC == model.nric && x.guardianEmail == model.email && x.isInUse == 1 && x.isDeleted != 1));
                        PatientGuardian patientGuardian2 = _context.PatientGuardian.FirstOrDefault(x => (x.guardianName2 == model.firstName && x.guardianNRIC2 == model.nric && x.guardianEmail == model.email && x.isInUse == 1 && x.isDeleted != 1));

                        if (patientGuardian == null && patientGuardian2 == null)
                        {
                            ViewBag.Error = "Enter the correct guardian name, nric and email registered under the patient";
                        }
                        else
                        {
                            if (patientGuardian != null)
                            {
                                Patient patient1 = _context.Patients.SingleOrDefault(x => (x.patientGuardianID == patientGuardian.patientGuardianID && x.isApproved == 1 && x.isDeleted != 1));
                                if (patient1 == null)
                                    ViewBag.Error = "Enter the correct guardian name, nric and email registered under the patient";

                                else if (patient1.preferredName != model.patientName)
                                    ViewBag.Error = "Incorrect patient's preferred name";
                            }
                            else if (patientGuardian2 != null)
                            {
                                Patient patient2 = _context.Patients.SingleOrDefault(x => (x.patientGuardianID == patientGuardian2.patientGuardianID && x.isApproved == 1 && x.isDeleted != 1));
                                if (patient2 == null)
                                    ViewBag.Error = "Enter the correct guardian name, nric and email registered under the patient";

                                else if (patient2.preferredName != model.patientName)
                                    ViewBag.Error = "Incorrect patient's preferred name";
                            }
                            
                            if (ViewBag.Error == null)
                            {
                                TempData["userType"] = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == selectedUser)).userTypeName;
                                TempData["firstName"] = model.firstName;
                                TempData["nric"] = model.nric;
                                TempData["email"] = model.email;

                                return RedirectToAction("Register", "Account");
                            }

                            string logDesc = "Deny new user account creation";
                            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
                            string remarks = "Failed guardian account creation for " + model.firstName;

                            // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, string? deleteReason)
                            shortcutMethod.addLogAccount(userID, null, null, null, logDesc, logCategoryID, remarks, "AspNetUsers", "ALL", null, null, null, null);
                            return View(model);
                        }
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/Register
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult Register()
        {
            List<SelectListItem> secretQuestionList = list.getSecretQuestionList(1, true);
            ViewBag.SelectedQuestion = new SelectList(secretQuestionList, "Value", "Text");

            ViewBag.userType = TempData["userType"];
            ViewBag.firstName = TempData["firstName"];
            ViewBag.nric = TempData["nric"];
            ViewBag.email = TempData["email"];

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        //[NoDirectAccess]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.isAdmin)]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            int selectedQuestionID = Convert.ToInt32(Request.Form["SelectedQuestion"]);

            List<SelectListItem> secretQuestionList = list.getSecretQuestionList(1, true);
            ViewBag.SelectedQuestion = new SelectList(secretQuestionList, "Value", "Text", selectedQuestionID.ToString());

            ViewBag.userType = model.userType;
            ViewBag.firstName = model.firstName;
            ViewBag.nric = model.nric;
            ViewBag.email = model.email;

            if (ModelState.IsValid)
            {
                UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(_context);
                UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
                DateTime date = DateTime.Now;

                if (_context.Users.FirstOrDefault(x => (x.UserName == model.userName && x.isApproved == 1 && x.isDeleted != 1)) != null)
                {
                    ViewBag.Error = "This username is already taken!";
                    return View(model);
                }
                else if (_context.Users.FirstOrDefault(x => x.Email == model.email && x.isApproved == 1 && x.isDeleted != 1) != null)
                {
                    ViewBag.Error = "The email is already used!";
                    return View(model);
                }
                else if (_context.Users.FirstOrDefault(x => x.nric == model.nric && x.isApproved == 1 && x.isDeleted != 1) != null)
                {
                    ViewBag.Error = "The NRIC is already taken!";
                    return View(model);
                }
                else if (shortcutMethod.checkNric(model.nric) == false)
                {
                    ViewBag.Error = "Invalid NRIC!";
                    return View(model);
                }
                else if (selectedQuestionID == 0)
                {
                    ViewBag.Error = "Choose a Secret Question";
                    return View(model);
                }
                else
                {
                    int userID = Convert.ToInt32(User.Identity.GetUserID2());
                    string selectedQuestion = _context.ListSecretQuestion.SingleOrDefault(x => (x.list_secretQuestionID == selectedQuestionID && x.isDeleted != 1)).value;

                    ApplicationUser user = new ApplicationUser();

                    user.Email = model.email;
                    user.UserName = model.userName;
                    user.password = account.HashPassword(model.Password);
                    user.secretQuestion = selectedQuestion;
                    user.secretAnswer = account.HashPassword(model.secretAnswer);
                    user.nric = model.nric;
                    user.maskedNric = model.nric.Remove(1, 4).Insert(1, "xxxx");
                    user.preferredName = model.preferredName;
                    user.firstName = model.firstName;
                    user.lastName = model.lastName;
                    user.address = model.address;
                    user.DOB = model.DOB;
                    user.gender = model.gender;
                    user.userTypeID =  _context.UserTypes.SingleOrDefault(x => (x.userTypeName == model.userType)).userTypeID;
                    user.PhoneNumber = model.handphoneNo;
                    user.officeNo = model.officeNo;
                    user.isLocked = 0;
                    user.reason = null;
                    user.allowNotification = 1;
                    user.isApproved = 1;
                    user.isDeleted = 0;
                    user.CreateDateTime = date;
                    user.userID = _context.UserTables.Count() + 1;

                    string roleName = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID && x.isDeleted != 1)).userTypeName;

                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        result = await UserManager.AddToRoleAsync(user.Id, roleName);

                        //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        ApplicationUser savedUser = _context.Users.FirstOrDefault(x => (x.nric == model.nric && x.isApproved == 1 && x.isDeleted != 1));

                        User newUser = new User();
                        newUser.aspNetID = savedUser.Id;
                        newUser.token = null;
                        newUser.lastPasswordChanged = date;
                        newUser.loginTimeStamp = null;
                        _context.UserTables.Add(newUser);
                        _context.SaveChanges();

                        string userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == savedUser.userTypeID)).userTypeName;
                        if (userType == "Guardian")
                        {
                            PatientGuardian selectedPatientGuardian = _context.PatientGuardian.SingleOrDefault(x => (x.guardianName == savedUser.firstName && x.guardianNRIC == savedUser.nric && x.guardianContactNo == savedUser.PhoneNumber && x.guardianEmail == savedUser.Email && x.isInUse == 1 && x.isDeleted != 1));
                            if (selectedPatientGuardian != null)
                            {
                                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientGuardianID == selectedPatientGuardian.patientGuardianID && x.isApproved == 1 && x.isDeleted != 1));
                                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patient.patientID && x.isApproved == 1 && x.isDeleted != 1));
                                patientAllocation.guardianID = savedUser.userID;
                            }
                            else
                            {
                                PatientGuardian selectedPatientGuardian2 = _context.PatientGuardian.SingleOrDefault(x => (x.guardianName2 == savedUser.firstName && x.guardianNRIC2 == savedUser.nric && x.guardianContactNo2 == savedUser.PhoneNumber && x.guardianEmail2 == savedUser.Email && x.isInUse == 1 && x.isDeleted != 1));
                                if (selectedPatientGuardian != null)
                                {
                                    Patient patient = _context.Patients.SingleOrDefault(x => (x.patientGuardianID == selectedPatientGuardian2.patientGuardianID && x.isApproved == 1 && x.isDeleted != 1));
                                    PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patient.patientID && x.isApproved == 1 && x.isDeleted != 1));
                                    patientAllocation.guardian2ID = savedUser.userID;
                                }
                            }
                            _context.SaveChanges();
                        }

                        AlbumUser albumUser = new AlbumUser();
                        if (model.gender == "M")
                            albumUser.albumPath = "https://pear.fyp2017.com/Image/UsersAvatar/boy.png";
                        else
                            albumUser.albumPath = "https://pear.fyp2017.com/Image/UsersAvatar/girl.png";
                        albumUser.userID = savedUser.userID;
                        albumUser.isApproved = 1;
                        albumUser.isDeleted = 0;
                        albumUser.createDateTime = date;
                        _context.AlbumUser.Add(albumUser);
                        _context.SaveChanges();

                        string logDesc = "New user account";
                        int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
                        string remarks = "account created id: " + savedUser.userID;

                        // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
                        shortcutMethod.addLogAccount(userID, null, null, null, logDesc, logCategoryID, remarks, "AspNetUsers", "ALL", savedUser.userID, null, null, null);

                        TempData["Message"] = "Account has been created for " + user.preferredName;
                        TempData["Modal"] = "true";
                        return RedirectToAction("Index", "Account");
                    }
                    AddErrors(result);
                    ViewBag.Error = "result failed";
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }


            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            account.logAccountEntry(userID, "Successful logout", "web logout");

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session["logged"] = false;
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult Index()
        {
            var users = (from u in _context.Users
                         join ut in _context.UserTables on u.userID equals ut.userID
                         where u.isApproved == 1
                         select new UserInformationViewModel
                         {
                             Id = u.Id,
                             userID = u.userID,
                             preferredName = u.preferredName,
                             firstName = u.firstName + " " + u.lastName,
                             email = u.Email,
                             gender = u.gender,
                             isLocked = u.isLocked,
                             reason = u.reason,
                             userTypeID = u.userTypeID,
                             lastLogin = ut.loginTimeStamp,
                             isDeleted = u.isDeleted
                         }).ToList();

            foreach (var user in users)
            {
                user.noOfPatient = account.getPatientCount(user.userTypeID, user.userID);
            }

            ViewBag.Modal = TempData["Modal"];

            var viewModel = new AdminManageAccountViewModel()
            {
                UserInformation = users
            };

            return View(viewModel);
        }

        // GET: /Account/CreateAccount
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult CreateAccount()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var roleList = _context.UserTypes.OrderBy(x => (x.userTypeName));
            foreach (var role in roleList)
                list.Add(new SelectListItem() { Value = role.userTypeID.ToString(), Text = role.userTypeName });
            ViewBag.Roles = list;

            List<SelectListItem> list2 = new List<SelectListItem>();
            var secretQuestion = _context.ListSecretQuestion.ToList();
            foreach (var question in secretQuestion)
                list2.Add(new SelectListItem() { Value = question.value, Text = question.value });
            ViewBag.Question = list2;

            return View();
        }

        // PUT: /Account/CreateAccount
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult DeleteAccount(AdminManageAccountViewModel model)
        {
            ApplicationUser user = _context.Users.Single(x => (x.Id == model.id && x.isApproved == 1 && x.isDeleted != 1));

            if (user != null)
            {
                user.isDeleted = 1;
                user.reason = model.reason;
                _context.SaveChanges();

                int userInitID = Convert.ToInt32(User.Identity.GetUserID2());
                string logDesc = "Delete user account";
                int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
                string remarks = "account deleted id: " + user.userID;

                // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
                shortcutMethod.addLogAccount(userInitID, null, null, null, logDesc, logCategoryID, remarks, "AspNetUsers", "isDeleted,reason", user.userID, null, null, model.reason);

            }
            return RedirectToAction("Index", "Account");
        }

        // GET: /Account/UpdateUser
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult UpdateUser(string id)
        {
            var user = _context.Users.SingleOrDefault(x => (x.Id == id && x.isApproved == 1 && x.isDeleted != 1));

            var userType = _context.UserTypes.ToList();
            ViewBag.userType = new SelectList(userType, "userTypeID", "userTypeName", user.userTypeID);

            ViewData["Error"] = TempData["Error"];
            ViewData["Error1"] = TempData["Error1"];
            ViewBag.Modal = TempData["Modal"];

            ViewBag.Info = TempData["Info"];
            if (ViewBag.Info != "inactive")
                ViewBag.Info = "active";

            ViewBag.Img = TempData["Img"];
            if (ViewBag.Pass != "active")
                ViewBag.Pass = "inactive";

            bool lockBool = false;
            if (user.isLocked == 1)
                lockBool = true;

            return View(new AdminManageAccountViewModel() { User = user, lockBool = lockBool});
        }

        // PUT: /Account/UpdateUser
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult UpdateUser2(AdminManageAccountViewModel model)
        {
            ApplicationUser user = _context.Users.Single(x => (x.Id == model.id && x.isApproved == 1 && x.isDeleted != 1));
            int userTypeID = Convert.ToInt32(Request.Form["userType"]);

            var userType = _context.UserTypes.ToList();
            ViewBag.userType = new SelectList(userType, "userTypeID", "userTypeName", userTypeID);
            ViewBag.Info = "active";
            ViewBag.Pass = "inactive";

            if (ModelState.IsValid)
            {
                int userID = _context.Users.Single(x => (x.Id == model.id && x.isApproved == 1 && x.isDeleted != 1)).userID;
                int isLocked = 0;

                if (model.lockBool == true)
                {
                    isLocked = 1;
                }

                string oldUserType = _context.UserTypes.Single(x => x.userTypeID == user.userTypeID).userTypeName;
                string newUserType = _context.UserTypes.Single(x => x.userTypeID == userTypeID).userTypeName;

                if (user.userTypeID != userTypeID)
                {
                    UserManager.RemoveFromRole(user.Id, oldUserType);
                    UserManager.AddToRole(user.Id, newUserType);
                }

                int userInitID = Convert.ToInt32(User.Identity.GetUserID2());
                //account.updateAccount(int userInitID, int userID, int userTypeID, string preferredName, string firstName, string lastName, string email, string address, string handphoneNo, string? officeNo, int isLocked)
                string result = account.updateAccount(userInitID, userID, userTypeID, newUserType, model.preferredName, model.firstName, model.lastName, model.Email, model.address, model.handphoneNo, model.officeNo, null, isLocked, model.reason);

                if (result == "Update Successfully.")
                {
                    TempData["Message"] = "Updated personal info for user: " + user.preferredName;
                    TempData["Modal"] = "true";
                }
                else
                {
                    TempData["Message"] = "No changes are made for user: " + user.preferredName;
                    TempData["Modal"] = "true";
                }

                //TempData["Message"] = "<script>alert('" + message + "');</script>";
                return RedirectToAction("UpdateUser", "Account", new { id = model.id });
            }
            // If we got this far, something failed, redisplay form
            return View("UpdateUser", new AdminManageAccountViewModel { User = user});
        }

        public static class StringCipher
        {
            // This constant is used to determine the keysize of the encryption algorithm in bits.
            // We divide this by 8 within the code below to get the equivalent number of bytes.
            private const int Keysize = 256;

            // This constant determines the number of iterations for the password bytes generation function.
            private const int DerivationIterations = 10;

            public static string Encrypt(string plainText, string passPhrase)
            {
                // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
                // so that the same Salt and IV values can be used when decrypting.  
                var saltStringBytes = Generate256BitsOfRandomEntropy();
                var ivStringBytes = Generate256BitsOfRandomEntropy();
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    var keyBytes = password.GetBytes(Keysize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                                {
                                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                    cryptoStream.FlushFinalBlock();
                                    // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                    var cipherTextBytes = saltStringBytes;
                                    cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                    cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Convert.ToBase64String(cipherTextBytes);
                                }
                            }
                        }
                    }
                }
            }

            public static string Decrypt(string cipherText, string passPhrase)
            {
                // Get the complete stream of bytes that represent:
                // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
                var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
                // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
                var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
                // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
                var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
                // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
                var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    var keyBytes = password.GetBytes(Keysize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    var plainTextBytes = new byte[cipherTextBytes.Length];
                                    var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }

            private static byte[] Generate256BitsOfRandomEntropy()
            {
                var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
                using (var rngCsp = new RNGCryptoServiceProvider())
                {
                    // Fill the array with cryptographically secure random bytes.
                    rngCsp.GetBytes(randomBytes);
                }
                return randomBytes;
            }
        }
        
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult SendResetEmail(AdminManageAccountViewModel model)
        {
            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.Id == model.id && x.isApproved == 1 && x.isDeleted != 1));
            
            SmtpMail oMail = new SmtpMail("TryIt");
            SmtpClient oSmtp = new SmtpClient();
            
            oMail.From = "pearservice@fyp2017.com";
            oMail.To = user.Email;
            oMail.Subject = "PEAR Account Password Reset";

            string unencryptedString = user.Id + "%" + DateTime.Now.ToString("yyyy/dd/MM HH:mm:ss"); 
            string encryptedString = StringCipher.Encrypt(unencryptedString, "Admin!23");

            oMail.HtmlBody = "<h3><b>Hi, " + user.firstName + " " + user.lastName + "!</b></h3><br/>" +
                            "<h3>There was a request to change your password.</h3><br/>" +
                            "If you did not make this request, just ignore this email. Otherwise, please click the link below to change your password. The link will expire in 5 minutes.<br/><br/>" +
                            "<a href=\"https://pear.fyp2017.com/Account/Reset?code=" + encryptedString + "\" class=\"button\" style=\"\">Change Password</a>";

            SmtpServer oServer = new SmtpServer("fyp2017.com");
            oServer.Port = 587;
            oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
            oServer.User = "pearservice@fyp2017.com";
            oServer.Password = "6Tnl78v^";

            try
            {
                oSmtp.SendMail(oServer, oMail);
                TempData["Message"] = "Sent password reset link to " + user.preferredName + "'s email.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex;
            }

            TempData["Modal"] = "true";
            return RedirectToAction("UpdateUser", "Account", new { id = model.id });
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetLink(string message, string errorMessage)
        {
            List<SelectListItem> optionList = new List<SelectListItem>();
            optionList.Add(new SelectListItem() { Value = "Reset using email", Text = "Reset using email" });
            optionList.Add(new SelectListItem() { Value = "Reset using username and secret question", Text = "Reset using username and secret question" });
            ViewBag.Option = new SelectList(optionList, "Value", "Text");

            List<SelectListItem> secretQuestionList = list.getSecretQuestionList(1, true);
            ViewBag.SelectedQuestion = new SelectList(secretQuestionList, "Value", "Text");

            ViewData["Error1"] = message;
            ViewData["Error2"] = errorMessage;

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetLink(ResetLinkViewModel model)
        {
            int secretQuestionID = Convert.ToInt32(Request.Form["SelectedQuestion"]);

            ApplicationUser user = new ApplicationUser();
            string message = null;
            string errorMessage = null;

            string option = Request.Form["Option"];
            if (option == "Reset using email")
            {
                user = _context.Users.SingleOrDefault(x => (x.Email == model.email && x.isDeleted != 1));
                if (user != null)
                    message = "Sent password reset link to " + model.email;
            }
            else if (option == "Reset using username and secret question")
            {
                user = _context.Users.SingleOrDefault(x => (x.UserName == model.username));
                string secretQuestion = _context.ListSecretQuestion.SingleOrDefault(x => (x.list_secretQuestionID == secretQuestionID && x.isDeleted != 1)).value;

                if (user.secretQuestion == secretQuestion && account.VerifyHashedPassword(user.secretAnswer, model.secretAnswer) == true)
                {
                    int count = 0;
                    for (int i = 0; i < user.Email.Length; i++)
                        if (user.Email.ElementAt(i) != '@')
                            count++;
                        else
                            break;

                    string maskedEmail = user.Email.Remove(4, count-4).Insert(4, string.Join("", Enumerable.Repeat("x", count - 4)));
                    message = "Sent password reset link to " + maskedEmail;
                }
                else
                    user = null;
            }
            
            if (user != null)
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                SmtpClient oSmtp = new SmtpClient();

                oMail.From = "pearservice@fyp2017.com";
                oMail.To = user.Email;
                oMail.Subject = "PEAR Account Password Reset";

                string unencryptedString = user.Id + "%" + DateTime.Now.ToString("yyyy/dd/MM HH:mm:ss");
                string encryptedString = StringCipher.Encrypt(unencryptedString, "Admin!23");

                oMail.HtmlBody = "<h3><b>Hi, " + user.firstName + " " + user.lastName + "!</b></h3><br/>" +
                                "<h3>There was a request to change your password.</h3><br/>" +
                                "If you did not make this request, just ignore this email. Otherwise, please click the link below to change your password. The link will expire in 5 minutes.<br/><br/>" +
                                "<a href=\"https://pear.fyp2017.com/Account/Reset?code=" + encryptedString + "\" class=\"button\" style=\"\">Change Password</a>";

                SmtpServer oServer = new SmtpServer("fyp2017.com");
                oServer.Port = 587;
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                oServer.User = "pearservice@fyp2017.com";
                oServer.Password = "6Tnl78v^";

                try
                {
                    oSmtp.SendMail(oServer, oMail);
                }
                catch (Exception ex)
                {
                    message = "Error: " + ex;
                }
            }
            else
                errorMessage = "Wrong credentials";

            return RedirectToAction("ResetLink", "Account", new { message = message, errorMessage = errorMessage });
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Reset(string code)
        {
            string cleanString = code.Replace(" ", "+");
            string decryptedString = StringCipher.Decrypt(cleanString, "Admin!23");

            string[] tokens = decryptedString.Split('%');
            string id = tokens[0];
            DateTime date = DateTime.ParseExact(tokens[1], "yyyy/dd/MM HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime expiredDate = date.AddMinutes(5);

            DateTime now = DateTime.Now;
            if (DateTime.Compare(expiredDate, now) > 0)
            {
                ApplicationUser user = _context.Users.SingleOrDefault(x => (x.Id == id && x.isApproved == 1 && x.isDeleted != 1));
                UserResetPasswordViewModel model = new UserResetPasswordViewModel { id = id, code = code };
                User user2 = _context.UserTables.SingleOrDefault(x => (x.userID == user.userID));

                if (DateTime.Compare(user2.lastPasswordChanged, date) < 0)
                    return View(model);
            }

            ViewBag.Title = "Password reset link expired.";
            return View("~/Views/Account/ExternalLoginFailure.cshtml");
        }

        //[NoDirectAccess]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Reset(UserResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = _context.Users.SingleOrDefault(x => (x.Id == model.id && x.isApproved == 1 && x.isDeleted != 1));
                bool result = await setPassword(user, null, model.Password, user.userID, "Reset user password");

                if (result == true)
                {
                    ViewData["Error1"] = "Password has been reset. You may login now.";
                    UserResetPasswordViewModel model2 = new UserResetPasswordViewModel { id = null };
                    return View(model2);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public async Task<ActionResult> ResetPw(AdminManageAccountViewModel model)
        {
            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.Id == model.id && x.isApproved == 1 && x.isDeleted != 1));
            TempData["Info"] = "inactive";
            TempData["Pass"] = "active";

            //var userType = _context.UserTypes.ToList();
            //ViewBag.userType = new SelectList(userType, "userTypeID", "userTypeName", user.userTypeID);

            if (ModelState.IsValid)
            {
                int userInitID = Convert.ToInt32(User.Identity.GetUserID2());

                //setPassword(ApplicationUser user, string secretAnswer, string newPassword, int userInitID)
                bool result = await setPassword(user, model.secretAnswer, model.Password, userInitID, "Update user password");

                if (result == true)
                {
                    TempData["Message"] = "Changed password for user: " + user.preferredName;
                    TempData["Modal"] = "true";
                    return RedirectToAction("UpdateUser", "Account", new { id = model.id });
                }
                else
                {
                    TempData["Error"] = "Invalid secret answer!";
                    string logDesc = "Invalid secret answer";
                    int logCategoryID = _context.LogCategories.SingleOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
                    string remarks = "invalid secret answer id: " + user.userID;
                    // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
                    shortcutMethod.addLogAccount(userInitID, null, null, null, logDesc, logCategoryID, remarks, "AspNetUsers", "Password", user.userID, null, null, null);
                    return RedirectToAction("UpdateUser", "Account", new { id = model.id });
                }
            }
            // If we got this far, something failed, redisplay form
            TempData["Error"] = "The password and confirmation password do not match.";
            return RedirectToAction("UpdateUser", "Account", new { id = model.id });
        }

        public async Task<bool> setPassword(ApplicationUser user, string secretAnswer, string newPassword, int userInitID, string logDesc)
        {
            UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(_context);
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);

            if (secretAnswer == null || (account.VerifyHashedPassword(user.secretAnswer, secretAnswer) == true))
            {
                string hashedNewPassword = UserManager.PasswordHasher.HashPassword(newPassword);

                await store.SetPasswordHashAsync(user, hashedNewPassword);
                await store.UpdateAsync(user);

                user.password = account.HashPassword(newPassword);
                _context.SaveChanges();

                int logCategoryID = _context.LogCategories.SingleOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
                string remarks = "password changed id " + user.userID;

                // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
                shortcutMethod.addLogAccount(userInitID, null, null, null, logDesc, logCategoryID, remarks, "AspNetUsers", "Password", user.userID, null, null, null);

                return true;
            }
            else
            {
                return false;
            }
        }

        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult GenerateSchedule()
        {
            return View();
        }

        // GET: /Account/Settings
        [Authorize]
        //[NoDirectAccess]
        public ActionResult Settings(string id)
        {
            ViewData["Error1"] = TempData["Error1"];
            ViewData["Error2"] = TempData["Error2"];
            ViewData["Error3"] = TempData["Error3"];

            ViewBag.Modal = TempData["Modal"];

            ViewBag.Info = TempData["Info"];
            if (ViewBag.Info != "inactive")
                ViewBag.Info = "active";

            ViewBag.Pass = TempData["Pass"];
            if (ViewBag.Pass != "active")
                ViewBag.Pass = "inactive";

            ViewBag.Img = TempData["Img"];
            if (ViewBag.Img != "active")
                ViewBag.Img = "inactive";

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.Id == id && x.isApproved == 1 && x.isDeleted != 1));
            string userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID && x.isDeleted != 1)).userTypeName;

            List<SelectListItem> secretQuestionList = list.getSecretQuestionList(1, true);
            secretQuestionList[0].Text = "-- You may choose a new Question --";
            ViewBag.SelectedQuestion = new SelectList(secretQuestionList, "Value", "Text");

            string date = shortcutMethod.leadingZero(user.DOB.Day.ToString()) + "/" + shortcutMethod.leadingZero(user.DOB.Month.ToString()) + "/" + user.DOB.Year.ToString();

            string gender = null;
            if (user.gender == "M")
                gender = "Male";
            else if (user.gender == "F")
                gender = "Female";

            bool notificationBool = false;
            if (user.allowNotification == 1)
                notificationBool = true;

            string albumPath = _context.AlbumUser.SingleOrDefault(x => (x.userID == user.userID && x.isApproved == 1 && x.isDeleted != 1)).albumPath;
            DateTime? lastLoginDate = _context.UserTables.SingleOrDefault(x => (x.userID == user.userID)).loginTimeStamp;
            string lastLogin = lastLoginDate == null ? null : ((DateTime)lastLoginDate).ToString("dddd dd/MM/yyyy HH:mm:ss");

            return View(new UserAccountViewModel() { User = user, userType = userType, date = date, gender = gender, imageUrl = albumPath, notificationBool = notificationBool, lastLogin = lastLogin });
        }

        // PUT: /Account/UpdateSettings
        [Authorize]
        //[NoDirectAccess]
        public ActionResult UpdateSettings(UserAccountViewModel model)
        {
            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.Id == model.id && x.isApproved == 1 && x.isDeleted != 1));
            string userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID && x.isDeleted != 1)).userTypeName;
            ViewBag.Info = "active";
            ViewBag.Pass = "inactive";
            ViewBag.Img = "inactive";

            string date = shortcutMethod.leadingZero(user.DOB.Day.ToString()) + "/" + shortcutMethod.leadingZero(user.DOB.Month.ToString()) + "/" + user.DOB.Year.ToString();

            string gender = null;
            if (user.gender == "M")
                gender = "Male";
            else if (user.gender == "F")
                gender = "Female";

            List<SelectListItem> secretQuestionList = list.getSecretQuestionList(1, true);
            ViewBag.SelectedQuestion = new SelectList(secretQuestionList, "Value", "Text");

            if (ModelState.IsValid)
            {
                int userInitID = Convert.ToInt32(User.Identity.GetUserID2());

                int allowNotification = 1;
                if (model.notificationBool == false)
                    allowNotification = 0;

                //account.updateAccount(int userInitID, int userID, int userTypeID, string preferredName, string firstName, string lastName, string email, string address, string handphoneNo, string? officeNo, int? allowNotification int? isLocked)
                string result = account.updateAccount(userInitID, user.userID, null, userType, model.preferredName, null, null, model.Email, model.address, model.handphoneNo, model.officeNo, allowNotification, null, null);

                if (result == "Update Successfully.")
                {
                    TempData["Message"] = "Updated Information!";
                    TempData["Modal"] = "true";
                    return RedirectToAction("Settings", "Account", new { id = model.id });
                }
                else
                {
                    TempData["Error1"] = "No changes are made!";
                    return RedirectToAction("Settings", "Account", new { id = model.id });
                }
            }
            // If we got this far, something failed, redisplay form
            return View("Settings", new UserAccountViewModel { User = user, userType = userType, date = date, gender = gender });
        }

        // PUT: /Account/ChangePassword
        [Authorize]
        //[NoDirectAccess]
        public async Task<ActionResult> ChangePassword(UserAccountViewModel model)
        {
            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.Id == model.id && x.isApproved == 1 && x.isDeleted != 1));
            TempData["Info"] = "inactive";
            TempData["Pass"] = "active";
            TempData["Img"] = "inactive";

            List<SelectListItem> secretQuestionList = list.getSecretQuestionList(1, true);
            ViewBag.SelectedQuestion = new SelectList(secretQuestionList, "Value", "Text");

            if (ModelState.IsValid)
            {
                int userInitID = Convert.ToInt32(User.Identity.GetUserID2());
                int selectedQuestionID = Convert.ToInt32(Request.Form["SelectedQuestion"]);

                if (selectedQuestionID != 0 && model.secretAnswer != null)
                {
                    string newSecretQuestion = _context.ListSecretQuestion.SingleOrDefault(x => (x.list_secretQuestionID == selectedQuestionID && x.isDeleted != 1)).value;
                    // account.updateSecretQuestion(int userID, string secretQuestion, string secretAnswer)
                    account.updateSecretQuestion(user.userID, newSecretQuestion, model.secretAnswer);

                    if (model.Password == null || model.ConfirmPassword == null)
                    {
                        TempData["Message"] = "Changed Secret Answer!";
                        TempData["Modal"] = "true";
                        return RedirectToAction("Settings", "Account", new { id = model.id });
                    }
                }

                if (model.Password != null && model.ConfirmPassword != null)
                {
                    if (account.VerifyHashedPassword(user.password, model.currentPassword) == false)
                    {
                        TempData["Error2"] = "Your current password is wrong";
                        return RedirectToAction("Settings", "Account", new { id = model.id });
                    }

                    //setPassword(ApplicationUser user, string secretAnswer, string newPassword, int userInitID)
                    bool result = await setPassword(user, null, model.Password, userInitID, "Update user password");

                    if (result == true)
                    {
                        TempData["Message"] = "Changed Password!";
                        TempData["Modal"] = "true";
                        return RedirectToAction("Settings", "Account", new { id = model.id });
                    }
                    else
                    {
                        TempData["Error2"] = "Unknown Error";
                        string logDesc = "Encountered error";
                        int logCategoryID = _context.LogCategories.SingleOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
                        string remarks = "error in changing password id: " + user.userID;
                        // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
                        shortcutMethod.addLogAccount(userInitID, null, null, null, logDesc, logCategoryID, remarks, null, null, user.userID, null, null, null);
                        return RedirectToAction("Settings", "Account", new { id = model.id });
                    }
                }
                TempData["Error2"] = "No changes are made!";
                return RedirectToAction("Settings", "Account", new { id = model.id });
            }
            // If we got this far, something failed, redisplay form
            TempData["Error2"] = "The password and confirmation password do not match.";
            return RedirectToAction("Settings", "Account", new { id = model.id });
        }

        [HttpPost]
        [Authorize]
        //[NoDirectAccess]
        public ActionResult uploadImage(HttpPostedFileBase file, UserAccountViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["Pass"] = "inactive";
            TempData["Img"] = "active";

            try
            {
                if (file != null)
                {
                    int userID = Convert.ToInt32(User.Identity.GetUserID2());
                    ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID && x.isApproved == 1 && x.isDeleted != 1));
                    string firstName = user.firstName;
                    string lastName = user.lastName;
                    string maskedNric = user.maskedNric;
                    
                    string result = account.uploadProfileImage(Server, file, userID, firstName, lastName, maskedNric);
                    
                    /*
                    if (result == "Extension Error")
                    {
                        TempData["Error3"] = "Please Upload image of type .jpg, .gif, .png.";
                        return RedirectToAction("Settings", "Account", new { id = model.id });
                    }
                    else if (result == "File Size Error")
                    {
                        TempData["Error3"] = "Please Upload a file upto 1 mb.";
                        return RedirectToAction("Settings", "Account", new { id = model.id });
                    }*/

                    if (result == null)
                    {
                        TempData["Error3"] = "Error in uploading to cloudinary";
                        return RedirectToAction("Settings", "Account", new { id = model.id });
                    }

                    TempData["Message"] = "Image Uploaded Successfully";
                    TempData["Modal"] = "true";
                    return RedirectToAction("Settings", "Account", new { id = model.id });

                    /*
                    string firstName = User.Identity.GetUserFirstName();

                    int MaxContentLength = 1024 * 1024 * 1;
                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                    var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                    var extension = ext.ToLower();

                    if (!AllowedFileExtensions.Contains(extension))
                    {
                        TempData["Error3"] = "Please Upload image of type .jpg, .gif, .png.";
                        return RedirectToAction("Settings", "Account", new { id = model.id });
                    }
                    else if (file.ContentLength > MaxContentLength)
                    {
                        TempData["Error3"] = "Please Upload a file upto 1 mb.";
                        return RedirectToAction("Settings", "Account", new { id = model.id });
                    }
                    else
                    {
                        path = "~/Image/User/ProfileImages/";
                        DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(path));
                        FileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(Server.MapPath(path + firstName + extension));

                        int index = 1;
                        while (System.IO.File.Exists(filePath))
                        {
                            addOns = "(" + index.ToString() + ")";
                            filePath = Path.Combine(Server.MapPath(path + addOns + extension));
                            index++;
                        }
                        file.SaveAs(filePath);
                    }

                    string albumPath = "/Image/User/ProfileImages/" + firstName + addOns + extension;
                    int userID = Convert.ToInt32(User.Identity.GetUserID2());
                    album.addToAlbumUser(userID, albumPath);

                    message = "Image Uploaded Successfully!";
                    TempData["Message"] = "<script>alert('" + message + "');</script>";
                    return RedirectToAction("Settings", "Account", new { id = model.id });*/
                }
                TempData["Error3"] = "No file chosen!";
                return RedirectToAction("Settings", "Account", new { id = model.id });
            }
            catch (Exception ex)
            {
                TempData["Error3"] = ex.Message;
                return RedirectToAction("Settings", "Account", new { id = model.id });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }
            _context.Dispose();
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            /*
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }*/
            return RedirectToAction("Index", "Home");
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

        // GET: /Account/ViewActivityLog
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isSupervisor + "," + RoleName.isAdmin)]
        public ActionResult ViewActivityLog()
        {
            List<Log> logs = _context.Logs.ToList();
            List<PatientAllocation> patientAllocations = _context.PatientAllocations.ToList();
            List<ApplicationUser> users = _context.Users.ToList();

            List<ViewActivityLogViewModel> model = new List<ViewActivityLogViewModel>();

            if (logs.Count > 0)
            {
                int logCount = logs[logs.Count - 1].logID;

                if (logCount < 500)
                    logCount = 0;
                else if (logCount > 500)
                    logCount -= 500;

                model = (from log in logs
                         join userInit in users on log.userIDInit equals userInit.userID
                         join userApprove in users on log.userIDApproved equals userApprove.userID into userCheck

                         join patientAllocation in patientAllocations
                             on log.patientAllocationID equals patientAllocation.patientID into patientCheck

                         from userApprove in userCheck.DefaultIfEmpty()
                         from patientAllocation in patientCheck.DefaultIfEmpty()
                         where log.isDeleted != 1 && log.logID > logCount
                         orderby log.logID descending

                         select new { log, patientAllocation, userInit, userApprove })
                    .Select(x => new ViewActivityLogViewModel
                    {
                        logID = x.log.logID,
                        oldlogData = x.log.oldLogData,
                        logData = x.log.logData,
                        logDesc = x.log.logDesc,
                        patientAllocationID = x.patientAllocation?.patientID,
                        patientPreferredName = x.patientAllocation?.Patient.preferredName,
                        userIDInit = x.log.userIDInit,
                        userIDInitType = x.userInit.UserType.userTypeName,
                        userIDInitPreferredName = x.userInit.preferredName,
                        userIDApproved = x.userApprove?.userID,
                        userIDApprovedType = x.userApprove?.UserType.userTypeName,
                        userIDApprovedPreferredName = x.userApprove?.preferredName,
                        remarks = x.log.remarks,
                        tableAffected = x.log.tableAffected,
                        isApproved = x.log.approved,
                        isRejected = x.log.reject,
                        rejectReason = x.log.rejectReason,
                        createDateTime = x.log.createDateTime,
                        logOldValue = x.log.logOldValue,
                        logNewValue = x.log.logNewValue
                    }).OrderByDescending(x => x.createDateTime).ToList();

                int num = 1;
                for (int i = 0; i < model.Count; i++)
                {
                    model[i].index = num++;
                }
            }
            return View(model);
        }

        // GET: /Account/ViewAccountLog
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult ViewAccountLog()
        {
            List<LogAccount> logAccount = _context.LogAccount.ToList();
            List<ApplicationUser> users = _context.Users.ToList();

            List<ViewAccountLogViewModel> model = new List<ViewAccountLogViewModel>();

            if (logAccount.Count > 0)
            {
                int logAccountCount = logAccount[logAccount.Count - 1].logAccountID;

                if (logAccountCount < 500)
                    logAccountCount = 0;
                else if (logAccountCount > 500)
                    logAccountCount -= 500;

                model = (from log in logAccount
                         join user in users
                              on log.userID equals user.userID into userCheck

                         from user in userCheck.DefaultIfEmpty()
                         where log.isDeleted != 1 && log.logAccountID > logAccountCount
                         orderby log.logID descending

                         select new { log, user })
                    .Select(x => new ViewAccountLogViewModel
                    {
                        logAccountID = x.log.logAccountID,
                        userID = x.user?.userID,
                        userIDType = x.user?.UserType.userTypeName,
                        userIDPreferredName = x.user?.preferredName,
                        logID = x.log.logID,
                        oldlogData = x.log.oldLogData,
                        logData = x.log.logData,
                        logDesc = x.log.logDesc,
                        remarks = x.log.remarks,
                        tableAffected = x.log.tableAffected,
                        createDateTime = x.log.createDateTime,
                        logOldValue = x.log.logOldValue,
                        logNewValue = x.log.logNewValue,
                    }).ToList();

                int num = 1;
                for (int i = model.Count() - 1; i >= 0; i--)
                {
                    model[i].index = num++;
                }
            }
            return View(model);
        }

        // GET: /Account/ViewApproveRejectLog
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isSupervisor + "," + RoleName.isAdmin)]
        public ActionResult ViewApproveRejectLog()
        {
            List<LogApproveReject> logApproveReject = _context.LogApproveReject.ToList();
            List<Log> logs = _context.Logs.ToList();
            List<ApplicationUser> users = _context.Users.ToList();

            List<ViewApproveRejectLogViewModel> model = new List<ViewApproveRejectLogViewModel>();

            if (logApproveReject.Count > 0)
            {
                int logApproveRejectCount = logApproveReject[logApproveReject.Count - 1].approveRejectID;

                if (logApproveRejectCount < 500)
                    logApproveRejectCount = 0;
                else if (logApproveRejectCount > 500)
                    logApproveRejectCount -= 500;

                model = (from logStatus in logApproveReject
                         join log in logs on logStatus.logID equals log.logID
                         join userIDInit in users on logStatus.userIDInit equals userIDInit.userID
                         join userIDRespond in users on logStatus.userIDReceived equals userIDRespond.userID

                         where log.isDeleted != 1 && logStatus.approveRejectID > logApproveRejectCount
                         orderby logStatus.logID descending

                         select new { logStatus, log, userIDInit, userIDRespond })
                    .Select(x => new ViewApproveRejectLogViewModel
                    {
                        approveRejectID = x.logStatus.approveRejectID,
                        userIDInit = x.userIDInit.userID,
                        userIDInitPreferredName = x.userIDInit.preferredName,
                        userIDReceived = x.userIDRespond.userID,
                        userIDReceivedPreferredName = x.userIDRespond.preferredName,
                        logID = x.log.logID,
                        logDesc = x.log.logDesc,
                        tableAffected = x.log.tableAffected,
                        logOldValue = x.log.logOldValue,
                        logNewValue = x.log.logNewValue,
                        approve = x.logStatus.approve,
                        reject = x.logStatus.reject,
                        createDateTime = x.logStatus.createDateTime,

                    }).ToList();

                int num = 1;
                for (int i = model.Count() - 1; i >= 0; i--)
                {
                    model[i].index = num++;
                }
            }
            return View(model);
        }

        // GET: /Account/ViewDropList
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult ViewDropList()
        {
            DropListViewModel model = list.getDropListItem();
            return View(model);
        }

        // GET: /Account/ViewSelectedList
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult ViewSelectedList(string name, string view)
        {
            List<SelectListItem> checkedList = null;
            List<SelectListItem> uncheckedList = null;

            ViewBag.Modal = TempData["Modal"];

            SelectedListViewModel model = new SelectedListViewModel();
            model.name = name;
            model.view = view;
            switch (name)
            {
                case "Album":
                    checkedList = list.getAlbumList(1, false, false);
                    uncheckedList = list.getAlbumList(0, false, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Allergy":
                    checkedList = list.getAllergyList(1, false, false);
                    uncheckedList = list.getAllergyList(0, false, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Country":
                    checkedList = list.getCountryList(1);
                    uncheckedList = list.getCountryList(0);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Diet":
                    checkedList = list.getDietList(1, false, false);
                    uncheckedList = list.getDietList(0, false, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Dislike":
                    checkedList = list.getDislikeList(1, false, false);
                    uncheckedList = list.getDislikeList(0, false, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Education":
                    checkedList = list.getEducationList(1, false);
                    uncheckedList = list.getEducationList(0, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Habit":
                    checkedList = list.getHabitList(1, false, false);
                    uncheckedList = list.getHabitList(0, false, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Hobby":
                    checkedList = list.getHobbyList(1, false, false);
                    uncheckedList = list.getHobbyList(0, false, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Language":
                    checkedList = list.getLanguageList(1, false);
                    uncheckedList = list.getLanguageList(0, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Like":
                    checkedList = list.getLikeList(1, false, false);
                    uncheckedList = list.getLikeList(0, false, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Live With":
                    checkedList = list.getLiveWithList(1, false);
                    uncheckedList = list.getLiveWithList(0, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Mobility":
                    checkedList = list.getMobilityList(1, false, false);
                    uncheckedList = list.getMobilityList(0, false, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Occupation":
                    checkedList = list.getOccupationList(1, false);
                    uncheckedList = list.getOccupationList(0, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Pet":
                    checkedList = list.getPetList(1, false, false);
                    uncheckedList = list.getPetList(0, false, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Prescription":
                    checkedList = list.getPrescriptionList(1, false, false);
                    uncheckedList = list.getPrescriptionList(0, false, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Problem Log":
                    checkedList = list.getProblemLogList(1, false);
                    uncheckedList = list.getProblemLogList(0, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Relationship":
                    checkedList = list.getRelationshipList(1, false);
                    uncheckedList = list.getRelationshipList(0, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Religion":
                    checkedList = list.getReligionList(1, false);
                    uncheckedList = list.getReligionList(0, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
                case "Secret Question":
                    checkedList = list.getSecretQuestionList(1, false);
                    uncheckedList = list.getSecretQuestionList(0, false);
                    ViewBag.CheckedList = new SelectList(checkedList, "Value", "Text");
                    ViewBag.UncheckedList = new SelectList(uncheckedList, "Text", "Text");
                    model.check = checkedList.Count;
                    model.notCheck = uncheckedList.Count;
                    break;
            }

            return View(model);
        }

        // POST: /Account/AddSelectedList
        [HttpPost]
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult AddSelectedList(SelectedListViewModel model)
        {
            if (ModelState.IsValid)
            {
                int userID = Convert.ToInt32(User.Identity.GetUserID2());
                string text = model.text;
                string name = model.name;
                bool exist = true;

                switch (name)
                {
                    case "Album":
                        AlbumCategory album = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatName.ToLower() == text.ToLower() && x.isApproved == 1 && x.isDeleted != 1));
                        if (album == null)
                        {
                            exist = false;
                            list.addAlbum(userID, text, 1);
                        }
                        break;
                    case "Allergy":
                        List_Allergy allergy = _context.ListAllergy.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (allergy == null)
                        {
                            exist = false;
                            list.addAllergy(userID, text, 1);
                        }
                        break;
                    case "Country":
                        List_Country country = _context.ListCountries.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (country == null)
                        {
                            exist = false;
                            list.addCountry(userID, text, 1);
                        }
                        break;
                    case "Diet":
                        List_Diet diet = _context.ListDiets.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (diet == null)
                        {
                            exist = false;
                            list.addDiet(userID, text, 1);
                        }
                        break;
                    case "Dislike":
                        List_Dislike dislike = _context.ListDislikes.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (dislike == null)
                        {
                            exist = false;
                            list.addDislike(userID, text, 1);
                        }
                        break;
                    case "Education":
                        List_Education education = _context.ListEducations.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (education == null)
                        {
                            exist = false;
                            list.addEducation(userID, text, 1);
                        }
                        break;
                    case "Habit":
                        List_Habit habit = _context.ListHabits.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (habit == null)
                        {
                            exist = false;
                            list.addHabit(userID, text, 1);
                        }
                        break;
                    case "Hobby":
                        List_Hobby hobby = _context.ListHobbies.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (hobby == null)
                        {
                            exist = false;
                            list.addHobby(userID, text, 1);
                        }
                        break;
                    case "Language":
                        List_Language language = _context.ListLanguages.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (language == null)
                        {
                            exist = false;
                            list.addLanguage(userID, text, 1);
                        }
                        break;
                    case "Like":
                        List_Like like = _context.ListLikes.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (like == null)
                        {
                            exist = false;
                            list.addLike(userID, text, 1);
                        }
                        break;
                    case "Live With":
                        List_LiveWith live = _context.ListLiveWiths.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (live == null)
                        {
                            exist = false;
                            list.addLiveWith(userID, text, 1);
                        }
                        break;
                    case "Mobility":
                        List_Mobility mobility = _context.ListMobility.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (mobility == null)
                        {
                            exist = false;
                            list.addMobility(userID, text, 1);
                        }
                        break;
                    case "Occupation":
                        List_Occupation occupation = _context.ListOccupations.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (occupation == null)
                        {
                            exist = false;
                            list.addOccupation(userID, text, 1);
                        }
                        break;
                    case "Pet":
                        List_Pet pet = _context.ListPets.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (pet == null)
                        {
                            exist = false;
                            list.addPet(userID, text, 1);
                        }
                        break;
                    case "Prescription":
                        List_Prescription prescription = _context.ListPrescriptions.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (prescription == null)
                        {
                            exist = false;
                            list.addPrescription(userID, text, 1);
                        }
                        break;
                    case "Problem Log":
                        List_ProblemLog problem = _context.ListProblemLogs.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (problem == null)
                        {
                            exist = false;
                            list.addProblemLog(userID, text, 1);
                        }
                        break;
                    case "Relationship":
                        List_Relationship relationship = _context.ListRelationships.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (relationship == null)
                        {
                            exist = false;
                            list.addRelationship(userID, text, 1);
                        }
                        break;
                    case "Religion":
                        List_Religion religion = _context.ListReligions.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 0 && x.isDeleted != 1));
                        if (religion == null)
                        {
                            exist = false;
                            list.addReligion(userID, text, 1);
                        }
                        break;
                    case "Secret Question":
                        List_SecretQuestion secret = _context.ListSecretQuestion.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 0 && x.isDeleted != 1));
                        if (secret == null)
                        {
                            exist = false;
                            list.addSecretQuestion(userID, text, 1);
                        }
                        break;
                }
                if (exist)
                    TempData["Message"] = "Cannot add value of " + model.text + ". It already exist in " + model.name + " list";
                else
                    TempData["Message"] = "Added " + model.text + " in " + model.name + " list";

                TempData["Modal"] = "true";

                return RedirectToAction("ViewSelectedList", "Account", new { name = model.name, view = "new" });
            }
            return RedirectToAction("ViewSelectedList", "Account", new { name = model.name, view = "new" });
        }

        /*
        // POST: /Account/DeleteSelectedList
        [HttpPost]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult DeleteSelectedList(SelectedListViewModel model)
        {
            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int checkedID = Convert.ToInt32(Request.Form["CheckedList"]);
            string name = model.name;
            string text = null;

            string oldLogData = null;
            string logData = null;
            string tableAffected = null;
            List<SocialHistory> history = new List<SocialHistory>();
            bool canDelete = false;

            JObject oldValue = new JObject();
            oldValue["isChecked"] = 0;
            oldValue["isDeleted"] = 0;
            string logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

            switch (name)
            {
                case "Album":
                    AlbumCategory album = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatID == checkedID && x.isApproved == 1 && x.isDeleted != 1));
                    text = album.albumCatName;
                    List<AlbumPatient> albumPatient = _context.AlbumPatient.Where(x => (x.albumCatID == checkedID && x.isDeleted != 1)).ToList();
                    if (album != null && albumPatient.Count == 0)
                    {
                        tableAffected = "albumCategory";
                        oldLogData = new JavaScriptSerializer().Serialize(album);

                        album.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(album);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, album.albumCatID);

                        canDelete = true;
                    }
                    break;
                case "Allergy":
                    List_Allergy allergy = _context.ListAllergy.SingleOrDefault(x => (x.list_allergyID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = allergy.value;
                    List<Allergy> allergies = _context.Allergies.Where(x => (x.allergyListID == checkedID && x.isDeleted != 1)).ToList();
                    if (allergy != null && allergies.Count == 0)
                    {
                        tableAffected = "list_allergy";
                        oldLogData = new JavaScriptSerializer().Serialize(allergy);

                        allergy.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(allergy);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, allergy.list_allergyID);

                        canDelete = true;
                    }
                    break;
                case "Country":
                    List_Country country = _context.ListCountries.SingleOrDefault(x => (x.list_countryID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = country.value;
                    List<HolidayExperience> holiday = _context.HolidayExperiences.Where(x => (x.countryID == checkedID && x.isDeleted != 1)).ToList();
                    if (country != null && holiday.Count == 0)
                    {
                        tableAffected = "list_country";
                        oldLogData = new JavaScriptSerializer().Serialize(country);

                        country.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(country);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, country.list_countryID);

                        canDelete = true;
                    }
                    break;
                case "Diet":
                    List_Diet diet = _context.ListDiets.SingleOrDefault(x => (x.list_dietID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = diet.value;
                    history = _context.SocialHistories.Where(x => (x.dietID == checkedID && x.isDeleted != 1)).ToList();
                    if (diet != null && history.Count == 0)
                    {
                        tableAffected = "list_diet";
                        oldLogData = new JavaScriptSerializer().Serialize(diet);

                        diet.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(diet);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, diet.list_dietID);

                        canDelete = true;
                    }
                    break;
                case "Dislike":
                    List_Dislike dislike = _context.ListDislikes.SingleOrDefault(x => (x.list_dislikeID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = dislike.value;
                    List<Dislike> dislikes = _context.Dislikes.Where(x => (x.dislikeItemID == checkedID && x.isDeleted != 1)).ToList();
                    if (dislike != null && dislikes.Count == 0)
                    {
                        tableAffected = "list_dislike";
                        oldLogData = new JavaScriptSerializer().Serialize(dislike);

                        dislike.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(dislike);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, dislike.list_dislikeID);

                        canDelete = true;
                    }
                    break;
                case "Education":
                    List_Education education = _context.ListEducations.SingleOrDefault(x => (x.list_educationID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = education.value;
                    history = _context.SocialHistories.Where(x => (x.educationID == checkedID && x.isDeleted != 1)).ToList();
                    if (education != null && history.Count == 0)
                    {
                        tableAffected = "list_education";
                        oldLogData = new JavaScriptSerializer().Serialize(education);

                        education.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(education);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, education.list_educationID);

                        canDelete = true;
                    }
                    break;
                case "Habit":
                    List_Habit habit = _context.ListHabits.SingleOrDefault(x => (x.list_habitID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = habit.value;
                    List<Habit> habits = _context.Habits.Where(x => (x.habitListID == checkedID && x.isDeleted != 1)).ToList();
                    if (habit != null && habits.Count == 0)
                    {
                        tableAffected = "list_habit";
                        oldLogData = new JavaScriptSerializer().Serialize(habit);

                        habit.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(habit);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, habit.list_habitID);

                        canDelete = true;
                    }
                    break;
                case "Hobby":
                    List_Hobby hobby = _context.ListHobbies.SingleOrDefault(x => (x.list_hobbyID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = hobby.value;
                    List<Hobbies> hobbies = _context.Hobbieses.Where(x => (x.hobbyListID == checkedID && x.isDeleted != 1)).ToList();
                    if (hobby != null && hobbies.Count == 0)
                    {
                        tableAffected = "list_hobby";
                        oldLogData = new JavaScriptSerializer().Serialize(hobby);

                        hobby.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(hobby);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, hobby.list_hobbyID);

                        canDelete = true;
                    }
                    break;
                case "Language":
                    List_Language language = _context.ListLanguages.SingleOrDefault(x => (x.list_languageID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = language.value;
                    List<Language> languages = _context.Languages.Where(x => (x.languageListID == checkedID && x.isDeleted != 1)).ToList();
                    if (language != null && languages.Count == 0)
                    {
                        tableAffected = "list_language";
                        oldLogData = new JavaScriptSerializer().Serialize(language);

                        language.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(language);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, language.list_languageID);

                        canDelete = true;
                    }
                    break;
                case "Like":
                    List_Like like = _context.ListLikes.SingleOrDefault(x => (x.list_likeID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = like.value;
                    List<Like> likes = _context.Likes.Where(x => (x.likeItemID == checkedID && x.isDeleted != 1)).ToList();
                    if (like != null && likes.Count == 0)
                    {
                        tableAffected = "list_like";
                        oldLogData = new JavaScriptSerializer().Serialize(like);

                        like.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(like);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, like.list_likeID);

                        canDelete = true;
                    }
                    break;
                case "Live With":
                    List_LiveWith live = _context.ListLiveWiths.SingleOrDefault(x => (x.list_liveWithID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = live.value;
                    history = _context.SocialHistories.Where(x => (x.liveWithID == checkedID && x.isDeleted != 1)).ToList();
                    if (live != null && history.Count == 0)
                    {
                        tableAffected = "list_liveWith";
                        oldLogData = new JavaScriptSerializer().Serialize(live);

                        live.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(live);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, live.list_liveWithID);

                        canDelete = true;
                    }
                    break;
                case "Mobility":
                    List_Mobility mobility = _context.ListMobility.SingleOrDefault(x => (x.list_mobilityID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = mobility.value;
                    List<Mobility> mobilities = _context.Mobility.Where(x => (x.mobilityListID == checkedID && x.isDeleted != 1)).ToList();
                    if (mobility != null && mobilities.Count == 0)
                    {
                        tableAffected = "list_mobility";
                        oldLogData = new JavaScriptSerializer().Serialize(mobility);

                        mobility.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(mobility);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, mobility.list_mobilityID);

                        canDelete = true;
                    }
                    break;
                case "Occupation":
                    List_Occupation occupation = _context.ListOccupations.SingleOrDefault(x => (x.list_occupationID== checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = occupation.value;
                    history = _context.SocialHistories.Where(x => (x.occupationID == checkedID && x.isDeleted != 1)).ToList();
                    if (occupation != null && history.Count == 0)
                    {
                        tableAffected = "list_occupation";
                        oldLogData = new JavaScriptSerializer().Serialize(occupation);

                        occupation.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(occupation);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, occupation.list_occupationID);

                        canDelete = true;
                    }
                    break;
                case "Pet":
                    List_Pet pet = _context.ListPets.SingleOrDefault(x => (x.list_petID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = pet.value;
                    history = _context.SocialHistories.Where(x => (x.petID == checkedID && x.isDeleted != 1)).ToList();
                    if (pet != null && history.Count == 0)
                    {
                        tableAffected = "list_pet";
                        oldLogData = new JavaScriptSerializer().Serialize(pet);

                        pet.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(pet);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, pet.list_petID);

                        canDelete = true;
                    }
                    break;
                case "Prescription":
                    List_Prescription prescription = _context.ListPrescriptions.SingleOrDefault(x => (x.list_prescriptionID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = prescription.value;
                    List<Prescription> prescriptions = _context.Prescriptions.Where(x => (x.drugNameID == checkedID && x.isDeleted != 1)).ToList();
                    if (prescription != null && prescriptions.Count == 0)
                    {
                        tableAffected = "list_prescription";
                        oldLogData = new JavaScriptSerializer().Serialize(prescription);

                        prescription.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(prescription);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, prescription.list_prescriptionID);

                        canDelete = true;
                    }
                    break;
                case "Problem Log":
                    List_ProblemLog problem = _context.ListProblemLogs.SingleOrDefault(x => (x.list_problemLogID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = problem.value;
                    List<ProblemLog> problems = _context.ProblemLogs.Where(x => (x.problemLogID == checkedID && x.isDeleted != 1)).ToList();
                    if (problem != null && problems.Count == 0)
                    {
                        tableAffected = "list_problemLog";
                        oldLogData = new JavaScriptSerializer().Serialize(problem);

                        problem.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(problem);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, problem.list_problemLogID);

                        canDelete = true;
                    }
                    break;
                case "Relationship":
                    List_Relationship relationship = _context.ListRelationships.SingleOrDefault(x => (x.list_relationshipID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                    text = relationship.value;
                    List<PatientGuardian> patientGuardian = _context.PatientGuardian.Where(x => (x.guardianRelationshipID == checkedID && x.isDeleted != 1)).ToList();
                    List<PatientGuardian> patientGuardian2 = _context.PatientGuardian.Where(x => (x.guardian2RelationshipID == checkedID && x.isDeleted != 1)).ToList();

                    if (relationship != null && patientGuardian.Count == 0 && patientGuardian2.Count == 0)
                    {
                        tableAffected = "list_relationship";
                        oldLogData = new JavaScriptSerializer().Serialize(relationship);

                        relationship.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(relationship);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, relationship.list_relationshipID);

                        canDelete = true;
                    }
                    break;
                case "Religion":
                    List_Religion religion = _context.ListReligions.SingleOrDefault(x => (x.list_religionID == checkedID && x.isChecked == 0 && x.isDeleted != 1));
                    text = religion.value;
                    history = _context.SocialHistories.Where(x => (x.religionID == checkedID)).ToList();
                    if (religion != null && history.Count == 0)
                    {
                        tableAffected = "list_religion";
                        oldLogData = new JavaScriptSerializer().Serialize(religion);

                        religion.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(religion);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, religion.list_religionID);

                        canDelete = true;
                    }
                    break;
                case "Secret Question":
                    List_SecretQuestion secret = _context.ListSecretQuestion.SingleOrDefault(x => (x.list_secretQuestionID == checkedID && x.isChecked == 0 && x.isDeleted != 1));
                    text = secret.value;
                    if (secret != null)
                    {
                        tableAffected = "list_secretQuestion";
                        oldLogData = new JavaScriptSerializer().Serialize(secret);

                        secret.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(secret);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, secret.list_secretQuestionID);

                        canDelete = true;
                    }
                    break;
            }
            if (!canDelete)
                TempData["Message"] = "Cannot delete value of " + text + " from " + model.name + " list. It is being used.";
            else
            {
                _context.SaveChanges();
                TempData["Message"] = "Successfully deleted " + model.text + " from " + model.name + " list";
            }

            TempData["Modal"] = "true";

            return RedirectToAction("ViewSelectedList", "Account", new { name = model.name, view = "delete" });
        }*/

        // POST: /Account/UpdateSelectedList
        [HttpPost]
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult UpdateSelectedList(SelectedListViewModel model)
        {
            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            string uncheckedValue = Request.Form["UncheckedList"];
            int checkedID = Convert.ToInt32(Request.Form["CheckedList"]);
            string action = model.action;
            string text = model.text;
            string name = model.name;
            int uncheckedID = 0;
            bool exist = true;

            string oldLogData = null;
            string logData = null;
            string tableAffected = null;

            switch (name)
            {
                case "Album":
                    AlbumCategory album = _context.AlbumCategories.Where(x => (x.albumCatName == uncheckedValue && x.isApproved == 0 && x.isDeleted != 1)).ToList()[0];

                    JObject oldValue = new JObject();
                    oldValue["value"] = album.albumCatName;
                    oldValue["isApproved"] = album.isApproved;
                    string logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "albumCategory";
                    oldLogData = new JavaScriptSerializer().Serialize(album);
                    uncheckedID = album.albumCatID;
                    if (action == "Edit")
                    {
                        AlbumCategory sameValue = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatName.ToLower() == text.ToLower() && x.isApproved == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            album.albumCatName = text;
                            album.isApproved = 1;

                            logData = new JavaScriptSerializer().Serialize(album);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, album.albumCatID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        album.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(album);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, album.albumCatID);

                        AlbumCategory album2 = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatID == checkedID && x.isApproved == 1 && x.isDeleted != 1));
                        album2.createDateTime = DateTime.Now;
                        List<AlbumPatient> albumPatient = _context.AlbumPatient.Where(x => (x.albumCatID == uncheckedID)).ToList();
                        for (int i = 0; i < albumPatient.Count; i++)
                            albumPatient[i].albumCatID = checkedID;

                        list.updateLog("albumPatient", "albumCatID", checkedID, uncheckedID);
                    }
                    break;
                case "Allergy":
                    List_Allergy allergy = _context.ListAllergy.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = allergy.value;
                    oldValue["isChecked"] = allergy.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_allergy";
                    oldLogData = new JavaScriptSerializer().Serialize(allergy);
                    uncheckedID = allergy.list_allergyID;
                    if (action == "Edit")
                    {
                        List_Allergy sameValue = _context.ListAllergy.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            allergy.value = text;
                            allergy.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(allergy);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, allergy.list_allergyID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        allergy.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(allergy);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, allergy.list_allergyID);

                        List_Allergy allergy2 = _context.ListAllergy.SingleOrDefault(x => (x.list_allergyID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        allergy2.createDateTime = DateTime.Now;
                        List<Allergy> allergies = _context.Allergies.Where(x => (x.allergyListID == uncheckedID)).ToList();
                        for (int i = 0; i < allergies.Count; i++)
                            allergies[i].allergyListID = checkedID;

                        list.updateLog("allergy", "allergyListID", checkedID, uncheckedID);
                    }
                    break;
                case "Country":
                    List_Country country = _context.ListCountries.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = country.value;
                    oldValue["isChecked"] = country.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_country";
                    oldLogData = new JavaScriptSerializer().Serialize(country);
                    uncheckedID = country.list_countryID;
                    if (action == "Edit")
                    {
                        List_Country sameValue = _context.ListCountries.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            country.value = text;
                            country.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(country);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, country.list_countryID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        country.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(country);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, country.list_countryID);

                        List_Country country2 = _context.ListCountries.SingleOrDefault(x => (x.list_countryID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        country2.createDateTime = DateTime.Now;
                        List<HolidayExperience> holiday = _context.HolidayExperiences.Where(x => (x.countryID == uncheckedID)).ToList();
                        for (int i = 0; i < holiday.Count; i++)
                            holiday[i].countryID = checkedID;

                        list.updateLog("holidayExperience", "countryID", checkedID, uncheckedID);
                    }
                    break;
                case "Diet":
                    List_Diet diet = _context.ListDiets.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = diet.value;
                    oldValue["isChecked"] = diet.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_diet";
                    oldLogData = new JavaScriptSerializer().Serialize(diet);
                    uncheckedID = diet.list_dietID;
                    if (action == "Edit")
                    {
                        List_Diet sameValue = _context.ListDiets.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            diet.value = text;
                            diet.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(diet);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, diet.list_dietID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        diet.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(diet);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, diet.list_dietID);

                        List_Diet diet2 = _context.ListDiets.SingleOrDefault(x => (x.list_dietID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        diet2.createDateTime = DateTime.Now;
                        List<SocialHistory> history = _context.SocialHistories.Where(x => (x.dietID == uncheckedID)).ToList();
                        for (int i = 0; i < history.Count; i++)
                            history[i].dietID = checkedID;

                        list.updateLog("socialHistory", "dietID", checkedID, uncheckedID);
                    }
                    break;
                case "Dislike":
                    List_Dislike dislike = _context.ListDislikes.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = dislike.value;
                    oldValue["isChecked"] = dislike.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_dislike";
                    oldLogData = new JavaScriptSerializer().Serialize(dislike);
                    uncheckedID = dislike.list_dislikeID;
                    if (action == "Edit")
                    {
                        List_Dislike sameValue = _context.ListDislikes.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            dislike.value = text;
                            dislike.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(dislike);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, dislike.list_dislikeID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        dislike.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(dislike);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, dislike.list_dislikeID);

                        List_Dislike dislike2 = _context.ListDislikes.SingleOrDefault(x => (x.list_dislikeID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        dislike2.createDateTime = DateTime.Now;
                        List<Dislike> dislikes = _context.Dislikes.Where(x => (x.dislikeItemID == uncheckedID)).ToList();
                        for (int i = 0; i < dislikes.Count; i++)
                            dislikes[i].dislikeItemID = checkedID;

                        list.updateLog("dislikes", "dislikeItemID", checkedID, uncheckedID);
                    }
                    break;
                case "Education":
                    List_Education education = _context.ListEducations.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = education.value;
                    oldValue["isChecked"] = education.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_education";
                    oldLogData = new JavaScriptSerializer().Serialize(education);
                    uncheckedID = education.list_educationID;
                    if (action == "Edit")
                    {
                        List_Education sameValue = _context.ListEducations.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            education.value = text;
                            education.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(education);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, education.list_educationID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        education.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(education);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, education.list_educationID);

                        List_Education education2 = _context.ListEducations.SingleOrDefault(x => (x.list_educationID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        education2.createDateTime = DateTime.Now;
                        List<SocialHistory> history = _context.SocialHistories.Where(x => (x.educationID == uncheckedID)).ToList();
                        for (int i = 0; i < history.Count; i++)
                            history[i].educationID = checkedID;

                        list.updateLog("socialHistory", "educationID", checkedID, uncheckedID);
                    }
                    break;
                case "Habit":
                    List_Habit habit = _context.ListHabits.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = habit.value;
                    oldValue["isChecked"] = habit.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_habit";
                    oldLogData = new JavaScriptSerializer().Serialize(habit);
                    uncheckedID = habit.list_habitID;
                    if (action == "Edit")
                    {
                        List_Habit sameValue = _context.ListHabits.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            habit.value = text;
                            habit.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(habit);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, habit.list_habitID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        habit.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(habit);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, habit.list_habitID);

                        List_Habit habit2 = _context.ListHabits.SingleOrDefault(x => (x.list_habitID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        habit2.createDateTime = DateTime.Now;
                        List<Habit> habits = _context.Habits.Where(x => (x.habitListID == uncheckedID)).ToList();
                        for (int i = 0; i < habits.Count; i++)
                            habits[i].habitListID = checkedID;

                        list.updateLog("habits", "habitListID", checkedID, uncheckedID);
                    }
                    break;
                case "Hobby":
                    List_Hobby hobby = _context.ListHobbies.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = hobby.value;
                    oldValue["isChecked"] = hobby.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_hobby";
                    oldLogData = new JavaScriptSerializer().Serialize(hobby);
                    uncheckedID = hobby.list_hobbyID;
                    if (action == "Edit")
                    {
                        List_Hobby sameValue = _context.ListHobbies.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            hobby.value = text;
                            hobby.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(hobby);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, hobby.list_hobbyID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        hobby.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(hobby);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, hobby.list_hobbyID);

                        List_Hobby hobby2 = _context.ListHobbies.SingleOrDefault(x => (x.list_hobbyID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        hobby2.createDateTime = DateTime.Now;
                        List<Hobbies> hobbies = _context.Hobbieses.Where(x => (x.hobbyListID == uncheckedID)).ToList();
                        for (int i = 0; i < hobbies.Count; i++)
                            hobbies[i].hobbyListID = checkedID;

                        list.updateLog("hobbies", "hobbyListID", checkedID, uncheckedID);
                    }
                    break;
                case "Language":
                    List_Language language = _context.ListLanguages.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = language.value;
                    oldValue["isChecked"] = language.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_language";
                    oldLogData = new JavaScriptSerializer().Serialize(language);
                    uncheckedID = language.list_languageID;
                    if (action == "Edit")
                    {
                        List_Language sameValue = _context.ListLanguages.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            language.value = text;
                            language.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(language);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, language.list_languageID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        language.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(language);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, language.list_languageID);

                        List_Language language2 = _context.ListLanguages.SingleOrDefault(x => (x.list_languageID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        language2.createDateTime = DateTime.Now;
                        List<Language> languages = _context.Languages.Where(x => (x.languageListID == uncheckedID)).ToList();
                        for (int i = 0; i < languages.Count; i++)
                            languages[i].languageListID = checkedID;

                        list.updateLog("language", "languageListID", checkedID, uncheckedID);
                    }
                    break;
                case "Like":
                    List_Like like = _context.ListLikes.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = like.value;
                    oldValue["isChecked"] = like.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_like";
                    oldLogData = new JavaScriptSerializer().Serialize(like);
                    uncheckedID = like.list_likeID;
                    if (action == "Edit")
                    {
                        List_Like sameValue = _context.ListLikes.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            like.value = text;
                            like.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(like);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, like.list_likeID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        like.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(like);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, like.list_likeID);

                        List_Like like2 = _context.ListLikes.SingleOrDefault(x => (x.list_likeID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        like2.createDateTime = DateTime.Now;
                        List<Like> likes = _context.Likes.Where(x => (x.likeItemID == uncheckedID)).ToList();
                        for (int i = 0; i < likes.Count; i++)
                            likes[i].likeItemID = checkedID;

                        list.updateLog("likes", "likeItemID", checkedID, uncheckedID);
                    }
                    break;
                case "Live With":
                    List_LiveWith live = _context.ListLiveWiths.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = live.value;
                    oldValue["isChecked"] = live.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_live";
                    oldLogData = new JavaScriptSerializer().Serialize(live);
                    uncheckedID = live.list_liveWithID;
                    if (action == "Edit")
                    {
                        List_LiveWith sameValue = _context.ListLiveWiths.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            live.value = text;
                            live.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(live);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, live.list_liveWithID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        live.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(live);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, live.list_liveWithID);

                        List_LiveWith live2 = _context.ListLiveWiths.SingleOrDefault(x => (x.list_liveWithID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        live2.createDateTime = DateTime.Now;
                        List<SocialHistory> history = _context.SocialHistories.Where(x => (x.liveWithID == uncheckedID)).ToList();
                        for (int i = 0; i < history.Count; i++)
                            history[i].liveWithID = checkedID;

                        list.updateLog("socialHistory", "liveWithID", checkedID, uncheckedID);
                    }
                    break;
                case "Mobility":
                    List_Mobility mobility = _context.ListMobility.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = mobility.value;
                    oldValue["isChecked"] = mobility.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_mobility";
                    oldLogData = new JavaScriptSerializer().Serialize(mobility);
                    uncheckedID = mobility.list_mobilityID;
                    if (action == "Edit")
                    {
                        List_Mobility sameValue = _context.ListMobility.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            mobility.value = text;
                            mobility.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(mobility);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, mobility.list_mobilityID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        mobility.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(mobility);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, mobility.list_mobilityID);

                        List_Mobility mobility2 = _context.ListMobility.SingleOrDefault(x => (x.list_mobilityID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        mobility2.createDateTime = DateTime.Now;
                        List<Mobility> mobilities = _context.Mobility.Where(x => (x.mobilityListID == uncheckedID)).ToList();
                        for (int i = 0; i < mobilities.Count; i++)
                            mobilities[i].mobilityListID = checkedID;

                        list.updateLog("mobility", "mobilityListID", checkedID, uncheckedID);
                    }
                    break;
                case "Occupation":
                    List_Occupation occupation = _context.ListOccupations.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = occupation.value;
                    oldValue["isChecked"] = occupation.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_occupation";
                    oldLogData = new JavaScriptSerializer().Serialize(occupation);
                    uncheckedID = occupation.list_occupationID;
                    if (action == "Edit")
                    {
                        List_Occupation sameValue = _context.ListOccupations.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            occupation.value = text;
                            occupation.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(occupation);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, occupation.list_occupationID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        occupation.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(occupation);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, occupation.list_occupationID);

                        List_Occupation occupation2 = _context.ListOccupations.SingleOrDefault(x => (x.list_occupationID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        occupation2.createDateTime = DateTime.Now;
                        List<SocialHistory> history = _context.SocialHistories.Where(x => (x.occupationID == uncheckedID)).ToList();
                        for (int i = 0; i < history.Count; i++)
                            history[i].occupationID = checkedID;

                        list.updateLog("socialHistory", "occupationID", checkedID, uncheckedID);
                    }
                    break;
                case "Pet":
                    List_Pet pet = _context.ListPets.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = pet.value;
                    oldValue["isChecked"] = pet.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_pet";
                    oldLogData = new JavaScriptSerializer().Serialize(pet);
                    uncheckedID = pet.list_petID;
                    if (action == "Edit")
                    {
                        List_Pet sameValue = _context.ListPets.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            pet.value = text;
                            pet.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(pet);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, pet.list_petID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        pet.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(pet);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, pet.list_petID);

                        List_Pet pet2 = _context.ListPets.SingleOrDefault(x => (x.list_petID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        pet2.createDateTime = DateTime.Now;
                        List<SocialHistory> history = _context.SocialHistories.Where(x => (x.petID == uncheckedID)).ToList();
                        for (int i = 0; i < history.Count; i++)
                            history[i].petID = checkedID;

                        list.updateLog("socialHistory", "petID", checkedID, uncheckedID);
                    }
                    break;
                case "Prescription":
                    List_Prescription prescription = _context.ListPrescriptions.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = prescription.value;
                    oldValue["isChecked"] = prescription.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_prescription";
                    oldLogData = new JavaScriptSerializer().Serialize(prescription);
                    uncheckedID = prescription.list_prescriptionID;
                    if (action == "Edit")
                    {
                        List_Prescription sameValue = _context.ListPrescriptions.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            prescription.value = text;
                            prescription.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(prescription);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, prescription.list_prescriptionID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        prescription.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(prescription);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, prescription.list_prescriptionID);

                        List_Prescription prescription2 = _context.ListPrescriptions.SingleOrDefault(x => (x.list_prescriptionID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        prescription2.createDateTime = DateTime.Now;
                        List<Prescription> prescriptions = _context.Prescriptions.Where(x => (x.drugNameID == uncheckedID)).ToList();
                        for (int i = 0; i < prescriptions.Count; i++)
                            prescriptions[i].drugNameID = checkedID;

                        list.updateLog("prescription", "drugNameID", checkedID, uncheckedID);
                    }
                    break;
                case "Problem Log":
                    List_ProblemLog problem = _context.ListProblemLogs.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = problem.value;
                    oldValue["isChecked"] = problem.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_problem";
                    oldLogData = new JavaScriptSerializer().Serialize(problem);
                    uncheckedID = problem.list_problemLogID;
                    if (action == "Edit")
                    {
                        List_ProblemLog sameValue = _context.ListProblemLogs.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            problem.value = text;
                            problem.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(problem);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, problem.list_problemLogID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        problem.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(problem);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, problem.list_problemLogID);

                        List_ProblemLog problem2 = _context.ListProblemLogs.SingleOrDefault(x => (x.list_problemLogID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        problem2.createDateTime = DateTime.Now;
                        List<ProblemLog> problems = _context.ProblemLogs.Where(x => (x.problemLogID == uncheckedID)).ToList();
                        for (int i = 0; i < problems.Count; i++)
                            problems[i].problemLogID = checkedID;

                        list.updateLog("problemLog", "categoryID", checkedID, uncheckedID);
                    }
                    break;
                case "Relationship":
                    List_Relationship relationship = _context.ListRelationships.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = relationship.value;
                    oldValue["isChecked"] = relationship.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_relationship";
                    oldLogData = new JavaScriptSerializer().Serialize(relationship);
                    uncheckedID = relationship.list_relationshipID;
                    if (action == "Edit")
                    {
                        List_Relationship sameValue = _context.ListRelationships.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            relationship.value = text;
                            relationship.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(relationship);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, relationship.list_relationshipID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        relationship.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(relationship);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, relationship.list_relationshipID);

                        List_Relationship relationship2 = _context.ListRelationships.SingleOrDefault(x => (x.list_relationshipID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        relationship2.createDateTime = DateTime.Now;
                        List<PatientGuardian> patientGuardian = _context.PatientGuardian.Where(x => (x.guardianRelationshipID == uncheckedID)).ToList();
                        for (int i = 0; i < patientGuardian.Count; i++)
                            patientGuardian[i].guardianRelationshipID = checkedID;
                        List<PatientGuardian> patientGuardian2 = _context.PatientGuardian.Where(x => (x.guardian2RelationshipID == uncheckedID)).ToList();
                        for (int i = 0; i < patientGuardian2.Count; i++)
                            patientGuardian2[i].guardian2RelationshipID = checkedID;

                        list.updateLog("patientGuardian", "guardianRelationshipID", checkedID, uncheckedID);
                        list.updateLog("patientGuardian", "guardian2RelationshipID", checkedID, uncheckedID);
                    }
                    break;
                case "Religion":
                    List_Religion religion = _context.ListReligions.Where(x => (x.value == uncheckedValue && x.isChecked == 0 && x.isDeleted != 1)).ToList()[0];

                    oldValue = new JObject();
                    oldValue["value"] = religion.value;
                    oldValue["isChecked"] = religion.isChecked;
                    logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);

                    tableAffected = "list_religion";
                    oldLogData = new JavaScriptSerializer().Serialize(religion);
                    uncheckedID = religion.list_religionID;
                    if (action == "Edit")
                    {
                        List_Religion sameValue = _context.ListReligions.SingleOrDefault(x => (x.value.ToLower() == text.ToLower() && x.isChecked == 1 && x.isDeleted != 1));
                        if (sameValue == null)
                        {
                            exist = false;
                            religion.value = text;
                            religion.isChecked = 1;

                            logData = new JavaScriptSerializer().Serialize(religion);
                            list.updateListLog(userID, oldLogData, logData, logOldValue, tableAffected, religion.list_religionID, text);
                        }
                    }
                    else if (action == "Replace")
                    {
                        religion.isDeleted = 1;
                        logData = new JavaScriptSerializer().Serialize(religion);
                        list.deleteListLog(userID, oldLogData, logData, logOldValue, tableAffected, religion.list_religionID);

                        List_Religion religion2 = _context.ListReligions.SingleOrDefault(x => (x.list_religionID == checkedID && x.isChecked == 1 && x.isDeleted != 1));
                        religion2.createDateTime = DateTime.Now;
                        List<SocialHistory> history = _context.SocialHistories.Where(x => (x.religionID == uncheckedID)).ToList();
                        for (int i = 0; i < history.Count; i++)
                            history[i].religionID = checkedID;

                        list.updateLog("socialHistory", "religionID", checkedID, uncheckedID);
                    }
                    break;
                case "Secret Question":
                    // not possible
                    break;
            }
            if (action == "Edit" && exist)
                TempData["Message"] = "Cannot add value of " + model.text + ". It already exist in " + model.name + " list";
            else
            {
                _context.SaveChanges();
                TempData["Message"] = "Updated " + model.text + " in " + model.name + " list";
            }
            TempData["Modal"] = "true";
            return RedirectToAction("ViewSelectedList", "Account", new { name = model.name, view = "update" });
        }

        // GET: /Account/ManageCentre
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult ManageCentre()
        {
            ViewBag.Modal = TempData["Modal"];

            var centreList = (from cca in _context.CareCentreAttributes
                         join lc in _context.ListCountries on cca.centreCountryID equals lc.list_countryID
                         where cca.isDeleted != 1
                         select new centreListViewModel
                         {
                             centreID = cca.centreID,
                             centreName = cca.centreName,
                             centreCountry = lc.value,
                             centreAddress = cca.centreAddress,
                             centrePostalCode = cca.centrePostalCode,
                             centreEmail = cca.centreEmail,
                             centreContactNo = cca.centreContactNo,
                         }).ToList();

            centreList = centre.getAllHours(centreList);
            
            var model = new CentreViewModel
            {
                careCentre = centreList,
                canAddCentre = centreList.Count > 0 ? 0 : 1
            };

            return View(model);
        }

        // GET: /Account/AddCentre
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult AddCentre()
        {
            List<SelectListItem> countries = list.getCountryList(1);
            ViewBag.Country = new SelectList(countries, "Value", "Text");

            ViewBag.Modal = TempData["Modal"];

            return View();
        }

        // POST: /Account/AddCentre
        [HttpPost]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult AddCentre(AddCentreViewModel model)
        {
            int countryID = Convert.ToInt32(Request.Form["Country"]);

            List<SelectListItem> countries = list.getCountryList(1);
            ViewBag.Country = new SelectList(countries, "Value", "Text");

            if (ModelState.IsValid)
            {
                DateTime date = DateTime.Now;

                if (_context.CareCentreAttributes.FirstOrDefault(x => (x.centreName == model.centreName && x.isDeleted != 1)) != null)
                {
                    ViewBag.Error = "Centre of the same name already exists!";
                    return View(model);
                }
                else if (countryID == 0)
                {
                    ViewBag.Error = "Select a Country";
                    return View(model);
                }
                else
                {
                    int userID = Convert.ToInt32(User.Identity.GetUserID2());

                    CareCentreAttributes centre = new CareCentreAttributes();
                    centre.centreName = model.centreName;
                    centre.centreCountryID = countryID;
                    centre.centreAddress = model.centreAddress;
                    centre.centrePostalCode = model.centrePostalCode;
                    centre.centreContactNo = model.centreContactNo;
                    centre.centreEmail = model.centreEmail;
                    centre.isDeleted = 0;
                    centre.createDateTime = date;
                    _context.CareCentreAttributes.Add(centre);
                    _context.SaveChanges();

                    string logData = new JavaScriptSerializer().Serialize(centre);
                    string logDesc = "New care centre";
                    int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
                    string remarks = "centre created name: " + model.centreName;
                    int rowAffected = _context.CareCentreAttributes.Max(x => x.centreID);

                    // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
                    shortcutMethod.addLogAccount(userID, null, null, logData, logDesc, logCategoryID, remarks, "CareCentreAttributes", "ALL", rowAffected, null, null, null);

                    TempData["Message"] = "Centre has been created for " + model.centreName + ". Please update the centre opening hours.";
                    TempData["Modal"] = "true";
                    return RedirectToAction("UpdateCentre", "Account", new { id = centre.centreID });
                }
            }
            // If we got this far, something failed, redisplay form
            return View();
        }

        // GET: /Account/UpdateCentre
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult UpdateCentre(int id)
        {
            ViewBag.Modal = TempData["Modal"];

            CareCentreAttributes careCentre = _context.CareCentreAttributes.SingleOrDefault(x => (x.centreID == id && x.isDeleted != 1));
            List<CareCentreHours> centreHoursList = _context.CareCentreHours.Where(x => (x.centreID == id && x.isDeleted != 1)).ToList();

            string country = _context.ListCountries.SingleOrDefault(x => (x.list_countryID == careCentre.centreCountryID)).value;

            var model = new ViewCentreViewModel
            {
                centreID = careCentre.centreID,
                centreName = careCentre.centreName,
                centreCountry = country,
                centreAddress = careCentre.centreAddress,
                centrePostalCode = careCentre.centrePostalCode,
                centreEmail = careCentre.centreEmail,
                centreContactNo = careCentre.centreContactNo,
            };

            model = centre.getViewCentreViewModel(centreHoursList, model);

            return View(model);
        }

        // Post: /Account/UpdateCentre
        [HttpPost]
        //[NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult UpdateCentre(ViewCentreViewModel model)
        {
            if (ModelState.IsValid)
            {
                int countryID = _context.ListCountries.SingleOrDefault(x => (x.value == model.centreCountry)).list_countryID;
                CareCentreAttributes careCentre = _context.CareCentreAttributes.SingleOrDefault(x => (x.centreID == model.centreID && x.centreName == model.centreName && x.centreCountryID == countryID && x.isDeleted != 1));
                if (careCentre == null)
                {
                    TempData["Message"] = "Centre name " + model.centreName + " does not exist";
                    ViewBag.Modal = "true";
                    return View();
                }

                int userID = Convert.ToInt32(User.Identity.GetUserID2());
                bool resultHours = centre.checkCentreHours(userID, careCentre.centreID, model.mon, model.monStartTime, model.monEndTime, model.tue, model.tueStartTime, model.tueEndTime, model.wed, model.wedStartTime, model.wedEndTime, model.thurs, model.thursStartTime, model.thursEndTime, model.fri, model.friStartTime, model.friEndTime, model.sat, model.satStartTime, model.satEndTime, model.sun, model.sunStartTime, model.sunEndTime);

                // centre.updateCentreInformation(int centreID, string centreName, string centreCountry, string centreAddress, string centrePostalCode, string centreContactNo, string centreEmail)
                bool resultCentre = centre.updateCentreInformation(userID, careCentre, model.centreAddress, model.centrePostalCode, model.centreContactNo, model.centreEmail);
                
                if (!resultHours && !resultCentre)
                {
                    TempData["Message"] = "No changes are made for centre: " + careCentre.centreName;
                    TempData["Modal"] = "true";
                    return RedirectToAction("ManageCentre", "Account");
                }

                TempData["Message"] = "Updated information for " + model.centreName;
                TempData["Modal"] = "true";
                return RedirectToAction("ManageCentre", "Account");
            }
            return View(model);
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
 