using GP.Core.Entities;
using GP.Core.IRepository;
using GP.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Service.Repository
{
    public class NotificationRepository : Repository<Notification> ,INotificationRepository
    {
        private readonly StoreContext dbContext;

        public NotificationRepository(StoreContext dbContext) : base(dbContext)
        {
            this.dbContext=dbContext;
        }

      

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await dbContext.notifications
                                 .Where(n => n.UserId == userId)
                                 .OrderByDescending(n => n.DeliveredAt)
                                 .ToListAsync();
        }
    }
}
