using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class LocationDto
    {
        public LocationDto(Location l)
        {
            Name = l.Name;
            Latitude = l.Latitude;
            Longitude = l.Longitude;
        }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
