using System;

namespace AspSecure.Models
{
  public class Observation
  {
    public int id { get; set; }
    public DateTime Date { get; set; }
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public float Temperature { get; set; }
    public float Humidity { get; set; }
    public float Pressure { get; set; }  
  }
}
