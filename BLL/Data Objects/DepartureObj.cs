using BLL.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Data_Objects
{
    internal class DepartureObj : IDataObj
    {
        private IDepartureLogic _logic;
        public FlightModel Flight { get; set; }
        public StationsPathModel StationsPath { get; set; }

        // For DI
        public DepartureObj(IDepartureLogic departureLogic)
        {
            _logic = departureLogic;
        }
        
        // For params
        public DepartureObj(string flightId)
        {
            Flight = new FlightModel(flightId);
            StationsPath = _logic?.StartDeparture(this);
            //Timer timer = new Timer(cbFunc,null,60000,randomNumber);
        }
    }
}
