using GP.Core.Entities;
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
    public class PaymentRepository : Repository<Payment>,IPaymentRepository 
    {
        private readonly StoreContext dbContext;



        public PaymentRepository(StoreContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

      
    }
}
