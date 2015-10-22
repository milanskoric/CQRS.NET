using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SharedKernel
{
    public interface IDomainEvent : IMessage
    {
        string SourceName { get;set;}

        IMessage Payload { get; set; }

        long AggregateRootID { get; set; }

        

        int EntityVersion { get; set; }
    }

    [Serializable]
     public abstract class DomainEvent<TEntity> : IDomainEvent
        where TEntity : CoreObject
    {
         public DomainEvent()
         {
            this.TimeStamp = TimeProvider.GetCurrentUniversalTime();
           
         }

         public DomainEvent(TEntity data)
         {
             this.TimeStamp = TimeProvider.GetCurrentUniversalTime();
             this.AggregateRootID = data.Id;
             this.EntityVersion = data.Version;
             this.Data = data;
             this.CorrelationId = data.CorrelationId;

         }

        public TEntity Data { get; protected set;}

        public DateTime TimeStamp
        {
            get;
            protected set;
        }

        public string CorrelationId
        {
            get;
            set;
        }

        public long AggregateRootID
        {
            get;
            set;
        }

        public int EntityVersion
        {
            get;
            set;
        }

        public string SourceName
        {
            get;
            set;
        }

          [XmlIgnore]
        public IMessage Payload
        {
            get;
            set;
        }
    }

    public abstract class DomainEventHandler<TParameter> : IEventHandler<TParameter>
        where TParameter : IDomainEvent
    {
        protected abstract void Handle(TParameter change, InvocationContext context);
  
        public IDomainEvent Input
        {
            get;
            set;
        }

        public void Execute()
        {
            Handle((TParameter)Input, Context );
        }

       public InvocationContext Context 
        {
            get;
            set;
        }
    }

     // Interface for event handlers - has a type parameters for the event
     public interface IEventHandler<in TParameter> : IEvent
         where TParameter : IDomainEvent
     {
         
     }

     public interface IEvent
     {
         IDomainEvent Input { get; set; }

         void Execute();
     }

    public interface IEventStore  :  IDisposable 
    {
        IEnumerable<IDomainEvent> GetAllEvents();

        void SaveEvents(IDomainEvent domainEvent);
    }

   
}
