using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ViewModels
{
    public class SetupViewModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Display(Name = "Full Name")]
        [Required]
        public string DisplayName { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
       // public byte Department { get; set; }
       // public List<DepartmentView>? Departments { get; set; }
    }
}
