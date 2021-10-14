using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace RundooApi.Models
{
    public class WeatherDataModel 
    {
        // Captures all of the weather data properties -- temp, humidity, wind speed, etc
        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        public string StationName { get; set; }

        public string ObservationDate { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public string Etag { get; set; }

        public object this[string name] 
        { 
            get => ( ContainsProperty(name)) ? _properties[name] : null; 
            set => _properties[name] = value; 
        }

        public ICollection<string> PropertyNames => _properties.Keys;

        public int PropertyCount => _properties.Count;

        public bool ContainsProperty(string name) => _properties.ContainsKey(name);       
    }
}
