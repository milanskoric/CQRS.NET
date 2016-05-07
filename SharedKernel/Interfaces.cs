using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{

    public interface ICommandBus : IDisposable
    {
        Result Send(ICommandMessage command, InvocationContext context);

        void Publish(IDomainEvent change, InvocationContext context);

        void Replay(IDomainEvent change);
        
    }

    //Represent base massage
    public interface IMessage
    {
        DateTime TimeStamp { get; }

        string CorrelationId { get; }
    }


    //Represent entity
    public interface IEntity
    {

    }

    public interface IEventSourcedEntity
    {
        int Version { get; set; }

        IEnumerable<IDomainEvent> Changes { get; }
    }

    public interface IEntity<TKey> : IEntity
    {
        TKey Id { get; set; }
    }
 
 
 

    public interface IRepository : IRepository<AggregateRoot, long>
    { 
    
    }

    public interface IBootstrapper
    {
        void Initialize(Microsoft.Practices.Unity.IUnityContainer container);

        bool Build();

        void Seed();
    }

    public interface IRepository<TEntityCore, TKey> :  IDisposable
        where TEntityCore : IEntity<TKey> 
    {
        IQueryable GetQueryable(Type t);

        IQueryable<TEntity> BuildQuery<TEntity>(QueryOptions options)
           where TEntity : TEntityCore;

        IQueryable<TEntity> GetQueryable<TEntity>()
            where TEntity : TEntityCore;

        IEnumerable<TEntity> ExecuteList<TEntity>(IQueryable<TEntity> query)
          where TEntity : TEntityCore;

        TEntity ExecuteSingle<TEntity>(IQueryable<TEntity> query)
            where TEntity : TEntityCore;
    }

    public interface IUnitOfWork : IUnitOfWork<AggregateRoot, long>
    {

    }

    public interface IUnitOfWork<TEntityCore, TKey> : IDisposable
        where TEntityCore : IEntity<TKey> 
    {
        void Commit();

        void Set<TEntity>(TEntity entity, InvocationContext context)
            where TEntity : TEntityCore;

        Result<TEntity> Save<TEntity>(TEntity entity, InvocationContext context)
            where TEntity : TEntityCore;

    }
}
