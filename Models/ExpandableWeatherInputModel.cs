using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RundooApi.Models
{
    public class ExpandableWeatherInputModel
    {
        public string StationName { get; set; }
        
        public string ObservationDate { get; set; }

        public string ObservationTime { get; set; }

    }
}
