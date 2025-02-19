using GP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.IRepository
{
    public interface IUserCartRepository : IRepository<UserCart>
    {


       // public void CreateCart(string userId);
        public void AddProductToCart(string userId, int productId, int quantity);




    }
}
