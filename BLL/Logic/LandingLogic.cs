using BLL.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public bool CanFinishLanding()
        {
            throw new NotImplementedException();
        }

        public bool CanMoveToNextStation(FlightModel flight)
        {
            throw new NotImplementedException();
        }

        public void FinishLanding()
        {
            throw new NotImplementedException();
        }

        public bool MoveToNextStation(FlightModel flight)
        {
            throw new NotImplementedException();
        }

        public StationModel StartLanding(FlightModel flight)
        {
            // Set station [0].Flight = flight
            return null; // return the station
        }
    }
}
