using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        //[Remote(action: "IsEmailInUse", controller: "Administration", AdditionalFields = nameof(Id))]
        public string Email { get; set; }

        [Display(Name = "Confirm Email?", Prompt = "Confirm Email?")]
        public bool IsEmailConfirmed { get; set; }

        [Display(Name = "Full Name", Prompt = "Full Name")]
        public string? DisplayName { get; set; }
        [Display(Name = "New Password", Prompt = "New Password")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }
    }
}
