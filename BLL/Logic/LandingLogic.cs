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

        public bool StartLanding(LandingObj landingObj)
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
            landingObj.Flight.LandingTime = DateTime.Now + landingObj.StationsPath.OverallTime;
            return _stationsState.MoveToStation(null, pathEdges.StartStation, landingObj.Flight);
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
                return _stationsState.RemoveFlight(landingObj.StationsPath.CurrentStation);

            return false;
        }
        public async Task<bool> FinishLandingAsync(LandingObj landingObj)
        {
            return await Task.Run(() => FinishLanding(landingObj));
        }
    }
}
