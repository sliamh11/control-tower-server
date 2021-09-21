using BLL.Data_Objects;
using BLL.Interfaces;
using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class StationsManager : IStationsManager
    {
        private IStationsState _stationsState;
        public StationsManager()
        {
            _stationsState = StationsState.Instance;
        }

        public void AddStation(List<StationModel> newStation)
        {
            // Side Note: Does the format validation happens here or in the service? 
            _stationsState.AddStation(newStation);
        }

        public IReadOnlyList<IReadOnlyList<StationModel>> GetStationsState() => _stationsState.GetStationsState();

        public bool StartDeparture(string flightId)
        {
            if (CanAddFlight(FlightType.Departure))
            {
                _ = new DepartureObj(flightId); // Works on another thread with a timer
                return true;
            }
            return false;

            // SignalR notification to client side & DB
        }

        public bool StartLanding(string flightId)
        {
            if (CanAddFlight(FlightType.Landing))
            {
                _ = new LandingObj(flightId); // Works on another thread with a timer
                return true;
            }
            return false;

            // SignalR notification to client side & DB
        }

        private bool CanAddFlight(FlightType type) => _stationsState.CanAddFlight(type);
    }
}
