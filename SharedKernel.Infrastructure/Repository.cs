using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure
{
    public abstract class Repository : IRepository
    {
        protected readonly DataContext dataContext = null;

        public Repository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public bool Build()
        {
            return this.dataContext.Database.CreateIfNotExists();
        }


        public IQueryable GetQueryable(Type t)
        {
            if (t == null)
                return null;

            MethodInfo mi = Extensions.InjectTypeToGenericMethod(this, "GetQueryable", t);

            Object[] parameters = new Object[] {};

            return (IQueryable)mi.Invoke(this, parameters);
        }

        public IQueryable<TEntity> GetQueryable<TEntity>()
            where TEntity : AggregateRoot
            
        {
            DbQuery<TEntity> set;

            set = (DbQuery<TEntity>)this.dataContext.Set<TEntity>();
        

            IQueryable<TEntity> output = set;

            return output; 
                  
        }

        public IQueryable<TEntity> BuildQuery<TEntity>(QueryOptions options)
           where TEntity : AggregateRoot
        {
            var q = this.GetQueryable<TEntity>();

            if (options != null)
            {
                if (options.HasLimit())
                {
                    q = q.Take(options.GetLimit());
                }
            }

            return q;
        }

        public IEnumerable<TEntity> ExecuteList<TEntity>(IQueryable<TEntity> query)
     where TEntity : AggregateRoot
        {
            return query.ToList();
        }

        public TEntity ExecuteSingle<TEntity>(IQueryable<TEntity> query)
            where TEntity : AggregateRoot
        {
            return query.SingleOrDefault();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dataContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
