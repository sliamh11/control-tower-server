using Common.Data_Structures;
using Common.Models;
using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface IStationsState
    {
        void LoadStations();
        void StateUpdated();
        IReadOnlyList<IReadOnlyList<StationModel>> GetStationsState();
        void AddStation(List<StationModel> newStation);
    }
}
