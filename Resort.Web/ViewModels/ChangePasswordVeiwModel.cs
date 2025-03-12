using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Resort.Web.ViewModels
{
    public class ChangePasswordVeiwModel
    {
        [Required]
        public string  CurrentPassword  { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string  NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }
}
