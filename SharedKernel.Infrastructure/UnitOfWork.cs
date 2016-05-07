using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure
{
    public class UnitOfWork : IUnitOfWork 
    {
        protected DataContext dataContext = null;

        public UnitOfWork(DataContext dataContext, IRepository reposiotry)
        {
            this.dataContext = dataContext;

            this.Repository = reposiotry;
        }

        public IRepository Repository
        {
            get;
            protected set;
        }


        public virtual void Set<TEntity>(TEntity entity, InvocationContext context)
            where TEntity : AggregateRoot
        { 

            var dbSet = this.dataContext.Set<TEntity>();

            bool IsAdded = entity.IsTransient();

            if (IsAdded)
            {
                dbSet.Add(entity);
            }
            else
            {
                DbEntityEntry dbEntityEntry = this.dataContext.Entry(entity);

                if (dbEntityEntry.State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }

                this.dataContext.Entry(entity).State = EntityState.Modified;
            } 
        }

        public virtual Result<TEntity> Save<TEntity>(TEntity entity, InvocationContext context)
    where TEntity : AggregateRoot
        {
            Result<TEntity> output = new Result<TEntity>();

            this.Set(entity, context);

            this.Commit();

            output.RecordsAffected = this.Rows;
            output.Success = this.Rows > 0;

            if (output.Success)
            {
                output.Data = entity;
                output.ObjectId = entity.Id.ToString();
            }
             
            return output;
        }

        public virtual void Commit()
        {
            this.Rows = this.dataContext.SaveChanges();
        }

        internal int Rows
        {
            get;
            set;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (Repository!=null)
                        Repository.Dispose();
                    if (dataContext != null)
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
