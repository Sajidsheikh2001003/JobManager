using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role Name", Prompt = "Enter New Role Name", Description = "Enter new role name")]

        public string RoleName { get; set; }
    }
}
