using BLL.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Data_Objects
{
    internal class LandingObj : IDataObj
    {
        #region Private Fields
        private ILandingLogic _logic;
        private Timer _timer;
        private int _dueTime;
        private int _periodTime;
        #endregion

        #region Public Properties
        public FlightModel Flight { get; set; }
        public StationsPathModel StationsPath { get; set; }
        #endregion

        public LandingObj(ILandingLogic landingLogic)
        {
            _logic = landingLogic;
        }

        public LandingObj(string flightId)
        {
            Flight = new FlightModel(flightId);
            StationsPath = _logic?.StartLanding(this);
            _dueTime = 60000;
            _periodTime = 60000;
            _timer = new Timer(OnTimerElapsed, null, _dueTime, _periodTime);
        }

        private void OnTimerElapsed(object state)
        {
            if (_logic.FinishLanding(this))
            {
                _timer.Dispose();
                return;
            }

            // Change next period time (might need 0 instead _dueTime)
            //Random rand = new Random();
            // _periodTime = rand.Next(30000, 60001);
            //_timer.Change(_dueTime, _periodTime); // roll a random number between 30-60 seconds.

            if (!_logic.MoveToNextStation(this))
            {
                // Update the flight's final time by _periodTime 
            }
        }
    }
}
