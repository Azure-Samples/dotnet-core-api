using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RundooApi.Models
{
    public class WeatherInputModel
    {

        public string StationName { get; set; }

        public string ObservationDate { get; set; }

        public string ObservationTime { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double Barometer { get; set; }

        public string WindDirection { get; set; }

        public double WindSpeed { get; set; }

        public double Precipitation { get; set; }

    }
}
