using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Observation
    {
        private float temperature;
        private int humidity;
        private float pressure;
        public int ObservationId { get; set; }
        public DateTime Date { get; set; }
        public Location Location { get; set; }

        public float Temperature
        {
            get { return temperature; }
            set { temperature = (float)System.Math.Round(value, 1); }
        }
        public int Humidity
        {
            get { return humidity; }
            set { humidity = (0 <= value && value <= 100) ? value : 0; }

        }

        public float Pressure
        {
            get { return pressure; }
            set { pressure = (float)System.Math.Round(value, 1); }
        }
    }
}
