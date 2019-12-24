using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Dtos
{
    public class LoginDto 
    {
        public int userID { get; set; }
        public int userTypeID { get; set; }
        public string token { get; set; }
    }
}