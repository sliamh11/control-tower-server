using Common.Data_Structures;
using Common.Models;
using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface IStationsManager
    {
        void LoadStations();
        bool StartDeparture(string flightId);
        bool StartLanding(string flightId);
        IReadOnlyList<LinkedList<StationModel>> GetStationsState();
    }
}
