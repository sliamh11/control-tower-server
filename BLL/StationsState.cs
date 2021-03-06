using BLL.Interfaces;
using Common.Data_Structures;
using Common.Enums;
using Common.Models;
using Common.Structs;
using ControlTowerHub;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;

namespace BLL
{
    /// <summary>
    /// This class is in charge of managing the program's current state.
    /// It holds the graph structure and updates the DB & Client (with the hub).
    /// </summary>
    public class StationsState : IStationsState
    {
        #region Private Fields
        private readonly StationsGraph _stations;
        private readonly IHubContext<TowerHub, ITowerHub> _hubContext;
        private readonly object _stationsLock = new object();
        private readonly object _getLandingLock = new object();
        private readonly object _getDepartureLock = new object();
        #endregion

        // For Unit Testing
        public StationsState()
        {
            _stations = new StationsGraph();
            LoadStations();
        }

        // For DI
        public StationsState(IHubContext<TowerHub, ITowerHub> hubContext) : this()
        {
            _hubContext = hubContext;
        }

        #region State Functions
        public async void StateUpdated()
        {
            // Emits the updated state to all subscribers (client).
            await _hubContext?.Clients.All.StateUpdated(GetStationsState());
        }
        #endregion

        #region Public Functions
        public IReadOnlyList<IReadOnlyDictionary<string, StationModel>> GetStationsState()
        {
            return _stations.GetStationsState();
        }
        public bool AddStation(Dictionary<string, StationModel> newStations)
        {
            lock (_stationsLock)
            {
                if (_stations.AddStation(newStations))
                {
                    if (_hubContext != null)
                        StateUpdated();
                    return true;
                }
                return false;
            }
        }
        public bool IsStationEmpty(StationModel station)
        {
            lock (_stationsLock)
                return _stations.IsStationEmpty(station);
        }
        public StationsPathModel FindFastestPath(int startIndex, int targetIndex)
        {
            lock (_stationsLock)
                return _stations.FindFastestPath(startIndex, targetIndex);
        }
        public StationsPathModel FindFastestPath(StationModel currStation, StationModel targetStation)
        {
            lock (_stationsLock)
                return _stations.FindFastestPath(currStation, targetStation);
        }
        public bool MoveToStation(StationModel fromStation, StationModel toStation, FlightModel flight)
        {
            lock (_stationsLock)
            {
                if (_stations.MoveToStation(fromStation, toStation, flight))
                {
                    StateUpdated();
                    return true;
                }
                return false;
            }
        }
        public PathEdgesStruct GetPathEdgeStations(FlightModel flight)
        {
            if (flight.Type == FlightType.Landing)
            {
                lock (_getLandingLock)
                    return _stations.GetLandingEdgeStations();
            }

            lock (_getDepartureLock)
                return _stations.GetDepartureEdgeStations();
        }
        public bool RemoveFlight(StationModel station)
        {
            lock (_stationsLock)
            {
                if (_stations.RemoveFlight(station))
                {
                    StateUpdated();
                    return true;
                }
                return false;
            }
        }
        public bool CanAddFlight(FlightType type)
        {
            if (type == FlightType.Departure)
            {
                lock (_getDepartureLock)
                    return _stations.CanAddFlight(type);
            }
            else
            {
                lock (_getLandingLock)
                    return _stations.CanAddFlight(type);
            }
        }
        public bool UpdateStation(StationModel updatedStation)
        {
            lock (_stationsLock)
            {
                if (_stations.UpdateStation(updatedStation))
                {
                    StateUpdated();
                    return true;
                }
                return false;
            }
        }
        public StationModel GetExitStation(FlightType type)
        {
            if (type == FlightType.Departure)
            {
                lock (_getDepartureLock)
                    return _stations.GetExitStation(type);
            }

            lock (_getLandingLock)
                return _stations.GetExitStation(type);
        }
        #endregion

        #region Private Functions
        private void LoadStations()
        {
            // Normal stations
            var station = new StationModel(1, new TimeSpan(0, 0, 20), StationType.Landing);
            AddStation(new Dictionary<string, StationModel>() { { station.Id, station } });
            station = new StationModel(2, new TimeSpan(0, 0, 15));
            AddStation(new Dictionary<string, StationModel>() { { station.Id, station } });
            station = new StationModel(3, new TimeSpan(0, 0, 13));
            AddStation(new Dictionary<string, StationModel>() { { station.Id, station } });

            // Runway
            station = new StationModel(4, new TimeSpan(0, 0, 12), StationType.Runway);
            AddStation(new Dictionary<string, StationModel>() { { station.Id, station } });

            // Airport & Depratures
            station = new StationModel(5, new TimeSpan(0, 0, 15));
            var stationB = new StationModel(6, new TimeSpan(0, 0, 15));
            AddStation(new Dictionary<string, StationModel>() {
                { station.Id, station },
                {stationB.Id, stationB }
            });
            station = new StationModel(7, new TimeSpan(0, 0, 15), StationType.LandingExit, StationType.Departure);
            AddStation(new Dictionary<string, StationModel>() { { station.Id, station } }); // Also has an exit
            station = new StationModel(7, new TimeSpan(0, 0, 15), StationType.LandingExit, StationType.Departure);
            AddStation(new Dictionary<string, StationModel>() { { station.Id, station } }); // Also has an exit
            station = new StationModel(3, new TimeSpan(0, 0, 17));
            AddStation(new Dictionary<string, StationModel>() { { station.Id, station } }); // Station before runway.
            if (_hubContext != null)
                StateUpdated();
        }
        #endregion
    }
}
