using BLL.Data_Objects;
using BLL.Interfaces;
using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }

        public IReadOnlyList<IReadOnlyList<StationModel>> GetStationsState() => _stationsState.GetStationsState();

        public bool StartDeparture(string flightId)
        {
            if (CanAddFlight(FlightType.Departure))
            {
                // Task.Run(new DepartureObj...)
                //var departureObj = new DepartureObj(flightId);
                //Task.Factory.StartNew(() => departureObj.OnTimerElapsed)
                return true;
            }
            return false;

            // SignalR notification to client side & DB
        }

        public bool StartLanding(string flightId)
        {
            if (CanAddFlight(FlightType.Landing))
            {
                // Task.Run(new LandingObj...)
                _ = new LandingObj(flightId); // Should start the timer off and dont need to do anything else. (timers works on different threads?)
                //Task.Factory.StartNew(() => new LandingObj(flightId));
                return true;
            }
            return false;

            // SignalR notification to client side & DB
        }

        private bool CanAddFlight(FlightType type) => _stationsState.CanAddFlight(type);
    }
}
