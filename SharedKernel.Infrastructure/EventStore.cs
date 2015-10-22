using Newtonsoft.Json;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure
{
    public class EventStore : IEventStore 
    {
        EventsContext _context = null;

        public EventStore()
        {
            _context = new EventsContext();

            _context.Database.CreateIfNotExists();
        }

        public  IEnumerable<IDomainEvent> GetAllEvents()
        {
            List<IDomainEvent> output = new List<IDomainEvent>();

            var itmes = _context.EventDescriptors.ToList();

            foreach (var ed in itmes)
            {
                using (Stream stream = new MemoryStream(ed.Data.SerializedValue))
                {
                    output.Add(EventSerializer.Deserialize(stream));
                }
            }

            return output;
        }

        public void SaveEvents(IDomainEvent domainEvent)
        {
            StoreValue data = new StoreValue();

            data.StringValue = JsonConvert.SerializeObject(domainEvent);
            data.TypeName = domainEvent.GetType().FullName;
            data.SerializedValue = EventSerializer.Serialize(domainEvent);

            StoreValue payload = new StoreValue();


            payload.StringValue = JsonConvert.SerializeObject(domainEvent.Payload);
            payload.TypeName = domainEvent.Payload.GetType().FullName;
            payload.SerializedValue = EventSerializer.Serialize(domainEvent.Payload);

            EventDescriptor ed = new EventDescriptor(
                domainEvent.GetType().Name,
                domainEvent.AggregateRootID,
                data, 
                domainEvent.SourceName,
                payload,
                domainEvent.EntityVersion,
                domainEvent.CorrelationId);

            _context.EventDescriptors.Add(ed);

            _context.SaveChanges();
        }

        public void Dispose()
        {
            if (_context != null)
                _context.Dispose();
        }
    }

    public static class EventSerializer
    {
        public static IDomainEvent Deserialize(Stream stream)
        {
            var formatter = new BinaryFormatter();
            return (IDomainEvent)formatter.Deserialize(stream);
        }

        public static byte[] Serialize(object domainEvent)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, domainEvent);
                return stream.ToArray();
            }
        }
    }

    internal class EventsContext : DataContext
    {
        public static readonly string Identifier = "EventsConnection";
        public static readonly string SchemaName = "events";

        public EventsContext()
            : base("name=" + EventsContext.Identifier, EventsContext.SchemaName)
        {
            Database.SetInitializer<EventsContext>(new DropCreateDatabaseIfModelChanges<EventsContext>());
        }

        public DbSet<EventDescriptor> EventDescriptors { get; set; }
    }

    public class EventDescriptor
    {
        private EventDescriptor()
        { 
        
        }

        public EventDescriptor(string name, long aggregateRootId, StoreValue data, string sourceName, StoreValue payload, int dataVersion, string corelationId)
        {
            this.Name = name;
            this.AggregateRootId = aggregateRootId;
            this.Data = data;
            this.SourceName = sourceName;
            this.Payload = payload;
            this.DataVersion = dataVersion;
            this.CorelationId = corelationId;
        }

          [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id {get;set;}

        public string Name { get; set; }

        public string SourceName { get; set; }

        public long AggregateRootId { get; set; }

        public StoreValue Data {get;set;}

        public StoreValue Payload { get; set; }

        public int DataVersion { get; set; }

        public int Version { get; set; }

        public string CorelationId {get;set;}
    }
}
