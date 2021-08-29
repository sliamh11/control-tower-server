﻿using BLL.Interfaces;
using Common.Data_Structures;
using Common.Models;
using System;

namespace BLL
{
    public class StationsManager : IStationsManager
    {
        private IStationsState _stationsState;
        public StationsManager(IStationsState stationsState)
        {
            _stationsState = stationsState;
        }

        public StationsGraph GetStationsState() => _stationsState.GetStationsState();

        public void LoadStations() => _stationsState.LoadStations();

        public bool StartDeparture(string flightId)
        {
            if (CanStartDeparture())
            {
                // Task.Run(new DepartureObj...)
                return true;
            }
            return false;

            // SignalR notification to client side & DB
        }

        public bool StartLanding(string flightId)
        {
            if (CanStartLanding())
            {
                // Task.Run(new LandingObj...)
                return true;
            }
            return false;

            // SignalR notification to client side & DB
        }

        private bool CanStartDeparture()
        {
            return true;
        }

        private bool CanStartLanding()
        {
            return true;
        }
    }
}
