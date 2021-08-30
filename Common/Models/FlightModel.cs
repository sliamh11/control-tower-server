using Common.Enums;
using System;

namespace Common.Models
{
    public class FlightModel
    {
        public readonly string Id;
        public DateTime? DepartureTime { get; set; }
        public DateTime? LandingTime { get; set; }
        public FlightType Type { get; set; }

        public FlightModel(string flightId, FlightType type)
        {
            Id = flightId;
            Type = type;
        }
    }
}
