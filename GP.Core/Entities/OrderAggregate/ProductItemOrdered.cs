using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entites.OrderAggregate
{
    public class ProductItemOrdered
    {
        public ProductItemOrdered()
        {
            
        }
        public ProductItemOrdered(int productId, string productName, string ImageUrl)
        {
            ProductId = productId;
            ProductName = productName;
            ImageUrl = ImageUrl;
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
    }
}
