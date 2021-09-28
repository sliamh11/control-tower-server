using BLL.Data_Objects;
using BLL.Interfaces;
using System;
using System.Threading.Tasks;

namespace BLL.Logic
{
    // Instance created per request (scoped).
    public class DepartureLogic : IDepartureLogic
    {
        private IStationsState _stationsState;
        public DepartureLogic(IStationsState state)
        {
            _stationsState = state;
            //_stationsState = StationsState.Instance;
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
            return _stationsState.MoveToStation(null, pathEdges.Item1, departureObj.Flight);
        }
        public async Task<bool> StartDepartureAsync(DepartureObj departureObj)
        {
            return await Task.Run(() => StartDeparture(departureObj));
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
        public async Task<bool> FinishDapertureAsync(DepartureObj departureObj)
        {
            return await Task.Run(() => FinishDaperture(departureObj));
        }

    }
}
