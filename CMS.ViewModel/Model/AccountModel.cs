using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CMS.ViewModel.Model
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    public class ChangePassword
    {
        public string USER_ID { get; set; }
        [DisplayName("Confirm Password")]
        [Compare("NEW_PASSWORD", ErrorMessage = "Confirm Password doesn't match..!")]
        [DataType(DataType.Password)]
        public string CONFIRM_PASSWORD { get; set; }
        public string USERNAME { get; set; }
        [DisplayName("New Password")]
        [DataType(DataType.Password)]
        public string NEW_PASSWORD { get; set; }
        [DisplayName("Current Password")]
        [DataType(DataType.Password)]
        public string PASSWORD { get; set; }
        public string DATE_OF_BIRTH { get; set; }
    }
    public class ForgotPassword
    {
        public string USER_ID { get; set; }
        public string EMAIL { get; set; }
        public string ROLL_NO { get; set; }
        public string USERNAME { get; set; }
        public string DATE_OF_BIRTH { get; set; }
    }

    public class USER_ACCOUNT_INFO
    {
        public string USER_ACCOUNT_ID { get; set; }
        public string USER_ID { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public string MOBILE { get; set; }
        public string ROLE { get; set; }
        public string ROLE_ID { get; set; }
        public string PHOTO { get; set; }
        public string USER_TYPE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string ENTRY_ID { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
    }
    public class USER_CHANGE_PASSWORD
    {
        public string USER_ACCOUNT_ID { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string ROLE { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public string MOBILE { get; set; }
        public string LAST_LOGIN { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string USER_ID { get; set; }
        public string USER_TYPE { get; set; }
        public string DATE { get; set; }
        public string TIME { get; set; }
    }

    public class SUP_USER
    {
        public string ROLE_ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string USER_TYPE_ID { get; set; }
        public string USER_TYPE_NAME { get; set; }
    }
    public class USER_ROLES_BY_ACADEMIC_YEAR
    {
        public string USER_ROLE_ID { get; set; }
        public string USER_ID { get; set; }
        public string ROLE_ID { get; set; }
        public string USER_TYPE { get; set; }
        public string ACADEMIC_YEAR { get; set; }
        public string IS_DELETED { get; set; }
        public string IS_ACTIVE { get; set; }
        public string USERNAME { get; set; }
        public string ROLE { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public string MOBILE { get; set; }
        public string PHOTO { get; set; }
        public string USER_ACCOUNT_ID { get; set; }

    }
    public class IP_ADDRESS_LOG
    {
        public string IP_ADDRESS { get; set; }
        public string USERNAME { get; set; }
        public string STATUS { get; set; }
        public string USER_AGENT { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string PLACE { get; set; }
        public string LOGTIME { get; set; }

    }
}
