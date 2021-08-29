using BLL.Interfaces;
using Common.Models;
using System;

namespace BLL.Logic
{
    public class DepartureLogic : IDepartureLogic
    {
        private IStationsState _stationsState;
        public DepartureLogic(IStationsState stationsState)
        {
            _stationsState = stationsState;
        }

        public bool CanFinishDeparture()
        {
            throw new NotImplementedException();
        }

        public bool CanMoveToNextStation(FlightModel flight)
        {
            throw new NotImplementedException();
        }

        public void FinishDaperture()
        {
            throw new NotImplementedException();
        }

        public bool MoveToNextStation(FlightModel flight)
        {
            throw new NotImplementedException();
        }

        public StationModel StartDeparture(FlightModel flight)
        {
            // Maybe create a function inside the Graph for that?
            // Set station [0].Flight = flight
            return null; // return the station
        }
    }
}
