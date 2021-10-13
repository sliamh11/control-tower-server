using Common.Enums;
using System;

namespace Common.Models
{
    public class FlightModel
    {
        // [Key]
        public string Id { get; private set; } // -> same as stationsId problem
        public DateTime? DepartureTime { get; set; }
        public DateTime? LandingTime { get; set; }
        public FlightType Type { get; set; }

        public FlightModel() { }
        public FlightModel(string id, FlightType type) : this()
        {
            Id = id;
            Type = type;
        }
    }
}
