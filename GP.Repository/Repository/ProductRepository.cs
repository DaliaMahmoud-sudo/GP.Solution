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
    public class ProductRepository : Repository<Product> , IProductRepository
    {
        private readonly StoreContext dbContext;

        public ProductRepository(StoreContext dbContext) : base(dbContext)
        {
            this.dbContext=dbContext;
        }
    }
}
