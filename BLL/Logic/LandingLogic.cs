using BLL.Data_Objects;
using BLL.Interfaces;
using Common.Exceptions;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            try
            {
                if (CanMoveToNextStation(dataObj))
                {
                    // Reset current station's flight (need to make a function - LeaveStation())
                    // Find fastest route from the next station to the target - with overrided FindFastestPath func (accepts objects)
                    // Call the StateUpdated() func.
                    // Update DB?
                    return true;
                }
                return false;
            }
            catch (StationNotFoundException ex)
            {
                var currStation = dataObj.StationsPath.CurrentStation;
                int targetIndex = dataObj.StationsPath.Path.Count - 1;
                var targetStation = dataObj.StationsPath.Path[targetIndex];
                // Make an override function for FindFastestPath which accepts StationModels as params instead of integers (to know the exact stations)
                dataObj.StationsPath = _stationsState.FindFastestPath(currStation, targetStation);
                MoveToNextStation(dataObj);
            }

            // Other exceptions will be cought in the service?
        }

        public DateTime CalcEndTime(StationsPathModel path)
        {
            // Get shortest path from graph and calc time to arrive to final station.
            throw new NotImplementedException();
        }
    }
}
