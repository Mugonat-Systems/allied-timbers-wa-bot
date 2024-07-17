using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlliedTimbers.Models;

namespace AlliedTimbers.ViewModel
{
    public class RoleViewModel
    {
        public ApplicationUser User { get; set; }
        public RoleViewModel(ApplicationUser user)
        {
            User = user;
        }

       
    }

   
}