using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RundooApi.Entities
{
    public class UpdateWeatherObject
    {

        public Dictionary<string, object> _properties = new Dictionary<string, object>();

        public string StationName { get; set; }

        public string ObservationDate { get; set; }

        public string Etag { get; set; }

        public object this[string name]
        {
            get => (ContainsProperty(name)) ? _properties[name] : null;
            set => _properties[name] = value;
        }

        public ICollection<string> PropertyNames => _properties.Keys;

        public int PropertyCount => _properties.Count;

        public bool ContainsProperty(string name) => _properties.ContainsKey(name);
    }
}
