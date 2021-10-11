using Common.Enums;
using System;

namespace Common.Models
{
    public class StationModel : IComparable<StationModel>
    {
        //[Key]
        public string Id { get; private set; } // Competible with EF -> some say internal and not private, check
        public int Number { get; set; }
        public int NextStation { get; set; }
        public FlightModel CurrentFlight { get; set; }
        public StationType[] Types { get; set; }
        public StationStatuses Status { get; set; }
        public TimeSpan StandbyPeriod { get; set; }

        public StationModel(int nextStation, TimeSpan standBy, params StationType[] types)
        {
            Id = Guid.NewGuid().ToString();
            Number = -1; // Default value.
            NextStation = nextStation;
            StandbyPeriod = standBy;
            if (types == null)
            {
                Types = new StationType[1];
                Types[0] = StationType.Normal; // Default value.
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

        public int CompareTo(StationModel other)
        {
            // Compare station's 'value' by StandbyPeriod
            return StandbyPeriod.CompareTo(other.StandbyPeriod);
        }
    }
}
