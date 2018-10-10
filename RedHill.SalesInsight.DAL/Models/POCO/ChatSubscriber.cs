using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.Models.POCO
{
    public class ChatSubscriber
    {
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool CanBeRemoved { get; set; }
    }
}
