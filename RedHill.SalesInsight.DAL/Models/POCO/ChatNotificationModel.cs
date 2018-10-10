using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.Models.POCO
{
    public class ChatNotificationModel
    {
        public List<ChatNotificationItem> ChatNotifications { get; set; }

        public int UnseenCount { get; set; }
    }
}
