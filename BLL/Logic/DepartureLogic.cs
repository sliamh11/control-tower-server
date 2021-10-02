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
        }

        public bool StartDeparture(DepartureObj departureObj)
        {
            // 'Fastest' start & end points (by StandbyPeriod)
            var pathEdges = _stationsState.GetPathEdgeStations(departureObj.Flight);
            if (pathEdges.StartStation == null || pathEdges.EndStation == null)
                return false;

            // Fastest path between the points
            var path = _stationsState.FindFastestPath(pathEdges.StartStation, pathEdges.EndStation);
            if (path == null)
                return false;

            departureObj.StationsPath = path;
            departureObj.Flight.DepartureTime = DateTime.Now + departureObj.StationsPath.OverallTime;
            return _stationsState.MoveToStation(null, pathEdges.StartStation, departureObj.Flight);
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
                return _stationsState.RemoveFlight(departureObj.StationsPath.CurrentStation);

            return false;
        }
        public async Task<bool> FinishDapertureAsync(DepartureObj departureObj)
        {
            return await Task.Run(() => FinishDaperture(departureObj));
        }

    }
}
