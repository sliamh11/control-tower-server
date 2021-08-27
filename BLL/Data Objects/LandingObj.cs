using BLL.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Data_Objects
{
    internal class LandingObj
    {
        private ILandingLogic _logic;
        public FlightModel Flight { get; set; }
        public StationModel Station { get; set; }

        public LandingObj(ILandingLogic landingLogic)
        {
            _logic = landingLogic;
        }

        public LandingObj(string flightId)
        {
            Flight = new FlightModel(flightId);
            Station = _logic?.StartLanding(Flight);
        }
    }
}
