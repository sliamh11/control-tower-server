using Common.Enums;
using Common.Models;
using Common.Structs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IStationsState
    {
        void StateUpdated();
        IReadOnlyList<IReadOnlyDictionary<string,StationModel>> GetStationsState();
        bool AddStation(Dictionary<string,StationModel> newStations);
        bool IsStationEmpty(StationModel station);
        StationsPathModel FindFastestPath(int startIndex, int targetIndex);
        StationsPathModel FindFastestPath(StationModel currStation, StationModel targetStation);
        bool MoveToStation(StationModel fromStation, StationModel toStation, FlightModel flight);
        PathEdgesStruct GetPathEdgeStations(FlightModel flight);
        bool RemoveFlight(StationModel station);
        bool CanAddFlight(FlightType type);
        bool UpdateStation(StationModel updatedStation);
    }
}
