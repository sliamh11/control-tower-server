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
        // Do I need stations to know their numbers? -> downside - items are less generic because the station number might be changed with the list.
        // A List of Linked lists of type StationModel might be better
        // because I can add / remove stations while maintaining the O(1) indexer.
        //private List<LinkedList<StationModel>> _stations;

        private LinkedList<StationModel>[] _stations;
        public StationsGraph()
        {
            _stations = new LinkedList<StationModel>[7];
            // Waiting stations
            _stations[0] = new LinkedList<StationModel>(new StationModel[] { new StationModel(0, 1) });
            _stations[1] = new LinkedList<StationModel>(new StationModel[] { new StationModel(1, 2) });
            _stations[2] = new LinkedList<StationModel>(new StationModel[] { new StationModel(2, 3) });
            // Runway
            _stations[3] = new LinkedList<StationModel>(new StationModel[] { new StationModel(3, 4) });
            // Airport & Depratures
            _stations[4] = new LinkedList<StationModel>(new StationModel[] { new StationModel(4, 5), new StationModel(4, 6) });
            _stations[5] = new LinkedList<StationModel>(new StationModel[] { new StationModel(5, 7) }); // Also has an exit
            _stations[6] = new LinkedList<StationModel>(new StationModel[] { new StationModel(6, 7) }); // Also has an exit
            _stations[7] = new LinkedList<StationModel>(new StationModel[] { new StationModel(7, 3) }); // Station before runway.
        }

        // Create add stations function

        // Path calculation (Diakstra's algo?) -> return fastest route.
        // Maybe categorize each station added, then add them to a dictionary / array and use those as specific entry points
        // For example, if station 5 is considered as a Departure station, it is possible to check if the path from this station to the runway is clear, etc.
    }
}
