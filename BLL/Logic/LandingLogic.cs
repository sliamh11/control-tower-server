using BLL.Data_Objects;
using BLL.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Logic
{
    // Instance created per request (scoped).
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
            // Get next station's index from dataObj's stations path queue.
            // Check if station is free by checking the graph itself
            // return true / false.
            return true;
        }
        public bool MoveToNextStation(IDataObj dataObj)
        {
            if (CanMoveToNextStation(dataObj))
            {
                // Reset current station's flight
                // Move flight to the next in queue (pop / remove the first element).
                // Call the StateUpdated() func.
                // Update DB?
                return true;
            }
            return false;
        }

        public DateTime CalcEndTime(StationsPathModel path)
        {
            // Get shortest path from graph and calc time to arrive to final station.
            throw new NotImplementedException();
        }
    }
}
