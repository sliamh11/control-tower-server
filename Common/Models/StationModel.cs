using Common.Enums;

namespace Common.Models
{
    public class StationModel
    {
        public readonly int Number;
        public int NextStation { get; set; }
        public FlightModel CurrentFlight { get; set; }
        public StationStatuses Status { get; set; }
        
        public StationModel(int stationNumber, int nextStation)
        {
            Number = stationNumber;
            NextStation = nextStation;
        }
    }
}
