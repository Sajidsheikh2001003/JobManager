using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Read", "Read"),
            new Claim("Create", "Create"),
            new Claim("Update", "Update"),
            new Claim("Delete","Delete"),
            //new Claim("Print","Print")
        };
    }
}
