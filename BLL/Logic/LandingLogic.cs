using BLL.Data_Objects;
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
        public StationsPathModel StartLanding(LandingObj landingObj)
        {
            // Already after landing approval
            // Get the stations path from the graph
            // var startingStation = _stationsState.GetStartingStation
            //LandingObj = _stationsState.FindFastestPath()
            // Set finalized time in FlightModel with 'CalcEndTime(...)'.
            // Set FlightModel as the flight at the right graph index.
            throw new NotImplementedException();
        }

        private bool CanFinishLanding(LandingObj landingObj)
        {
            // If current object's station is the last one / queue is empty (What will be more generic and better?) - return true.

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
