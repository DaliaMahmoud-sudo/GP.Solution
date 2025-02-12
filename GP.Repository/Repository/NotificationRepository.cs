using GP.Core.Entities;
using GP.Core.IRepository;
using GP.Repository.Data;
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
    }
}
