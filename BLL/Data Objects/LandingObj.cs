using BLL.Interfaces;
using Common.Enums;
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
        private TimeSpan _dueTime;
        private TimeSpan _periodTime;
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
            InitLanding(flightId);
        }

        private void InitLanding(string flightId)
        {
            Flight = new FlightModel(flightId, FlightType.Landing);
            if (_logic.StartLanding(this))
            {
                _dueTime = new TimeSpan(0);
                _periodTime = StationsPath.CurrentStation.StandbyPeriod;
                _timer = new Timer(OnTimerElapsed, null, _dueTime, _periodTime);
            }
            else
            {
                // Add to TowerManager Landing Queue
            }
        }

        // Add setter for estimated landing time (with the time from StationsModelPath)
        private void OnTimerElapsed(object state)
        {
            if (_logic.FinishLanding(this))
            {
                _timer.Dispose();
                return;
            }


            if (_logic.MoveToNextStation(this))
            {
                // Update the _periodTime to the station's StandbyTime.
                _periodTime = StationsPath.CurrentStation.StandbyPeriod;
                _timer.Change(new TimeSpan(0), _periodTime);
            }
            else
            {
                // Delay scedhualed landing.
                Flight.LandingTime += _periodTime;
            }
        }
    }
}
