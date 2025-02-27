using GP.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Message { get; set; }
        public enum MessageStatus
        {
            [EnumMember(Value = "Read")]
            Read,
            [EnumMember(Value = "Unread")]
            Unread,
        }
        public MessageStatus Status { get; set; }
        public DateTimeOffset DeliveredAt { get; set; }
        
    }
}
