using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Specifications
{
    public class ProductSpecParams
    {
        public  string? Sort {  get; set; }
        public string? Name { get; set; }
        

        private  int  pageSize=10;

        public int PageSize
        {
            get { return   pageSize; }
            set { pageSize = value>10 ? 10 : value; }
        }
        public int PageIndex { get; set; } = 1;


    }
}
