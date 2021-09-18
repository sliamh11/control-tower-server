﻿using BLL.Data_Objects;
using BLL.Interfaces;
using Common.Exceptions;
using Common.Models;
using System;

namespace BLL.Logic
{
    // Instance created per request (scoped).
    // MAKE THIS CLASS METHODS ASYNC
    internal class LandingLogic : ILandingLogic
    {
        private IStationsState _stationsState;
        public LandingLogic(IStationsState stationsState)
        {
            _stationsState = stationsState;
        }

        public bool StartLanding(LandingObj landingObj)
        {
            // 'Fastest' start & end points (by StandbyPeriod)
            var pathEdges = _stationsState.GetPathEdgeStations(landingObj.Flight);
            if (pathEdges == null)
                return false;

            // Fastest path between the points
            var path = _stationsState.FindFastestPath(pathEdges.Item1, pathEdges.Item2);
            if (path == null)
                return false;

            landingObj.StationsPath = path;
            landingObj.Flight.LandingTime = DateTime.Now + landingObj.StationsPath.OverallTime;
            _stationsState.MoveToStation(null, pathEdges.Item1, landingObj.Flight);
            return true;
        }

        private bool CanFinishLanding(LandingObj landingObj)
        {
            // If current object's station is the last one / queue is empty (What will be more generic and better?) - return true.
            if(landingObj.StationsPath.CurrentStation)

            return true;
        }
        public bool FinishLanding(LandingObj landingObj)
        {
            if (CanFinishLanding(landingObj))
            {
                // Remove the flight from the station
                // send _stationsState.StateUpdated();
                // Update DB?
                return true;
            }

            return false;
        }

        private bool CanMoveToNextStation(IDataObj dataObj)
        {
            var nextStation = dataObj.StationsPath.Path[1];
            return _stationsState.IsStationEmpty(nextStation);
        }
        public bool MoveToNextStation(IDataObj dataObj)
        {
            var currStation = dataObj.StationsPath.CurrentStation;
            int targetIndex = dataObj.StationsPath.Path.Count - 1;
            var targetStation = dataObj.StationsPath.Path[targetIndex];
            try
            {
                if (CanMoveToNextStation(dataObj))
                {
                    var nextStation = dataObj.StationsPath.Path[1];
                    _stationsState.FindFastestPath(nextStation, targetStation);
                    _stationsState.MoveToStation(currStation, nextStation, dataObj.Flight); // Activated only when found a valid path from the next station.

                    // Call the StateUpdated() func.
                    // Update DB?
                    return true;
                }
            }
            catch (StationNotFoundException)
            {
                // Re-set the flight's stations path.
                dataObj.StationsPath = _stationsState.FindFastestPath(currStation, targetStation);
                MoveToNextStation(dataObj);
            }
            return false;
            // Other exceptions will be cought in the service?
        }
    }
}
