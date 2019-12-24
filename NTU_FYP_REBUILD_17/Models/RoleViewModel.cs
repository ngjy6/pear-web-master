using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class RoleViewModel
    {
        public RoleViewModel() { }

        public RoleViewModel(UserType role)
        {
            Id = role.userTypeID;
            Name = role.userTypeName;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<UserType> UserTypes { get; set; }
    }
}