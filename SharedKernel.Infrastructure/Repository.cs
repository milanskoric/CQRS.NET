using System;
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
        protected DataContext dataContext = null;

        public Repository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public virtual Result<TEntity> Save<TEntity>(TEntity entity, InvocationContext context)
            where TEntity : CoreObject
        {
            Result<TEntity> output = new Result<TEntity>();

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

            int rows = this.dataContext.SaveChanges();

            output.RecordsAffected = rows;
            output.Data = entity;
            output.ObjectId = entity.Id.ToString();
            output.Success = true;

            //output.ApplyChangeEvent<T>(entity);

            return output;
        }

        public void Dispose()
        {
            dataContext.Dispose();
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
            where TEntity : CoreObject
            
        {
            DbQuery<TEntity> set;

            set = (DbQuery<TEntity>)this.dataContext.Set<TEntity>();
        

            IQueryable<TEntity> output = set;

            return output; 
                  
        }
    }
}
