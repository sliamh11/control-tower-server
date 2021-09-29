using Common.Data_Structures;
using Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ITowerManager
    {
        bool AddStation(Dictionary<string,StationModel> newStations);
        Task<bool> StartDepartureAsync(string flightId);
        Task<bool> StartLandingAsync(string flightId);
        IReadOnlyList<IReadOnlyDictionary<string, StationModel>> GetStationsState();
    }
}
