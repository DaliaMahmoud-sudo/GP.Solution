using GP.Core.Entites.OrderAggregate;
using GP.Core.IRepository;
using GP.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Service.Repository
{
    public class OrderRepository : Repository<Order> , IOrderRepository
    {
        private readonly StoreContext dbContext;

        public OrderRepository(StoreContext dbContext) : base(dbContext)
        {
            this.dbContext=dbContext; 
        }
    }
}
