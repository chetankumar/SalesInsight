using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ChangePasswordView
    {
     
        public int NewUser;

        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public bool UserFound { get; set; }

        public ChangePasswordView()
        {
        }

        public ChangePasswordView(string username, string token, int newUser = 0)
        {
            this.UserName = username;
            this.Token = token;
            this.NewUser = newUser;
        }

    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}