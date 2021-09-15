using BLL.Data_Objects;
using BLL.Interfaces;
using Common.Models;
using System;

namespace BLL.Logic
{
    internal class DepartureLogic : IDepartureLogic
    {
        private IStationsState _stationsState;
        public DepartureLogic(IStationsState stationsState)
        {
            _stationsState = stationsState;
        }

        public StationsPathModel StartDeparture(DepartureObj departureObj)
        {
            // Maybe create a function inside the Graph for that?
            // Set station [0].Flight = flight
            return null; // return the station
        }

        private bool CanMoveToNextStation(IDataObj dataObj)
        {
            throw new NotImplementedException();
        }
        public bool MoveToNextStation(IDataObj dataObj)
        {
            throw new NotImplementedException();
        }

        private bool CanFinishDeparture()
        {
            throw new NotImplementedException();
        }
        public bool FinishDaperture(DepartureObj departureObj)
        {
            throw new NotImplementedException();
        }
    }
}
