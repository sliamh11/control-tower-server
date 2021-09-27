using Common.Enums;
using Common.Exceptions;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Common.Data_Structures
{
    public class StationsGraph
    {
        // Reasons for List over LinkedList
        // 1. I would use LinkedList if I needed to add and remove items frequently, which I don't.
        // 2. Easy to control a list's size (mostly O(1), when needed to expend - O(n)).
        // 3. Can access directly by index with O(1).

        private readonly List<List<StationModel>> _stations;
        private LinkedList<StationModel> _startLandingStations;
        private LinkedList<StationModel> _startDepartureStations;
        private LinkedList<StationModel> _endLandingStations;
        private LinkedList<StationModel> _endDepartureStations;

        public StationsGraph()
        {
            _stations = new List<List<StationModel>>(15); // Base size will be 15 (instead of 4 in meta-deta -> less array-overriding)
            _startLandingStations = new LinkedList<StationModel>();
            _startDepartureStations = new LinkedList<StationModel>();
            _endLandingStations = new LinkedList<StationModel>();
            _endDepartureStations = new LinkedList<StationModel>();
        }

        public IReadOnlyList<IReadOnlyList<StationModel>> GetStationsState() => _stations;

        // Add LandingStationsList & DepartureStationsList and save specific spots to make it easier and faster later on.
        public void AddStation(List<StationModel> station)
        {
            _stations.Add(station);
            // Add the relevant stations to the relevant lists
            foreach (var item in station)
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
        public void RemoveFlight(StationModel station)
        {
            _stations[station.Number].Find(x => x == station).CurrentFlight = null;
        }

        // O(n)
        public Tuple<StationModel, StationModel> GetLandingEdgeStations()
        {
            var startingPoint = GetFastestStation(_startLandingStations, true);
            var endingPoint = GetFastestStation(_endLandingStations);

            return startingPoint == null || endingPoint == null
                ? null
                : new Tuple<StationModel, StationModel>(startingPoint, endingPoint);
        }

        // O(n)
        public Tuple<StationModel, StationModel> GetDepartureEdgeStations()
        {
            var startingPoint = GetFastestStation(_startDepartureStations, true);
            var endingPoint = GetFastestStation(_endDepartureStations);

            return startingPoint == null || endingPoint == null
                ? null
                : new Tuple<StationModel, StationModel>(startingPoint, endingPoint);
        }

        // O(n)
        private StationModel GetFastestStation(IEnumerable<StationModel> stations, bool isEmptyStation = false)
        {
            if (stations == null || stations.FirstOrDefault() == null)
                return null;

            return stations.Where(x => isEmptyStation ? x.CurrentFlight == null : true)
                .Aggregate((minStandbyStation, station) =>
                station.StandbyPeriod < (minStandbyStation?.StandbyPeriod ?? station.StandbyPeriod) ? station : minStandbyStation);
        }

        // O(n)
        public bool UpdateStation(StationModel updatedStation)
        {
            var station = _stations[updatedStation.Number];
            int index = station.IndexOf(updatedStation); // Compares with .Equals() (Checks station's content)
            //int index = station.FindIndex(station => station.CompareTo(updatedStation) == 0);
            if (index >= 0)
            {
                station[index] = updatedStation; // Should work (is by reference), if not - change the _stations directly
                return true;
            }
            return false;
        }

        // O(n) 
        public bool CanAddFlight(FlightType type)
        {
            var list = type == FlightType.Departure ? _startDepartureStations : _startLandingStations;
            foreach (var item in list)
            {
                if (item.CurrentFlight == null)
                    return true;
            }
            return false;
        }

        // Dijakstra's algo
        public StationsPathModel FindFastestPath(int startIndex, int targetIndex)
        {
            if (startIndex < 0 || startIndex >= _stations.Count
                || targetIndex <= 0 || targetIndex >= _stations.Count)
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
               || targetIndex <= 0 || targetIndex >= _stations.Count)
                throw new ArgumentOutOfRangeException("Start index was out of the array's boundries.");

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

        // O(n)
        private void FillStationsTable(int index, StationsTable[] table)
        {
            while (table.Any(table => !table.IsVisited))
            {
                table[index].IsVisited = true;

                foreach (var item in _stations[index])
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

        // O(n)
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
                    foreach (var item in _stations[index])
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

        // O(n)
        public bool MoveToStation(StationModel fromStation, StationModel toStation, FlightModel flight)
        {
            if (toStation == null)
                throw new StationNotFoundException();

            var targetStation = _stations[toStation.Number].Find(x => x == toStation);

            // In case of Starting a landing / departure process.
            if (fromStation == null && toStation != null)
            {
                if (targetStation == null)
                    throw new StationNotFoundException();

                targetStation.CurrentFlight = flight;
                return true;
            }

            // At this point, fromStation must have a value.
            if (fromStation == null)
                throw new StationNotFoundException();

            var startStation = _stations[fromStation.Number].Find(x => x == fromStation);

            if (startStation == null || targetStation == null)
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
            // Note: why do I search for the last station here and not in dijakstra?
            // Answer: Because when I tried to convert dijakstra to work that way it began to be inefficient (it works by indexs and not objects, therefor i can only get the next station's index, not the specific object).
            if (table[targetIndex].PrevStation == null)
                return null;

            TimeSpan pathTime = new TimeSpan(0);
            Stack<StationModel> pathStack = new Stack<StationModel>();
            int tempIndex = targetIndex;

            while (table[tempIndex].PrevStation != null)
            {
                pathStack.Push(table[tempIndex].PrevStation);
                tempIndex = table[tempIndex].PrevStation.Number;
            }

            var stations = new LinkedList<StationModel>();

            while (pathStack.Count > 0)
            {
                var station = pathStack.Pop();
                pathTime += station.StandbyPeriod;
                stations.AddLast(station);
            }

            // Find the target station by minimum standby time - O(n)
            StationModel lastStation = GetFastestStation(_stations[targetIndex]);

            stations.AddLast(lastStation);
            pathTime += lastStation.StandbyPeriod;

            return new StationsPathModel(stations, pathTime);
        }

        // O(n)
        public bool IsStationEmpty(StationModel station)
        {
            var currStation = _stations[station.Number].Find(x => x == station);
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
