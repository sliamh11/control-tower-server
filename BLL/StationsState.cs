using BLL.Interfaces;
using Common.Data_Structures;
using Common.Enums;
using Common.Models;
using Common.Structs;
using System;
using System.Collections.Generic;

namespace BLL
{
    // Holds the Data Structure (is singleton).
    public class StationsState : IStationsState
    {
        private readonly StationsGraph _stations;
        private readonly object _stationsLock = new object(); // For working with the data structure itself
        private readonly object _stateLock = new object(); // For state updates, etc.
        private readonly object _getLandingLock = new object();
        private readonly object _getDepartureLock = new object();

        public StationsState()
        {
            _stations = new StationsGraph();
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
        }

        public void StateUpdated()
        {
            // Called each time something updated the _stations field.
            // Emits the updated state to all subscribers (client + DB).

            // Maybe somehow update it in 'set' (property) if its even possible?
            throw new NotImplementedException();
        }

        public IReadOnlyList<IReadOnlyDictionary<string, StationModel>> GetStationsState()
        {
            lock (_stationsLock)
                return _stations.GetStationsState();
        }

        public bool AddStation(Dictionary<string, StationModel> newStations)
        {
            lock (_stationsLock)
                return _stations.AddStation(newStations);
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
                return _stations.MoveToStation(fromStation, toStation, flight);
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
                return _stations.RemoveFlight(station);
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
                return _stations.UpdateStation(updatedStation);
        }
    }
}
