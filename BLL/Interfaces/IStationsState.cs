﻿using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;

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
        Tuple<StationModel, StationModel> GetPathEdgeStations(FlightModel flight);
        bool RemoveFlight(StationModel station);
        bool CanAddFlight(FlightType type);
        bool UpdateStation(StationModel updatedStation);
    }
}
