using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIUserPasswordVerification
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string PasswordVerificationToken { get; set; }

        public DateTime PasswordVerificationTokenExpirationDate { get; set; }
    }
}
