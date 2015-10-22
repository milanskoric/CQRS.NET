﻿using System;
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


    //Represent 
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
 
 
 

    public interface IRepository : IRepository<CoreObject, long>
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
        Result<TEntity> Save<TEntity>(TEntity entity, InvocationContext context)
            where TEntity : TEntityCore;

        IQueryable GetQueryable(Type t);

        IQueryable<TEntity> GetQueryable<TEntity>()
            where TEntity : TEntityCore;
    }

    public interface IUnitOfWork : IDisposable
    {

    }

    public interface IUnitOfWork<TKey> : IUnitOfWork
    {
        void Commit();

        void Set<TEntity>(TEntity entity, InvocationContext context)
            where TEntity : IEntity<TKey>;

        Result Result { get;}
    }
}
