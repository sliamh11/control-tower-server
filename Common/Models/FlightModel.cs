using Common.Enums;
using System;

namespace Common.Models
{
    public class FlightModel
    {
        public readonly string Id;
        public DateTime? DepartureTime { get; set; }
        public DateTime? LandingTime { get; set; }
        public FlightStatuses Status { get; set; }

        public FlightModel(string flightId)
        {
            Id = flightId;
        }
    }
}
