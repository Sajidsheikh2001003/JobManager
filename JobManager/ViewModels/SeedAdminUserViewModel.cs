using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ViewModels
{
    public class SeedAdminUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email", Prompt = "Email Address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "Password")]
        public string Password { get; set; }
    }
}
