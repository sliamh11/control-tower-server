using Common.Data_Structures;
using Common.Models;
using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface ITowerManager
    {
        void AddStation(List<StationModel> newStation);
        bool StartDeparture(string flightId);
        bool StartLanding(string flightId);
        IReadOnlyList<IReadOnlyList<StationModel>> GetStationsState();
    }
}
