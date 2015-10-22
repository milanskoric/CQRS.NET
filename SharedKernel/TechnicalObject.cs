using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
namespace SharedKernel
{
    [DataContract]
    [Serializable]
    public abstract class CoreObject : IEntity<long>, IEventSourcedEntity
    {
        /// <summary>
        ///     Get or Set Primary Id
        ///     All operational tables have one numeric column Id as Primary Key
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DataMember]
        public long Id
        {
            get;
            set;
        }

        public string CorrelationId { get; set; }

        private Guid _Guid = Guid.Empty;

        /// <summary>
        ///    Get the persistent object identifier if exist 
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [XmlIgnore]
        [ScriptIgnore]
        public Guid Guid
        {
            get { return _Guid; }
            set
            {
                if (IsTransient())
                    _Guid = value;
            }
        }

        /// <summary>
        ///     Check if this entity is transient, ie, without identity at this moment
        /// </summary>
        /// <returns>True if entity is transient, else false</returns>
        public bool IsTransient()
        {
            return this.Guid == Guid.Empty;
        }

        protected virtual void ApplyChange(IDomainEvent change)
        {
            if (change != null)
            {
                changeEvents.Add(change);
            }
        }

        [DataMember]
        public int Version { get;set;}

        [XmlIgnore]
        [JsonIgnore]
        [IgnoreDataMember]
        [ScriptIgnore]
        private readonly HashSet<IDomainEvent> changeEvents = new HashSet<IDomainEvent>();

        [XmlIgnore]
        [JsonIgnore]
        [IgnoreDataMember]
        [ScriptIgnore]
        public IEnumerable<IDomainEvent> Changes
        {
            get { return changeEvents; }
        }

        public bool HasChanges()
        {
            return changeEvents != null && changeEvents.Count > 0;

        }
    }
}
