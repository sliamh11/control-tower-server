using Common.Enums;
using System;

namespace Common.Models
{
    public class FlightModel
    {
        public readonly Guid Id;
        public DateTime? DepartureTime { get; set; }
        public DateTime? LandingTime { get; set; }
        public StationModel CurrentStation { get; set; }
        public FlightStatuses Status { get; set; }

        public FlightModel()
        {
            Id = Guid.NewGuid();
        }

        public bool IsLanding()
        {
            if (DepartureTime != null)
                return DateTime.Now > DepartureTime.Value;

            throw new Exception("DepartureTime is null.");
        }
    }
}
