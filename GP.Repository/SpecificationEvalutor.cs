using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GP.Core.Entites;
using GP.Core.Specifications;

namespace GP.Repository
{
    public class SpecificationEvalutor<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> spec)
        {
            var Query = inputQuery;
            if (spec.Criteria is not null)
            {
                Query = Query.Where(spec.Criteria);

            }
            if (spec.OrderBy is not null)
            {
                Query = Query.OrderBy(spec.OrderBy);
            }
            if (spec.OrderByDesc is not null)
            {
                Query = Query.OrderByDescending(spec.OrderByDesc);

            }
            if (spec.IsPaginationEnabled)
            {
                Query = Query.Skip(spec.Skip).Take(spec.Take);
            }
            Query = spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));



            return Query;

        }
    }
}
