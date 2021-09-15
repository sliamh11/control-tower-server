using Common.Exceptions;
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
            int index = station.IndexOf(updatedStation); // Compares with .Equals() (supposes to work on ref)
            //int index = station.FindIndex(station => station.CompareTo(updatedStation) == 0);
            if (index >= 0)
            {
                station[index] = updatedStation; // Should work (is by reference), if not - change the _stations directly
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

        // O(n Log n)
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

            var stations = new List<StationModel>(pathStack.Count + 1); // +1 for the last station

            while (pathStack.Count > 0)
            {
                var station = pathStack.Pop();
                pathTime += station.StandbyPeriod;
                stations.Add(station);
            }

            // Find the target station by minimum standby time - O(n)
            StationModel lastStation = _stations[targetIndex].Aggregate((minTimeStation, station) =>
            station.StandbyPeriod < (minTimeStation?.StandbyPeriod ?? station.StandbyPeriod)
            ? station : minTimeStation);

            stations.Add(lastStation);
            pathTime += lastStation.StandbyPeriod;

            return new StationsPathModel(stations, pathTime);
        }

        // O(n)
        public bool IsStationEmpty(StationModel station)
        {
            var currStation = _stations[station.Number].Find(x => x.CompareTo(station) == 0);
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
