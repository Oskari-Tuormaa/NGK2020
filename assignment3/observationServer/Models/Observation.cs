using System;

namespace observationServer.Models
{
    public class Observation
    {
        private float temperature;
        private int humidity;
        private float pressure;

        public int Id { get; set; }
        public DateTime Date { get; set; }           // Required
        public string Name { get; set; }             // Required
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public float Temperature
        {
            get { return temperature; }
            set { temperature = (float)System.Math.Round(value, 1); } // Round value to 1 digit
        }

        public int Humidity
        {
            get { return humidity; }
            set { humidity = (0 <= value && value <= 100) ? value : 0; } // Ensure that value is between 0-100
        }

        public float Pressure
        {
            get { return pressure; }
            set { pressure = (float)System.Math.Round(value, 1); } // Round value to 1 digit
        }
    }
}
