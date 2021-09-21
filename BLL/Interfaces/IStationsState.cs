using Common.Enums;
using Common.Models;
using System;
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
        void MoveToStation(StationModel fromStation, StationModel toStation, FlightModel flight);
        Tuple<StationModel, StationModel> GetPathEdgeStations(FlightModel flight);
        void RemoveFlight(StationModel station);
        bool CanAddFlight(FlightType type);
    }
}
