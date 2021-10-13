using Common.Models;
using System.Collections.Generic;

namespace DAL
{
    public interface ITowerRepository
    {
        IEnumerable<StationModel> GetStations();
        void SaveNewStation(StationModel station);
        void SaveNewFlight(FlightModel flight);
        void SaveMovementHistory(StationModel fromStation, StationModel toStation);
    }
}