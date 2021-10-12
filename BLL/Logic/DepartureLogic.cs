using BLL.Data_Objects;
using BLL.Interfaces;
using System;
using System.Threading.Tasks;

namespace BLL.Logic
{
    public class DepartureLogic : IDepartureLogic
    {
        private IStationsState _stationsState;
        public DepartureLogic(IStationsState state)
        {
            _stationsState = state;
        }

        #region Public Functions
        public bool StartDeparture(DepartureObj departureObj)
        {
            try
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
                SetFlightTimeSpans(departureObj);
                return _stationsState.MoveToStation(null, pathEdges.StartStation, departureObj.Flight);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> StartDepartureAsync(DepartureObj departureObj)
        {
            return await Task.Run(() => StartDeparture(departureObj));
        }
        public bool FinishDaperture(DepartureObj departureObj)
        {
            try
            {
                if (CanFinishDeparture(departureObj))
                    return _stationsState.RemoveFlight(departureObj.StationsPath.CurrentStation);

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> FinishDapertureAsync(DepartureObj departureObj)
        {
            return await Task.Run(() => FinishDaperture(departureObj));
        }
        #endregion

        #region Helper Functions
        private void SetFlightTimeSpans(DepartureObj departureObj)
        {
            var rand = new Random();
            var hours = rand.Next(0, 6);
            var minutes = rand.Next(1, 61);
            var seconds = rand.Next(1, 61);
            departureObj.Flight.DepartureTime = DateTime.Now + departureObj.StationsPath.OverallTime;
            departureObj.Flight.LandingTime = departureObj.Flight.DepartureTime + new TimeSpan(hours, minutes, seconds);
        }
        private bool CanFinishDeparture(DepartureObj departureObj)
        {
            return departureObj.StationsPath.CurrentStation == departureObj.StationsPath.Path.Last.Value;
        }
        #endregion
    }
}
