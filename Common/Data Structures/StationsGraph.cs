using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Common.Structs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Data_Structures
{
    public class StationsGraph
    {
        // Reasons for List of Dictionaries
        // 1. Direct Access to list (via station number), then inner search by StationId - O(1)
        // 2. Easy to control a list's size (mostly O(1), when needed to expend - O(n)).

        #region Private Fields
        private readonly List<Dictionary<string, StationModel>> _stations;
        private readonly LinkedList<StationModel> _startLandingStations;
        private readonly LinkedList<StationModel> _startDepartureStations;
        private readonly LinkedList<StationModel> _endLandingStations;
        private readonly LinkedList<StationModel> _endDepartureStations;
        #endregion

        public StationsGraph()
        {
            _stations = new List<Dictionary<string, StationModel>>(15);
            _startLandingStations = new LinkedList<StationModel>();
            _startDepartureStations = new LinkedList<StationModel>();
            _endLandingStations = new LinkedList<StationModel>();
            _endDepartureStations = new LinkedList<StationModel>();
        }

        #region Public Functions
        public IReadOnlyList<IReadOnlyDictionary<string, StationModel>> GetStationsState() => _stations;
        public PathEdgesStruct GetLandingEdgeStations()
        {
            var startingPoint = GetFastestStation(_startLandingStations, true);
            var endingPoint = GetFastestStation(_endLandingStations);

            return startingPoint == null || endingPoint == null
                ? default
                : new PathEdgesStruct(startingPoint, endingPoint);
        }
        public PathEdgesStruct GetDepartureEdgeStations()
        {
            var startingPoint = GetFastestStation(_startDepartureStations, true);
            var endingPoint = GetFastestStation(_endDepartureStations);

            return startingPoint == null || endingPoint == null
                ? default
                : new PathEdgesStruct(startingPoint, endingPoint);
        }
        public StationModel GetExitStation(FlightType flightType)
        {
            var list = flightType == FlightType.Departure ? _endDepartureStations : _endLandingStations;
            return GetFastestStation(list, true);
        }
        public bool CanAddFlight(FlightType type)
        {
            return type == FlightType.Departure
                ? CanAddDepartingFlight()
                : CanAddLandingFlight();
        }

        // O(1)
        public bool RemoveFlight(StationModel station)
        {
            if (_stations[station.Number].TryGetValue(station.Id, out station))
            {
                station.CurrentFlight = null;
                return true;
            }
            throw new StationNotFoundException();
        }

        // O(1)
        public bool UpdateStation(StationModel updatedStation)
        {
            if (_stations[updatedStation.Number].TryGetValue(updatedStation.Id, out StationModel existingStation))
            {
                _stations[existingStation.Number][existingStation.Id] = updatedStation;
                return true;
            }
            return false;
        }

        // O(1)
        public bool IsStationEmpty(StationModel station)
        {
            if (station.Number < 0 || station.Number >= _stations.Count)
                throw new ArgumentException();

            // TODO: If the planned station is not empty, check if other stations in the same station's number are empty.
            var currStation = _stations[station.Number].GetValueOrDefault(station.Id);
            if (currStation == null)
                throw new StationNotFoundException();

            return currStation.CurrentFlight == null;
        }

        // O(1)
        public bool MoveToStation(StationModel fromStation, StationModel toStation, FlightModel flight)
        {
            if (toStation == null
                || flight == null
                || ((toStation.Number < 0 || toStation.Number >= _stations.Count))
                || (fromStation != null && (fromStation.Number < 0 || fromStation.Number >= _stations.Count)))
                throw new ArgumentException();

            var targetStation = _stations[toStation.Number].GetValueOrDefault(toStation.Id);
            if (targetStation == null)
                throw new StationNotFoundException();

            // In case of Starting a landing / departure process.
            if (fromStation == null)
                return MoveStation(null, toStation, flight);

            var startStation = _stations[fromStation.Number].GetValueOrDefault(fromStation.Id);
            if (startStation == null)
                throw new StationNotFoundException();

            return MoveStation(fromStation, toStation, flight);
        }

        // O(n*m) - Dijakstra's algo
        public StationsPathModel FindFastestPath(int startIndex, int targetIndex)
        {
            if (startIndex < 0 || startIndex >= _stations.Count
                || targetIndex < 0 || targetIndex >= _stations.Count)
                throw new ArgumentOutOfRangeException("Start index was out of the array's boundries.");

            StationsTable[] table = new StationsTable[_stations.Count];
            InitStationsTable(table, startIndex);
            FillStationsTable(startIndex, table);
            return GetFastestPath(targetIndex, table);
        }

        // O(n*m) - Dijakstra's algo (for specific start & end stations)
        public StationsPathModel FindFastestPath(StationModel startStation, StationModel targetStation)
        {
            int startIndex = startStation.Number;
            int targetIndex = targetStation.Number;

            if (startIndex < 0 || startIndex >= _stations.Count
               || targetIndex < 0 || targetIndex >= _stations.Count)
                throw new ArgumentException("One of the stations isn't valid.");

            if (!_stations[startIndex].TryGetValue(startStation.Id, out StationModel stationA)
                || !_stations[targetIndex].TryGetValue(targetStation.Id, out StationModel stationB))
                throw new StationNotFoundException();

            StationsTable[] table = new StationsTable[_stations.Count];
            InitStationsTable(table, startIndex);
            FillStationsTable(startIndex, table, startStation, targetStation);
            return GetFastestPath(targetIndex, table);
        }

        // O(n*m)
        public bool AddStation(Dictionary<string, StationModel> stations)
        {
            if (stations == null || stations.Count == 0)
                throw new ArgumentException("Argument is invalid.");

            // NextStation must be >= 0 & Specified number must be the same for all stations.
            var stationNum = stations.First().Value.Number;
            if (stations.Values.Any(x => x.NextStation < 0 || x.Number != stationNum))
                return false;

            if (stationNum <= -1) // If default value (-1) - new station
            {
                foreach (var item in stations.Values)
                    item.Number = _stations.Count;

                _stations.Add(stations);
            }
            else // Existing station
            {
                foreach (var item in stations.Values)
                    _stations[stationNum].TryAdd(item.Id, item);
            }

            AddStationToHelperList(stations);
            return true;
        }
        #endregion

        #region Private Functions
        private void AddStationToHelperList(Dictionary<string, StationModel> stations)
        {
            foreach (var item in stations.Values)
            {
                foreach (var type in item.Types)
                {
                    switch (type)
                    {
                        case StationType.LandingExit:
                            _endLandingStations.AddLast(item);
                            break;
                        case StationType.Landing:
                            _startLandingStations.AddLast(item);
                            break;
                        case StationType.Departure:
                            _startDepartureStations.AddLast(item);
                            break;
                        case StationType.Runway:
                            _endDepartureStations.AddLast(item);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        // O(n)
        private void InitStationsTable(StationsTable[] table, int startIndex)
        {
            for (int i = 0; i < _stations.Count; i++)
            {
                table[i] = new StationsTable
                {
                    StationIndex = i,
                    PrevStation = null,
                    IsVisited = false,
                    Weight = new TimeSpan(1, 0, 0, 0)// 1 Day as max value.
                };
            }
            table[startIndex].Weight = new TimeSpan(0);
        }
        // O(n)
        private StationModel GetFastestStation(IEnumerable<StationModel> stations, bool isEmptyStation = false)
        {
            if (stations == null || stations.FirstOrDefault() == null)
                return null;

            var list = stations.Where(x => isEmptyStation ? x.CurrentFlight == null : true);
            return list.Any()
                ? list.Aggregate((minStandbyStation, station) =>
                station.StandbyPeriod < (minStandbyStation?.StandbyPeriod ?? station.StandbyPeriod) ? station : minStandbyStation)
                : null;
        }

        // O(n) 
        private StationsPathModel GetFastestPath(int targetIndex, StationsTable[] table)
        {
            // If path not found
            if (table[targetIndex].PrevStation == null)
                return null;

            var pathStack = GetDijakstraStack(table, targetIndex);
            var stations = new LinkedList<StationModel>();
            TimeSpan pathTime = new TimeSpan(0);

            while (pathStack.Count > 0)
            {
                var station = pathStack.Pop();
                pathTime += station.StandbyPeriod;
                stations.AddLast(station);
            }

            var lastStation = GetFastestStation(_stations[targetIndex].Values);
            stations.AddLast(lastStation);
            pathTime += lastStation.StandbyPeriod;
            return new StationsPathModel(stations, pathTime);
        }

        // O(n*m)
        private void FillStationsTable(int index, StationsTable[] table)
        {
            while (table.Any(table => !table.IsVisited))
            {
                table[index].IsVisited = true;
                FillIfTableValid(table, index);
                index = table.Where(t => !t.IsVisited).OrderBy(t => t.Weight).Select(t => t.StationIndex).FirstOrDefault();
            }
        }

        // O(n*m) - overload for specific stations scenario
        private void FillStationsTable(int index, StationsTable[] table, StationModel startStation, StationModel targetStation)
        {
            while (table.Any(table => !table.IsVisited))
            {
                table[index].IsVisited = true;

                if (index == startStation.Number)
                    FillSpecificStation(table, startStation);
                else if (index == targetStation.Number)
                    FillSpecificStation(table, targetStation);
                else
                    FillIfTableValid(table, index);

                index = table.Where(t => !t.IsVisited).OrderBy(t => t.Weight).Select(t => t.StationIndex).FirstOrDefault();
            }
        }

        #endregion

        #region Helper Fucntions
        private bool MoveStation(StationModel fromStation, StationModel toStation, FlightModel flight)
        {
            if (toStation.CurrentFlight == null)
            {
                if (fromStation != null)
                    fromStation.CurrentFlight = null;
                toStation.CurrentFlight = flight;
                return true;
            }
            return false;
        }
        private bool CanAddDepartingFlight()
        {
            // If station before the landing flight's 'exit stations' has a flight in it - dont add a flight.
            var lastLandingStationsNumber = _endLandingStations.First.Value.Number - 1;
            if (_stations[lastLandingStationsNumber].Values.Any(station => station.CurrentFlight != null))
                return false;
            // If it does - only if there is more then 1 station empty, add a departure flight.
            return _startDepartureStations.Count(station => station.CurrentFlight == null) > 1;
        }
        private bool CanAddLandingFlight()
        {
            return _startLandingStations.Any(x => x.CurrentFlight == null);
        }
        private Stack<StationModel> GetDijakstraStack(StationsTable[] table, int targetIndex)
        {
            var stack = new Stack<StationModel>();
            int tempIndex = targetIndex;

            while (table[tempIndex].PrevStation != null)
            {
                stack.Push(table[tempIndex].PrevStation);
                tempIndex = table[tempIndex].PrevStation.Number;
            }
            return stack;
        }
        private void FillSpecificStation(StationsTable[] table, StationModel station)
        {
            // Helper method for FillStationsTable overload function.
            TimeSpan weight = table[station.Number].Weight + station.StandbyPeriod;
            var destTable = table[station.NextStation];
            destTable.Weight = weight;
            destTable.PrevStation = station;
        }

        private void FillIfTableValid(StationsTable[] table, int index)
        {
            foreach (var item in _stations[index].Values)
            {
                TimeSpan newWeight = table[index].Weight + item.StandbyPeriod;
                var destTable = table[item.NextStation];
                // If current total time (weight) + the station's StandbyTime < the weight of the next station in the table
                if (newWeight < destTable.Weight)
                {
                    destTable.Weight = newWeight;
                    destTable.PrevStation = item; // Save the current station to get it's reference.
                }
            }
        }
        #endregion

        private class StationsTable
        {
            public int StationIndex { get; set; }
            public bool IsVisited { get; set; }
            public TimeSpan Weight { get; set; }
            public StationModel PrevStation { get; set; }
        }
    }
}
