using BLL.Interfaces;
using Common.Data_Structures;
using Common.Enums;
using Common.Models;
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
        private static readonly object _initLock = new object();
        private readonly object _getLandingLock = new object();
        private readonly object _getDepartureLock = new object();

        public StationsState()
        {
            _stations = new StationsGraph();
            LoadStations();
        }

        private void LoadStations()
        {
            // Waiting stations
            AddStation(new List<StationModel>(new StationModel[] { new StationModel(0, 1, new TimeSpan(0, 1, 0), StationType.Landing) }));
            AddStation(new List<StationModel>(new StationModel[] { new StationModel(1, 2, new TimeSpan(0, 0, 45)) }));
            AddStation(new List<StationModel>(new StationModel[] { new StationModel(2, 3, new TimeSpan(0, 0, 40)) }));
            // Runway
            AddStation(new List<StationModel>(new StationModel[] { new StationModel(3, 4, new TimeSpan(0, 1, 30), StationType.Runway) }));
            // Airport & Depratures
            AddStation(new List<StationModel>(new StationModel[] {
                new StationModel(4, 5, new TimeSpan(0, 0, 45)),
                new StationModel(4, 6, new TimeSpan(0, 0, 45)) }));
            AddStation(new List<StationModel>(new StationModel[] { new StationModel(5, 7, new TimeSpan(0, 1, 0),
                StationType.LandingExit, StationType.Departure) }));// Also has an exit
            AddStation(new List<StationModel>(new StationModel[] { new StationModel(6, 7, new TimeSpan(0, 1, 0),
                StationType.LandingExit, StationType.Departure) }));// Also has an exit
            AddStation(new List<StationModel>(new StationModel[] { new StationModel(7, 3, new TimeSpan(0, 0, 35)) })); // Station before runway.
        }

        public void StateUpdated()
        {
            // Called each time something updated the _stations field.
            // Emits the updated state to all subscribers (client + DB).

            // Maybe somehow update it in 'set' (property) if its even possible?
            throw new NotImplementedException();
        }

        public IReadOnlyList<IReadOnlyList<StationModel>> GetStationsState()
        {
            lock (_stationsLock)
            {
                return _stations.GetStationsState();
            }
        }

        public void AddStation(List<StationModel> newStation)
        {
            lock (_stationsLock)
            {
                _stations.AddStation(newStation);
            }
        }

        public bool IsStationEmpty(StationModel station)
        {
            lock (_stationsLock)
            {
                return _stations.IsStationEmpty(station);
            }
        }

        public StationsPathModel FindFastestPath(int startIndex, int targetIndex)
        {
            lock (_stationsLock)
            {
                return _stations.FindFastestPath(startIndex, targetIndex);
            }
        }

        public StationsPathModel FindFastestPath(StationModel currStation, StationModel targetStation)
        {
            lock (_stationsLock)
            {
                return _stations.FindFastestPath(currStation, targetStation);
            }
        }

        public bool MoveToStation(StationModel fromStation, StationModel toStation, FlightModel flight)
        {
            lock (_stationsLock)
            {
                return _stations.MoveToStation(fromStation, toStation, flight);
            }
        }

        public Tuple<StationModel, StationModel> GetPathEdgeStations(FlightModel flight)
        {
            // If returns null - no available station for now -> put in queue or something.
            if (flight.Type == FlightType.Landing)
            {
                lock (_getLandingLock)
                    return _stations.GetLandingEdgeStations();
            }

            if (flight.Type == FlightType.Departure)
            {
                lock (_getDepartureLock)
                    return _stations.GetDepartureEdgeStations();
            }

            return null;
        }

        public void RemoveFlight(StationModel station)
        {
            lock (_stationsLock)
            {
                _stations.RemoveFlight(station);
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

        //public bool UpdateStation(StationModel updatedStation)
        //{
        //    lock (_stationsLock)
        //    {

        //    }
        //}
    }
}
