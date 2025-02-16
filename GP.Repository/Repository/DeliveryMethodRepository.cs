using GP.Core.Entites.OrderAggregate;
using GP.Core.IRepository;
using GP.Repository.Data;
using GP.Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Repository.Repository
{
    public class DeliveryMethodRepository : Repository<DeliveryMethod> , IDeliveryMethodRepository
    {
        private readonly StoreContext dbContext;



        public DeliveryMethodRepository(StoreContext dbContext) : base(dbContext)
        {
            this.dbContext=dbContext;


        }
    }
}
