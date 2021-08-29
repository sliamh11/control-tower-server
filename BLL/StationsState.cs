using BLL.Interfaces;
using Common.Data_Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    // Holds the Data Structure.
    public class StationsState : IStationsState
    {
        private StationsGraph _stations;
        public StationsGraph Stations
        {
            get { return _stations; }

            set
            {
                if (_stations == null)
                    _stations = value;
            }
        }

        public StationsState()
        {
            _stations = new StationsGraph();
        }

        public StationsGraph GetStationsState() => Stations;

        public void LoadStations()
        {
            throw new NotImplementedException();
        }

        public void StateUpdated()
        {
            // Called each time something updated the _stations field.
            // Emits the updated state to all subscribers (client + DB).

            // Maybe somehow updated it in 'set' if its even possible?
            throw new NotImplementedException();
        }
    }
}
