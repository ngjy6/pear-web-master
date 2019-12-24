using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class CentreViewModel
    {
        public List<centreListViewModel> careCentre { get; set; }
        public int canAddCentre { get; set; }
    }

    public class AddCentreViewModel
    {
        public int centreID { get; set; }

        [Display(Name = "Centre Name")]
        public string centreName { get; set; }

        [Display(Name = "Centre Country")]
        public int centreCountry { get; set; }

        [Display(Name = "Centre Address")]
        public string centreAddress { get; set; }

        [StringLength(6, ErrorMessage = "{0} must be {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Centre Postal Code")]
        public string centrePostalCode { get; set; }

        [StringLength(8, ErrorMessage = "{0} must be {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Centre Contact No")]
        public string centreContactNo { get; set; }

        [EmailAddress]
        [Display(Name = "Centre Email")]
        public string centreEmail { get; set; }

        public IEnumerable<List_Country> Countries { get; set; }
    }

    public class centreListViewModel
    {
        public int centreID { get; set; }
        public string centreName { get; set; }
        public string centreCountry { get; set; }
        public string centreAddress { get; set; }
        public string centrePostalCode { get; set; }
        public string centreContactNo { get; set; }
        public string centreEmail { get; set; }

        public string monStartTime { get; set; }
        public string monEndTime { get; set; }

        public string tueStartTime { get; set; }
        public string tueEndTime { get; set; }

        public string wedStartTime { get; set; }
        public string wedEndTime { get; set; }

        public string thursStartTime { get; set; }
        public string thursEndTime { get; set; }

        public string friStartTime { get; set; }
        public string friEndTime { get; set; }

        public string satStartTime { get; set; }
        public string satEndTime { get; set; }

        public string sunStartTime { get; set; }
        public string sunEndTime { get; set; }
    }

    public class ViewCentreViewModel
    {
        [Display(Name = "Centre ID")]
        public int centreID { get; set; }

        [Display(Name = "Centre Name")]
        public string centreName { get; set; }

        [Display(Name = "Centre Country")]
        public string centreCountry { get; set; }

        [Display(Name = "Centre Address")]
        public string centreAddress { get; set; }

        [StringLength(6, ErrorMessage = "{0} must be {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Centre Postal Code")]
        public string centrePostalCode { get; set; }

        [StringLength(8, ErrorMessage = "{0} must be {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Centre Contact No")]
        public string centreContactNo { get; set; }

        [EmailAddress]
        [Display(Name = "Centre Email")]
        public string centreEmail { get; set; }

        public string mon { get; set; }
        public string monStartTime { get; set; }
        public string monEndTime { get; set; }

        public string tue { get; set; }
        public string tueStartTime { get; set; }
        public string tueEndTime { get; set; }

        public string wed { get; set; }
        public string wedStartTime { get; set; }
        public string wedEndTime { get; set; }

        public string thurs { get; set; }
        public string thursStartTime { get; set; }
        public string thursEndTime { get; set; }

        public string fri { get; set; }
        public string friStartTime { get; set; }
        public string friEndTime { get; set; }

        public string sat { get; set; }
        public string satStartTime { get; set; }
        public string satEndTime { get; set; }

        public string sun { get; set; }
        public string sunStartTime { get; set; }
        public string sunEndTime { get; set; }
    }
}