using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class PasswordHistoryLimit
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
       
    }
}
