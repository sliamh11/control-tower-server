using Common.Data_Structures;
using Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ITowerManager
    {
        void AddStation(List<StationModel> newStation);
        Task<bool> StartDepartureAsync(string flightId);
        Task<bool> StartLandingAsync(string flightId);
        IReadOnlyList<IReadOnlyList<StationModel>> GetStationsState();
    }
}
