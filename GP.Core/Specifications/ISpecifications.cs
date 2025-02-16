using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GP.Core.Entites;

namespace GP.Core.Specifications
{
    public interface ISpecifications<T>
    {
        //where condition
        public Expression<Func<T, bool>> Criteria { get; set; }

        //list of includes
        public List<Expression<Func<T, Object>>> Includes { get; set; }

        //prop for orderBy
        public Expression<Func<T, object>> OrderBy { get; set; }

        //prop for orderByDesc
        public Expression<Func<T, Object>> OrderByDesc { get; set; }

        //take (2)
        public int Take { get; set; }

        //skip(2)
        public int Skip { get; set; }

        public bool IsPaginationEnabled { get; set; } 
    }
}
