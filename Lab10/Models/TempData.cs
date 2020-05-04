using System;

namespace aspnet.Models
{
  public class TempData
  {
    public DateTime Date { get; set; }
    public float Temperature { get; set; }
    public float Humidity { get; set; }
    public float Pressure { get; set; }

    public TempData Clone()
    {
      return this.MemberwiseClone() as TempData;
    }
  }
}
