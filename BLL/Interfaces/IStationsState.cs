using Common.Data_Structures;
using Common.Models;
using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface IStationsState
    {
        void StateUpdated();
        IReadOnlyList<IReadOnlyList<StationModel>> GetStationsState();
        void AddStation(List<StationModel> newStation);
        bool IsStationEmpty(StationModel station);
        StationsPathModel FindFastestPath(int startIndex, int targetIndex);
        StationsPathModel FindFastestPath(StationModel currStation, StationModel targetStation);
    }
}
