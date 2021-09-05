using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data_Structures
{
    public class StationsGraph
    {
        // Reasons for List over LinkedList
        // 1. I would use LinkedList if I needed to add and remove items frequently, which I don't.
        // 2. Easy to control a list's size (mostly O(1), when needed to expend - O(n)).
        // 3. Can access directly by index with O(1).

        private readonly List<List<StationModel>> _stations;

        public StationsGraph()
        {
            // Base size will be 15 (instead of 4 in meta-deta -> increases performance slightly).
            _stations = new List<List<StationModel>>(15);
        }

        // Add LandingStationsList & DepartureStationsList and save specific spots to make it easier and faster later on.
        public void AddStation(List<StationModel> station) => _stations.Add(station);

        public IReadOnlyList<IReadOnlyList<StationModel>> GetStationsState() => _stations;

        // O(n)
        public bool UpdateStation(StationModel updatedStation)
        {
            var station = _stations[updatedStation.Number];
            int index = station.FindIndex(station => station.CompareTo(updatedStation) == 0);
            if (index >= 0)
            {
                station[index] = updatedStation; // Should work (is by reference), if not - change the _stations directly
                return true;
            }
            return false;
        }

        public StationsPathModel FindFastestRoute(int startIndex, int targetIndex)
        {
            bool[][] isVisited = new bool[_stations.Count][];
            bool isRouteExists = false;
            for (int i = 0; i < isVisited.Length; i++)
            {
                isVisited[i] = new bool[_stations[i].Count];
            }
            isVisited[startIndex][0] = true;
            var stationsList = new List<List<StationModel>>();
            do
            {
                isRouteExists = FindRoutes(startIndex, startIndex, targetIndex, stationsList, null, isVisited);
            } while (!ResetVisitedStations(0, isVisited).Item1 && isRouteExists);
            if (stationsList.Count > 0)
                return FindFastestRoute(stationsList);
            return null;
        }

        // Might want to add another List<StationModel> param for current stations.
        private bool FindRoutes(int current,
            int startIndex,
            int targetIndex,
            List<List<StationModel>> stationsList,
            List<StationModel> station,
            bool[][] isVisited)
        {
            if (station == null)
                station = new List<StationModel>();

            if (_stations[current] == null)
                return false; // If reached end point (check it)

            if (current == targetIndex)
                return true;

            for (int i = 0; i < _stations[current].Count; i++)
            {
                var item = _stations[current][i];

                if (isVisited[item.NextStation][i])
                    continue; // If the item has already been checked, skip it.

                isVisited[item.NextStation][i] = true;
                bool isPathFound = FindRoutes(item.NextStation, startIndex, targetIndex, stationsList, station, isVisited);

                if (isPathFound)
                {
                    station.Add(item);
                    if (item.Number == startIndex)
                        stationsList.Add(station);

                    return true;
                }

                // When we reach here we actually on our way back after non of the above returned true.
                // Since I dont want the visited items to be = true (because I need to check them in more then one scenario), I'll get their value returned to false. Thats OK because there's no way I'll go the same way on the same root.
                isVisited[item.NextStation][i] = false;
            }

            return false;
        }

        private Tuple<bool, int> ResetVisitedStations(int current, bool[][] isVisited)
        {
            // Go from last to first
            // When a row contains a false column - go to the next row and start resetting it all from the start
            // return when all of the first row's columns are true.
            bool isRowNotDone = false;
            if (isVisited[current + 1] != null)
            {
                var result = ResetVisitedStations(current + 1, isVisited);
                isRowNotDone = result.Item1;
                if (isRowNotDone)
                {
                    for (int i = 0; i < isVisited[current].Length; i++)
                        isVisited[current][i] = false;

                    return new Tuple<bool, int>(true, result.Item2);
                }
            }

            // when reaching here, im in the end of the isVisited row (at first)

            // Check if row contains visited & unvisited slots
            if (current > 0)
            {
                var rowNotDone = isVisited[current].Any(item => item && !item);
                return new Tuple<bool, int>(rowNotDone, current);
            }
            else
            {
                return isVisited[current].All(visit => visit)
                    ? new Tuple<bool, int>(true, current)
                    : new Tuple<bool, int>(false, current); // Done
            }
        }

        // O(n*m)
        private StationsPathModel FindFastestRoute(List<List<StationModel>> stationsList)
        {
            if (stationsList.Count == 0)
                return null;

            var minTime = new TimeSpan(1, 0, 0, 0);
            var fastestRoute = new List<StationModel>();

            foreach (var station in stationsList)
            {
                var currentTime = new TimeSpan(0);
                foreach (var item in station)
                {
                    currentTime += item.StandbyPeriod;
                }
                if (currentTime < minTime)
                {
                    fastestRoute = station;
                    minTime = currentTime;
                }
            }

            return new StationsPathModel(fastestRoute, minTime);
        }

        // Dijakstra's algo
        public StationsPathModel FindFastestPath(int startIndex, int targetIndex)
        {
            // How will it work:
            // Instead of saving it all in a big array, save ONLY the minimal path (get it as a param each function call).
            // Side note - maybe make an inner class in the graph which represents the data - understand what is needed
            if (startIndex < 0 || startIndex >= _stations.Count
                || targetIndex <= startIndex || targetIndex >= _stations.Count)
                throw new ArgumentOutOfRangeException("Start index was out of the array's boundries.");

            StationsTable[] table = new StationsTable[_stations.Count];
            InitStationsTable(table, startIndex);
            FillStationsTable(startIndex, targetIndex, table);
            return GetFastestPath(targetIndex, table);
        }

        // O(n)
        private void InitStationsTable(StationsTable[] table, int source)
        {
            for (int i = 0; i < _stations.Count; i++)
            {
                table[i] = new StationsTable
                {
                    StationIndex = i,
                    PrevStation = null,
                    IsVisited = false,
                    Weight = new TimeSpan(1, 0, 0, 0) // 1 Day as max value.
                };
            }
            table[source].Weight = new TimeSpan(0);
        }

        // O(n Log n) (recursive + decreasing inner loops)
        private void FillStationsTable(int index, int targetIndex, StationsTable[] table)
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
        private StationsPathModel GetFastestPath(int targetIndex, StationsTable[] table)
        {
            Stack<StationModel> pathStack = new Stack<StationModel>();
            TimeSpan pathTime = new TimeSpan(0);

            // Not efficient, try to chanage it.
            TimeSpan lastStationTime = _stations[targetIndex].Min(station => station.StandbyPeriod);
            StationModel lastStation = _stations[targetIndex].FirstOrDefault(station => station.StandbyPeriod == lastStationTime);

            if (lastStation != null)
                pathStack.Push(lastStation);

            while (table[targetIndex].PrevStation != null)
            {
                pathStack.Push(table[targetIndex].PrevStation);
                targetIndex = table[targetIndex].PrevStation.Number;
            }

            var stations = new List<StationModel>(pathStack.Count);

            while (pathStack.Count > 0)
            {
                var station = pathStack.Pop();
                pathTime += station.StandbyPeriod;
                stations.Add(station);
            }

            return new StationsPathModel(stations, pathTime);
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
