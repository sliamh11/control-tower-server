using BLL.Data_Objects;
using BLL.Interfaces;
using System;
using System.Threading.Tasks;

namespace BLL.Logic
{
    // Instance created per request (scoped).
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
        public async Task<bool> StartLandingAsync(LandingObj landingObj)
        {
            return await Task.Run(() => StartLanding(landingObj));
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
        public async Task<bool> FinishLandingAsync(LandingObj landingObj)
        {
            return await Task.Run(() => FinishLanding(landingObj));
        }
    }
}
