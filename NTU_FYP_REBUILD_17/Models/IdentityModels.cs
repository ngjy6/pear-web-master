using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NTU_FYP_REBUILD_17.Models
{
   
    public class ApplicationUser : IdentityUser
    {
        public int userID { get; set; }
        public virtual UserType UserType { get; set; }
        [ForeignKey("UserType")]
        public int userTypeID { get; set; }
        public virtual UserType tempUserType { get; set; }
        [ForeignKey("tempUserType")]
        public int? tempUserTypeID { get; set; }
        [StringLength(256)]
        public string password { get; set; }
        public string secretQuestion { get; set; }
        public string secretAnswer { get; set; }
        [StringLength(256)]
        public string preferredName { get; set; }
        [StringLength(256)]
        public string firstName { get; set; }
        [StringLength(256)]
        public string lastName { get; set; }
        [StringLength(16)]
        public string nric { get; set; }
        [StringLength(16)]
        public string maskedNric { get; set; }
        public string address { get; set; }
        [StringLength(32)]
        public string officeNo { get; set; }
        public DateTime DOB { get; set; }
        [StringLength(1)]
        public string gender { get; set; }
        public int isLocked { get; set; }
        public string reason { get; set; }
        public int allowNotification { get; set; }
        public int isActive { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime CreateDateTime { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim("userID", this.userID.ToString()));
            userIdentity.AddClaim(new Claim("userTypeID", this.userTypeID.ToString()));
            userIdentity.AddClaim(new Claim("tempUserTypeID", this.userTypeID.ToString()));
            userIdentity.AddClaim(new Claim("preferredName", this.firstName.ToString()));
            userIdentity.AddClaim(new Claim("firstName", this.firstName.ToString()));
            userIdentity.AddClaim(new Claim("lastName", this.lastName.ToString()));
            userIdentity.AddClaim(new Claim("nric", this.nric.ToString()));
            userIdentity.AddClaim(new Claim("allowNotification", this.allowNotification.ToString()));
            return userIdentity;
        }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }

    public static class IdentityExtensions
    {
        public static string GetUserTypeID(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            var ci = identity as ClaimsIdentity;
            if (ci != null)
            {
                return ci.FindFirstValue("userTypeID");
            }
            return null;
        }

        public static string GetUserFirstName(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            var ci = identity as ClaimsIdentity;
            if (ci != null)
            {
                return ci.FindFirstValue("firstName");
            }
            return null;
        }

        public static string GetUserID2(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            var ci = identity as ClaimsIdentity;
            if (ci != null)
            {
                return ci.FindFirstValue("userID");
            }
            return null;
        }

        public static string GetAllowNotification(this IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }
            var ci = identity as ClaimsIdentity;
            if (ci != null)
            {
                string test = ci.FindFirstValue("allowNotification");
                return ci.FindFirstValue("allowNotification");
            }
            return null;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ActivityAvailability> ActivityAvailabilities { get; set; }
        public DbSet<ActivityExclusion> ActivityExclusions { get; set; }
        public DbSet<ActivityPreference> ActivityPreferences { get; set; }
        public DbSet<AdHoc> AdHocs { get; set; }
        public DbSet<AlbumCategory> AlbumCategories { get; set; }
        public DbSet<AlbumPatient> AlbumPatient { get; set; }
        public DbSet<AlbumUser> AlbumUser { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<AssignedGame> AssignedGames { get; set; }
        public DbSet<AttendanceLog> AttendanceLog { get; set; }
        public DbSet<CareCentreAttributes> CareCentreAttributes { get; set; }
        public DbSet<CareCentreHours> CareCentreHours { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CentreActivity> CentreActivities { get; set; }
        public DbSet<DailyUpdates> DailyUpdates { get; set; }
        public DbSet<DementiaType> DementiaTypes { get; set; }
        public DbSet<Dislike> Dislikes { get; set; }
        public DbSet<DoctorNote> DoctorNotes { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameCategoryAssignedDementia> GameCategoryAssignedDementia { get; set; }
        public DbSet<GameAssignedDementia> GameAssignedDementias { get; set; }
        public DbSet<GameCategory> GameCategories { get; set; }
        public DbSet<GameRecord> GameRecords { get; set; }
        public DbSet<GamesTypeRecommendation> GamesTypeRecommendations { get; set; }
        public DbSet<Habit> Habits { get; set; }
        public DbSet<Highlight> Highlight { get; set; }
        public DbSet<HighlightType> HighlightType { get; set; }
        public DbSet<HighlightThreshold> HighlightThreshold { get; set; }
        public DbSet<Hobbies> Hobbieses { get; set; }
        public DbSet<HolidayExperience> HolidayExperiences { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<List_Allergy> ListAllergy { get; set; }
        public DbSet<List_Country> ListCountries { get; set; }
        public DbSet<List_Diet> ListDiets { get; set; }
        public DbSet<List_Dislike> ListDislikes { get; set; }
        public DbSet<List_Education> ListEducations { get; set; }
        public DbSet<List_Habit> ListHabits { get; set; }
        public DbSet<List_Hobby> ListHobbies { get; set; }
        public DbSet<List_Language> ListLanguages { get; set; }
        public DbSet<List_Like> ListLikes { get; set; }
        public DbSet<List_LiveWith> ListLiveWiths { get; set; }
        public DbSet<List_Mobility> ListMobility { get; set; }
        public DbSet<List_Occupation> ListOccupations { get; set; }
        public DbSet<List_Pet> ListPets { get; set; }
        public DbSet<List_Prescription> ListPrescriptions { get; set; }
        public DbSet<List_ProblemLog> ListProblemLogs { get; set; }
        public DbSet<List_Relationship> ListRelationships { get; set; }
        public DbSet<List_Religion> ListReligions { get; set; }
        public DbSet<List_SecretQuestion> ListSecretQuestion { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<LogAccount> LogAccount { get; set; }
        public DbSet<LogApproveReject> LogApproveReject { get; set; }
        public DbSet<LogCategory> LogCategories { get; set; }
        public DbSet<LogNotification> LogNotification { get; set; }
        public DbSet<MedicalHistory> MedicalHistory { get; set; }
        public DbSet<MedicationLog> MedicationLog { get; set; }
        public DbSet<Mobility> Mobility { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientGuardian> PatientGuardian { get; set; }
        public DbSet<PatientAllocation> PatientAllocations { get; set; }
        public DbSet<PatientAssignedDementia> PatientAssignedDementias { get; set; }
        public DbSet<PerformanceMetricName> PerformanceMetricNames { get; set; }
        public DbSet<PerformanceMetricOrder> PerformanceMetricOrders { get; set; }
        public DbSet<PersonInCharge> PersonInCharge { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrivacyUserRole> PrivacyUserRole { get; set; }
        public DbSet<PrivacyLevel> PrivacyLevel { get; set; }
        public DbSet<PrivacySettings> PrivacySettings { get; set; }
        public DbSet<ProblemLog> ProblemLogs { get; set; }
        public DbSet<Routine> Routines { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<SocialHistory> SocialHistories { get; set; }
        public DbSet<UserDeviceToken> UserDeviceToken { get; set; }
        public DbSet<User> UserTables { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Vital> Vitals { get; set; }


        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ActivityAvailability>().ToTable("activityAvailability");
            modelBuilder.Entity<ActivityExclusion>().ToTable("activityExclusion");
            modelBuilder.Entity<ActivityPreference>().ToTable("activityPreferences");
            modelBuilder.Entity<AdHoc>().ToTable("adHoc");
            modelBuilder.Entity<AlbumCategory>().ToTable("albumCategory");
            modelBuilder.Entity<AlbumPatient>().ToTable("albumPatient");
            modelBuilder.Entity<AlbumUser>().ToTable("albumUser");
            modelBuilder.Entity<Allergy>().ToTable("allergy");
            modelBuilder.Entity<AssignedGame>().ToTable("assignedGame");
            modelBuilder.Entity<AttendanceLog>().ToTable("attendanceLog");
            modelBuilder.Entity<CareCentreAttributes>().ToTable("careCentreAttributes");
            modelBuilder.Entity<CareCentreHours>().ToTable("careCentreHours");
            modelBuilder.Entity<Category>().ToTable("category");
            modelBuilder.Entity<CentreActivity>().ToTable("centreActivity");
            modelBuilder.Entity<DailyUpdates>().ToTable("dailyUpdates");
            modelBuilder.Entity<DementiaType>().ToTable("dementiaType");
            modelBuilder.Entity<Dislike>().ToTable("dislikes");
            modelBuilder.Entity<DoctorNote>().ToTable("doctorNote");
            modelBuilder.Entity<Game>().ToTable("game");
            modelBuilder.Entity<GameCategoryAssignedDementia>().ToTable("gameCategoryAssignedDementia");
            modelBuilder.Entity<GameAssignedDementia>().ToTable("gameAssignedDementia");
            modelBuilder.Entity<GameCategory>().ToTable("gameCategory");
            modelBuilder.Entity<GameRecord>().ToTable("gameRecord");
            modelBuilder.Entity<GamesTypeRecommendation>().ToTable("gamesTypeRecommendation");
            modelBuilder.Entity<Habit>().ToTable("habits");
            modelBuilder.Entity<Highlight>().ToTable("highlight");
            modelBuilder.Entity<HighlightType>().ToTable("highlightType");
            modelBuilder.Entity<HighlightThreshold>().ToTable("highlightThreshold");
            modelBuilder.Entity<Hobbies>().ToTable("hobbies");
            modelBuilder.Entity<HolidayExperience>().ToTable("holidayExperience");
            modelBuilder.Entity<Language>().ToTable("language");
            modelBuilder.Entity<Like>().ToTable("likes");
            modelBuilder.Entity<List_Allergy>().ToTable("list_allergy");
            modelBuilder.Entity<List_Country>().ToTable("list_country");
            modelBuilder.Entity<List_Diet>().ToTable("list_diet");
            modelBuilder.Entity<List_Dislike>().ToTable("list_dislike");
            modelBuilder.Entity<List_Education>().ToTable("list_education");
            modelBuilder.Entity<List_Habit>().ToTable("list_habit");
            modelBuilder.Entity<List_Hobby>().ToTable("list_hobby");
            modelBuilder.Entity<List_Language>().ToTable("list_language");
            modelBuilder.Entity<List_Like>().ToTable("list_like");
            modelBuilder.Entity<List_LiveWith>().ToTable("list_liveWith");
            modelBuilder.Entity<List_Mobility>().ToTable("list_mobility");
            modelBuilder.Entity<List_Occupation>().ToTable("list_occupation");
            modelBuilder.Entity<List_Pet>().ToTable("list_pet");
            modelBuilder.Entity<List_Prescription>().ToTable("list_prescription");
            modelBuilder.Entity<List_ProblemLog>().ToTable("list_problemLog");
            modelBuilder.Entity<List_Relationship>().ToTable("list_relationship");
            modelBuilder.Entity<List_Religion>().ToTable("list_religion");
            modelBuilder.Entity<List_SecretQuestion>().ToTable("list_secretQuestion");
            modelBuilder.Entity<Log>().ToTable("log");
            modelBuilder.Entity<LogAccount>().ToTable("logAccount");
            modelBuilder.Entity<LogApproveReject>().ToTable("logApproveReject");
            modelBuilder.Entity<LogCategory>().ToTable("logCategory");
            modelBuilder.Entity<LogNotification>().ToTable("logNotification");
            modelBuilder.Entity<MedicalHistory>().ToTable("medicalHistory");
            modelBuilder.Entity<MedicationLog>().ToTable("medicationLog");
            modelBuilder.Entity<Mobility>().ToTable("mobility");
            modelBuilder.Entity<Patient>().ToTable("patient");
            modelBuilder.Entity<PatientGuardian>().ToTable("patientGuardian");
            modelBuilder.Entity<PatientAllocation>().ToTable("patientAllocation");
            modelBuilder.Entity<PatientAssignedDementia>().ToTable("patientAssignedDementia");
            modelBuilder.Entity<PerformanceMetricName>().ToTable("performanceMetricName");
            modelBuilder.Entity<PerformanceMetricOrder>().ToTable("performanceMetricOrder");
            modelBuilder.Entity<PersonInCharge>().ToTable("personInCharge");
            modelBuilder.Entity<Prescription>().ToTable("prescription");
            modelBuilder.Entity<PrivacyUserRole>().ToTable("privacyUserRole");
            modelBuilder.Entity<PrivacyLevel>().ToTable("privacyLevel");
            modelBuilder.Entity<PrivacySettings>().ToTable("privacySettings");
            modelBuilder.Entity<ProblemLog>().ToTable("problemLog");
            modelBuilder.Entity<Routine>().ToTable("routine");
            modelBuilder.Entity<Schedule>().ToTable("schedule");
            modelBuilder.Entity<SocialHistory>().ToTable("socialHistory");
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<UserDeviceToken>().ToTable("userDeviceToken");
            modelBuilder.Entity<UserType>().ToTable("userType");
            modelBuilder.Entity<Vital>().ToTable("vital");
        }
    }
}