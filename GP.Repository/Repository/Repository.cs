using GP.Core.Entites.OrderAggregate;
using GP.Core.IRepository;
using GP.Core.Specifications;
using GP.Repository;
using GP.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GP.Service.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly StoreContext dbContext;
        private DbSet<T> dbSet;
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }
        public Repository(StoreContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<T>();
        }

        // CRUD operations
        public IEnumerable<T> Get( Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true)
        {
           
            

            IQueryable<T> query = dbSet;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includeProps != null)
            {
                foreach (var prop in includeProps)
                {
                    query = query.Include(prop);
                }
            }

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            return query.ToList();

        }

        public T? GetOne(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true)
        {
            return Get(includeProps, expression, tracked).FirstOrDefault();
        }
        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> Spec)
        {
            return await ApplySpecification(Spec).ToListAsync();
        }

        public void Create(T entity)
        {
            dbSet.Add(entity);
        }

        public void Edit(T entity)
        {
            dbSet.Update(entity);
        }
     
        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Commit()
        {
            dbContext.SaveChanges();
        }
        private IQueryable<T> ApplySpecification(ISpecifications<T> Spec)
        {
            return SpecificationEvalutor<T>.GetQuery(dbContext.Set<T>(), Spec);
        }
        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;

        }
        public void AddOrderByDesc(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDesc = orderByDescExpression;
        }
       

    }
}
