using BLL.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Data_Objects
{
    internal class DepartureObj
    {
        private IDepartureLogic _logic;
        public FlightModel Flight { get; set; }
        public StationModel Station { get; set; }

        // For DI
        public DepartureObj(IDepartureLogic departureLogic)
        {
            _logic = departureLogic;
        }
        
        // For params
        public DepartureObj(string flightId)
        {
            Flight = new FlightModel(flightId);
            Station = _logic?.StartDeparture(Flight);
        }
    }
}
