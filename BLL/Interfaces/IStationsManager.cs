using Common.Data_Structures;
using Common.Models;

namespace BLL.Interfaces
{
    public interface IStationsManager
    {
        void LoadStations();
        bool StartDeparture(string flightId);
        bool StartLanding(string flightId);
        StationsGraph GetStationsState();
    }
}
