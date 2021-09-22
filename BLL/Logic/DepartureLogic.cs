using BLL.Data_Objects;
using BLL.Interfaces;
using Common.Exceptions;
using Common.Models;
using System;

namespace BLL.Logic
{
    public class DepartureLogic : IDepartureLogic
    {
        private IStationsState _stationsState;
        public DepartureLogic()
        {
            _stationsState = StationsState.Instance;
        }

        public bool StartDeparture(DepartureObj departureObj)
        {
            // 'Fastest' start & end points (by StandbyPeriod)
            var pathEdges = _stationsState.GetPathEdgeStations(departureObj.Flight);
            if (pathEdges == null)
                return false;

            // Fastest path between the points
            var path = _stationsState.FindFastestPath(pathEdges.Item1, pathEdges.Item2);
            if (path == null)
                return false;

            departureObj.StationsPath = path;
            departureObj.Flight.DepartureTime = DateTime.Now + departureObj.StationsPath.OverallTime;
            _stationsState.MoveToStation(null, pathEdges.Item1, departureObj.Flight);
            return true;
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

        private bool CanFinishDeparture(DepartureObj departureObj)
        {
            return departureObj.StationsPath.CurrentStation == departureObj.StationsPath.Path.Last.Value;
        }
        public bool FinishDaperture(DepartureObj departureObj)
        {
            if (CanFinishDeparture(departureObj))
            {
                _stationsState.RemoveFlight(departureObj.StationsPath.CurrentStation);
                // send _stationsState.StateUpdated();
                // Update DB?
                return true;
            }

            return false;
        }
    }
}
