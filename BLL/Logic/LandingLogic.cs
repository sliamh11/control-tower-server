using BLL.Data_Objects;
using BLL.Interfaces;
using System;
using System.Threading.Tasks;

namespace BLL.Logic
{
    public class LandingLogic : ILandingLogic
    {
        private IStationsState _stationsState;
        public LandingLogic(IStationsState stationsState)
        {
            _stationsState = stationsState;
        }

        #region Public Functions
        public bool StartLanding(LandingObj landingObj)
        {
            try
            {
                // 'Fastest' start & end points (by StandbyPeriod)
                var pathEdges = _stationsState.GetPathEdgeStations(landingObj.Flight);
                if (pathEdges.StartStation == null)
                    return false;

                // Fastest path between the points
                var path = _stationsState.FindFastestPath(pathEdges.StartStation, pathEdges.EndStation);
                if (path == null)
                    return false;

                landingObj.StationsPath = path;
                SetFlightTimeSpans(landingObj);
                return _stationsState.MoveToStation(null, pathEdges.StartStation, landingObj.Flight);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> StartLandingAsync(LandingObj landingObj)
        {
            return await Task.Run(() => StartLanding(landingObj));
        }
        public bool FinishLanding(LandingObj landingObj)
        {
            try
            {
                if (CanFinishLanding(landingObj))
                    return _stationsState.RemoveFlight(landingObj.StationsPath.CurrentStation);

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> FinishLandingAsync(LandingObj landingObj)
        {
            return await Task.Run(() => FinishLanding(landingObj));
        }
        #endregion

        #region Helper Functions
        private void SetFlightTimeSpans(LandingObj landingObj)
        {
            var rand = new Random();
            var hours = rand.Next(0, 6);
            var minutes = rand.Next(1, 61);
            var seconds = rand.Next(1, 61);
            landingObj.Flight.DepartureTime = DateTime.Now - new TimeSpan(hours, minutes, seconds);
            landingObj.Flight.LandingTime = DateTime.Now + landingObj.StationsPath.OverallTime;
        }
        private bool CanFinishLanding(LandingObj landingObj)
        {
            return landingObj.StationsPath.CurrentStation == landingObj.StationsPath.Path.Last.Value;
        }
        #endregion
    }
}
