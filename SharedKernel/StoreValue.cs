using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    [ComplexType]
    [Serializable]
    public class StoreValue
    {
        public string XmlValue { get; set; }

        public string StringValue { get; set; }

        public string TypeName {get;set;}

        public byte[] SerializedValue { get; set; }
    }
}
