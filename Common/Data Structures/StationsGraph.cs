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
        // Do I need stations to know their numbers?
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

        // Create add stations function?
        // (station numbers are pre-set, therefor no need for this, but as a future addition it might be good)
    }
}
