using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Common.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Common.Data_Structures
{
    public class StationsGraph
    {
        // Reasons for List of Dictionaries
        // 1. Direct Access to list (via station number), then inner search by StationId - O(1)
        // 2. Easy to control a list's size (mostly O(1), when needed to expend - O(n)).

        private readonly List<Dictionary<string, StationModel>> _stations;
        private readonly LinkedList<StationModel> _startLandingStations;
        private readonly LinkedList<StationModel> _startDepartureStations;
        private readonly LinkedList<StationModel> _endLandingStations;
        private readonly LinkedList<StationModel> _endDepartureStations;

        public StationsGraph()
        {
            _stations = new List<Dictionary<string, StationModel>>(15); // Base size will be 15 (instead of 4 in meta-deta -> less array-overriding)
            _startLandingStations = new LinkedList<StationModel>();
            _startDepartureStations = new LinkedList<StationModel>();
            _endLandingStations = new LinkedList<StationModel>();
            _endDepartureStations = new LinkedList<StationModel>();
        }

        public IReadOnlyList<IReadOnlyDictionary<string, StationModel>> GetStationsState() => _stations;

        // O(n*m)
        public bool AddStation(Dictionary<string, StationModel> station)
        {
            if (station == null || station.Count == 0)
                throw new ArgumentException("Argument is invalid.");

            var stationNum = station.First().Value.Number;

            // NextStation must be >= 0 & Specified number must be the same for all stations.
            if (station.Values.Any(x => x.NextStation < 0 || x.Number != stationNum))
                return false;

            if (stationNum <= -1) // If default value (-1) - new station
            {
                // Add a new station and set it's number.
                foreach (var item in station.Values)
                    item.Number = _stations.Count;

                _stations.Add(station);
            }
            else
            {
                // Add to existing station
                foreach (var item in station.Values)
                    _stations[stationNum].TryAdd(item.StationId, item); // Will not add a duplicated station
            }

            // Add the relevant stations to the relevant lists
            foreach (var item in station.Values)
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
            return true;
        }

        // O(1)
        public bool RemoveFlight(StationModel station)
        {
            if (_stations[station.Number].TryGetValue(station.StationId, out station))
            {
                station.CurrentFlight = null;
                return true;
            }
            throw new StationNotFoundException();
        }

        // O(n)
        public PathEdgesStruct GetLandingEdgeStations()
        {
            var startingPoint = GetFastestStation(_startLandingStations, true);
            var endingPoint = GetFastestStation(_endLandingStations);

            return startingPoint == null || endingPoint == null
                ? default
                : new PathEdgesStruct(startingPoint, endingPoint);
        }

        // O(n)
        public PathEdgesStruct GetDepartureEdgeStations()
        {
            var startingPoint = GetFastestStation(_startDepartureStations, true);
            var endingPoint = GetFastestStation(_endDepartureStations);

            return startingPoint == null || endingPoint == null
                ? default
                : new PathEdgesStruct(startingPoint, endingPoint);
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

        // O(1)
        public bool UpdateStation(StationModel updatedStation)
        {
            if (_stations[updatedStation.Number].TryGetValue(updatedStation.StationId, out StationModel existingStation))
            {
                _stations[existingStation.Number][existingStation.StationId] = updatedStation;
                return true;
            }
            return false;
        }

        // O(n) 
        public bool CanAddFlight(FlightType type)
        {
            var list = type == FlightType.Departure ? _startDepartureStations : _startLandingStations;
            return list.Any(x => x.CurrentFlight == null);
        }

        // Dijakstra's algo
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

        // Dijakstra's algo (for specific start & end stations)
        public StationsPathModel FindFastestPath(StationModel startStation, StationModel targetStation)
        {
            int startIndex = startStation.Number;
            int targetIndex = targetStation.Number;

            if (startIndex < 0 || startIndex >= _stations.Count
               || targetIndex < 0 || targetIndex >= _stations.Count)
                throw new ArgumentException("One of the stations isn't valid.");

            if (!_stations[startIndex].TryGetValue(startStation.StationId, out StationModel stationA)
                || !_stations[targetIndex].TryGetValue(targetStation.StationId, out StationModel stationB))
                throw new StationNotFoundException();

            StationsTable[] table = new StationsTable[_stations.Count];
            InitStationsTable(table, startIndex);
            FillStationsTable(startIndex, table, startStation, targetStation);
            return GetFastestPath(targetIndex, table);
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

        // O(n*m)
        private void FillStationsTable(int index, StationsTable[] table)
        {
            while (table.Any(table => !table.IsVisited))
            {
                table[index].IsVisited = true;

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
                index = table.Where(t => !t.IsVisited).OrderBy(t => t.Weight).Select(t => t.StationIndex).FirstOrDefault();
            }
        }

        // O(n*m)
        private void FillStationsTable(int index, StationsTable[] table, StationModel startStation, StationModel targetStation)
        {
            while (table.Any(table => !table.IsVisited))
            {
                table[index].IsVisited = true;

                if (index == startStation.Number)
                {
                    TimeSpan weight = table[index].Weight + startStation.StandbyPeriod;
                    var destTable = table[startStation.NextStation];
                    destTable.Weight = weight;
                    destTable.PrevStation = startStation;
                }
                else if (index == targetStation.Number)
                {
                    TimeSpan weight = table[index].Weight + targetStation.StandbyPeriod;
                    var destTable = table[targetStation.NextStation];
                    destTable.Weight = weight;
                    destTable.PrevStation = targetStation;
                }
                else
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
                index = table.Where(t => !t.IsVisited).OrderBy(t => t.Weight).Select(t => t.StationIndex).FirstOrDefault();
            }
        }

        // O(1)
        public bool MoveToStation(StationModel fromStation, StationModel toStation, FlightModel flight)
        {
            if (toStation == null
                || flight == null
                || ((toStation.Number < 0 || toStation.Number >= _stations.Count))
                || (fromStation != null && (fromStation.Number < 0 || fromStation.Number >= _stations.Count)))
                throw new ArgumentException();

            var targetStation = _stations[toStation.Number].GetValueOrDefault(toStation.StationId);
            if (targetStation == null)
                throw new StationNotFoundException();

            // In case of Starting a landing / departure process.
            if (fromStation == null)
            {
                // If target station is empty
                if (targetStation.CurrentFlight == null)
                {
                    targetStation.CurrentFlight = flight;
                    return true;
                }
                return false;
            }

            var startStation = _stations[fromStation.Number].GetValueOrDefault(fromStation.StationId);
            if (startStation == null)
                throw new StationNotFoundException();

            // If next station is empty - move the flight to it.
            if (targetStation.CurrentFlight == null)
            {
                startStation.CurrentFlight = null;
                targetStation.CurrentFlight = flight;
                return true;
            }
            return false;
        }

        // O(n) 
        private StationsPathModel GetFastestPath(int targetIndex, StationsTable[] table)
        {
            // If path not found
            if (table[targetIndex].PrevStation == null)
                return null;

            TimeSpan pathTime = new TimeSpan(0);
            var pathStack = new Stack<StationModel>();
            int tempIndex = targetIndex;

            while (table[tempIndex].PrevStation != null)
            {
                pathStack.Push(table[tempIndex].PrevStation); // Something crashes here when testing
                tempIndex = table[tempIndex].PrevStation.Number;
            }

            var stations = new LinkedList<StationModel>();

            while (pathStack.Count > 0)
            {
                var station = pathStack.Pop();
                pathTime += station.StandbyPeriod;
                stations.AddLast(station);
            }

            // Note: why do I search for the last station here and not in dijakstra?
            // Answer: Because when I tried to convert dijakstra to work that way it began to be inefficient (its based on previous indexs, therefor I can only get the next station's index, not the specific object).
            // Find the target station by minimum standby time - O(n)
            StationModel lastStation = GetFastestStation(_stations[targetIndex].Values);

            stations.AddLast(lastStation);
            pathTime += lastStation.StandbyPeriod;

            return new StationsPathModel(stations, pathTime);
        }

        // O(1)
        public bool IsStationEmpty(StationModel station)
        {
            if (station.Number < 0 || station.Number >= _stations.Count)
                throw new ArgumentException();

            var currStation = _stations[station.Number].GetValueOrDefault(station.StationId);
            if (currStation == null)
                throw new StationNotFoundException();

            return currStation.CurrentFlight == null;
        }

        private class StationsTable
        {
            public int StationIndex { get; set; }
            public bool IsVisited { get; set; }
            public TimeSpan Weight { get; set; }
            public StationModel PrevStation { get; set; }
        }
    }
}
