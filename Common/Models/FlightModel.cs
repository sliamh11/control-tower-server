using Common.Enums;
using System;

namespace Common.Models
{
    public class FlightModel
    {
        public string Id { get; set; }
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
