using System.ComponentModel.DataAnnotations;

namespace CloudService.Web.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is required to be repeated twice")]
        public string PasswordRepeat { get; set; }
    }
}
