using BLL.Interfaces;
using Common.Enums;
using Common.Models;

namespace BLL.Data_Objects
{
    public class DepartureObj : IDataObj
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
            Flight = new FlightModel(flightId, FlightType.Departure);
            StationsPath = _logic?.StartDeparture(this);
            //Timer timer = new Timer(cbFunc,null,60000,randomNumber);
        }
    }
}
