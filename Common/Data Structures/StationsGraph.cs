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

        // Path calculation (Diakstra's algo?) -> return fastest route.
        // Maybe categorize each station added, then add them to a dictionary / array and use those as specific entry points
        // For example, if station 5 is considered as a Departure station, it is possible to check if the path from this station to the runway is clear, etc.
    }
}
