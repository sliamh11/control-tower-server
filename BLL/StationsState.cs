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
    // Holds the Data Structure (is singleton).
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
        }

        // For DI
        public StationsState(IHubContext<TowerHub, ITowerHub> hubContext) : this()
        {
            _hubContext = hubContext;
            LoadStations();
        }

        private void LoadStations()
        {
            // Normal stations
            var station = new StationModel(1, new TimeSpan(0, 1, 0), StationType.Landing);
            AddStation(new Dictionary<string, StationModel>() { { station.StationId, station } });
            station = new StationModel(2, new TimeSpan(0, 0, 45));
            AddStation(new Dictionary<string, StationModel>() { { station.StationId, station } });
            station = new StationModel(3, new TimeSpan(0, 0, 40));
            AddStation(new Dictionary<string, StationModel>() { { station.StationId, station } });

            // Runway
            station = new StationModel(4, new TimeSpan(0, 1, 30), StationType.Runway);
            AddStation(new Dictionary<string, StationModel>() { { station.StationId, station } });

            // Airport & Depratures
            station = new StationModel(5, new TimeSpan(0, 0, 45));
            var stationB = new StationModel(6, new TimeSpan(0, 0, 45));
            AddStation(new Dictionary<string, StationModel>() {
                { station.StationId, station },
                {stationB.StationId, stationB }
            });
            station = new StationModel(7, new TimeSpan(0, 1, 0), StationType.LandingExit, StationType.Departure);
            AddStation(new Dictionary<string, StationModel>() { { station.StationId, station } }); // Also has an exit
            station = new StationModel(7, new TimeSpan(0, 1, 0), StationType.LandingExit, StationType.Departure);
            AddStation(new Dictionary<string, StationModel>() { { station.StationId, station } }); // Also has an exit
            station = new StationModel(3, new TimeSpan(0, 0, 35));
            AddStation(new Dictionary<string, StationModel>() { { station.StationId, station } }); // Station before runway.
            StateUpdated();
        }

        // If not working and needs to be Task instead of void - use SemaphoreSlim object (lock but for async tasks)
        public async void StateUpdated()
        {
            // Emits the updated state to all subscribers (client + DB).
            // Update DB too
            await _hubContext?.Clients.All.StateUpdated(GetStationsState());
        }

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
    }
}
