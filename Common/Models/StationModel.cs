using Common.Enums;
using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class StationModel
    {
        public string Id { get; set; } 
        public int Number { get; set; }
        public int NextStation { get; set; }
        public FlightModel CurrentFlight { get; set; }
        public ICollection<StationType> Types { get; set; }
        public StationStatuses Status { get; set; }
        public TimeSpan StandbyPeriod { get; set; }

        public StationModel() { }

        public StationModel(int nextStation, TimeSpan standBy, params StationType[] types) : this()
        {
            Id = Guid.NewGuid().ToString();
            Number = -1; // Default value.
            NextStation = nextStation;
            StandbyPeriod = standBy;
            if (types == null)
            {
                Types = new StationType[] { StationType.Normal };
            }
            else
            {
                Types = new StationType[types.Length];
                Types = types;
            }
        }

        public StationModel(int existingStation, int nextStation, TimeSpan standBy, params StationType[] types) : this(nextStation, standBy, types)
        {
            Number = existingStation;
        }
    }
}
