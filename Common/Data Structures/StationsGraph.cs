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

        // Dijakstra's algo
        public IReadOnlyList<StationModel> FindFastestPath(int startIndex)
        {
            // How will it work:
            // Instead of saving it all in a big array, save ONLY the minimal path (get it as a param each function call).
            // Side note - maybe make an inner class in the graph which represents the data - understand what is needed
            if (startIndex < 0 || startIndex >= _stations.Count)
                throw new ArgumentOutOfRangeException("Start index was out of the array's boundries.");

            StationsTable[] table = new StationsTable[_stations.Count];
            InitStationsTable(table, startIndex);
            FillStationsTable(startIndex, table);
            return GetFastestPath(table);
        }

        // O(n)
        private void InitStationsTable(StationsTable[] table, int source)
        {
            for (int i = 0; i < _stations.Count; i++)
            {
                table[i] = new StationsTable
                {
                    StationIndex = i,
                    IsVisited = false,
                    PrevStation = null,
                    Weight = new TimeSpan(1, 0, 0, 0) // 1 Day as max value.
                };
            }
            table[source].Weight = new TimeSpan(0);
        }

        // O(n^2) (recursive + inner loops)
        private void FillStationsTable(int index, StationsTable[] table)
        {
            if (table.All(station => station.IsVisited))
                return;

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

            // Get the table with the lowest weight which havn't been visited yet.
            index = table.Where(i => !i.IsVisited).OrderBy(i => i.Weight).Select(i => i.StationIndex).FirstOrDefault();
            FillStationsTable(index, table);
        }
        // O(n^2) (loop in a loop)
        private IReadOnlyList<StationModel> GetFastestPath(StationsTable[] table)
        {
            TimeSpan minimalTime = new TimeSpan(1, 0, 0, 0);
            List<StationModel> path = null;
            foreach (var item in table)
            {
                Stack<StationModel> pathStack = new Stack<StationModel>();
                int tempIndex = item.StationIndex;
                TimeSpan pathTime = new TimeSpan(0);
                while (table[tempIndex].PrevStation != null)
                {
                    pathStack.Push(table[tempIndex].PrevStation);
                    tempIndex = table[tempIndex].PrevStation.Number;
                    pathTime += table[tempIndex].Weight;
                }

                if (pathTime < minimalTime)
                {
                    var stations = new List<StationModel>(pathStack.Count);
                    while (pathStack.Count > 0)
                    {
                        stations.Add(pathStack.Pop());
                    }
                    path = stations;
                    minimalTime = pathTime;
                }
            }
            return path;
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
