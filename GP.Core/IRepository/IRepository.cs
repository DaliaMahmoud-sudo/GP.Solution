using GP.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.IRepository
{
    public interface IRepository<T> where T : class
    {
     
       

        public IEnumerable<T> Get( Expression<Func<T, object>>[]? includeProps = null,Expression<Func<T, bool>>? expression = null, bool tracked = true);
        T? GetOne(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true);
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);
        void Create(T entity);

        void Edit(T entity);
      
        void Delete(T entity);

        void Commit();

        //prop for orderBy
        public Expression<Func<T, object>> OrderBy { get; set; }

        //prop for orderByDesc
        public Expression<Func<T, Object>> OrderByDesc { get; set; }
       

    }
}
