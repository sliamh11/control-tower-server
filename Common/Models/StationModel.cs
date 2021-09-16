using Common.Enums;
using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class StationModel : IComparable<StationModel>
    {
        public readonly int Number;
        public int NextStation { get; set; }
        public FlightModel CurrentFlight { get; set; }
        public StationType Type { get; set; }
        public StationStatuses Status { get; set; }
        public TimeSpan StandbyPeriod { get; set; }

        public StationModel(int stationNumber, int nextStation, TimeSpan standBy, StationType type = StationType.Normal)
        {
            Number = stationNumber;
            NextStation = nextStation;
            StandbyPeriod = standBy;
        }

        public int CompareTo(StationModel other)
        {
            // If result = 0 -> stations are the same. no such thing bigger / smaller station.
            return StandbyPeriod.CompareTo(other.StandbyPeriod);
        }
    }
}
