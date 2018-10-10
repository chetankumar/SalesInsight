using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class LoginView
    {
        [Required(ErrorMessage = "Please enter your username")]
        public String Username { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        public String Password { get; set; }
        public string RedirectUrl { get; set; }
    }
}