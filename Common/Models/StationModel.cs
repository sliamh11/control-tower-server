using Common.Enums;
using System;

namespace Common.Models
{
    public class StationModel : IComparable<StationModel>
    {
        public readonly int Number;
        public int NextStation { get; set; }
        public FlightModel CurrentFlight { get; set; }
        public StationType[] Types { get; set; }
        public StationStatuses Status { get; set; }
        public TimeSpan StandbyPeriod { get; set; }

        public StationModel(int stationNumber, int nextStation, TimeSpan standBy, params StationType[] types)
        {
            Number = stationNumber;
            NextStation = nextStation;
            StandbyPeriod = standBy;
            if (types == null)
            {
                Types = new StationType[1];
                Types[0] = StationType.Normal; // Default value if not entered
            }
            else
            {
                Types = new StationType[types.Length];
                Types = types;
            }
        }

        public int CompareTo(StationModel other)
        {
            // Compare station's 'value' by StandbyPeriod
            return StandbyPeriod.CompareTo(other.StandbyPeriod);
        }
    }
}
