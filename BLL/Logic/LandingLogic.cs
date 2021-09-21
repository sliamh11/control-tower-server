using BLL.Data_Objects;
using BLL.Interfaces;
using Common.Exceptions;
using System;

namespace BLL.Logic
{
    // Instance created per request (scoped).
    // MAKE THIS CLASS METHODS ASYNC
    public class LandingLogic : ILandingLogic
    {
        private IStationsState _stationsState;
        public LandingLogic()
        {
            _stationsState = StationsState.Instance;
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
            return landingObj.StationsPath.CurrentStation == landingObj.StationsPath.Path.Last.Value;
        }
        public bool FinishLanding(LandingObj landingObj)
        {
            if (CanFinishLanding(landingObj))
            {
                _stationsState.RemoveFlight(landingObj.StationsPath.CurrentStation);
                // send _stationsState.StateUpdated();
                // Update DB?
                return true;
            }

            return false;
        }

        private bool CanMoveToNextStation(IDataObj dataObj)
        {
            var nextStation = dataObj.StationsPath.Path.First.Next?.Value;
            if (nextStation == null)
                throw new StationNotFoundException();

            return _stationsState.IsStationEmpty(nextStation);
        }

        public bool MoveToNextStation(IDataObj dataObj)
        {
            var currStation = dataObj.StationsPath.CurrentStation;
            var targetStation = dataObj.StationsPath.Path.Last.Value;
            try
            {
                if (CanMoveToNextStation(dataObj))
                {
                    var nextStation = dataObj.StationsPath.Path.First.Next.Value;
                    _stationsState.FindFastestPath(nextStation, targetStation);
                    _stationsState.MoveToStation(currStation, nextStation, dataObj.Flight);
                    dataObj.StationsPath.Path.RemoveFirst(); // Remove old station
                    dataObj.StationsPath.CurrentStation = nextStation; // Update the current station.

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
