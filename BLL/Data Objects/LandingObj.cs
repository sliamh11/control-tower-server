using BLL.Interfaces;
using BLL.Logic;
using Common.Enums;
using Common.Models;
using System;
using System.Diagnostics;
using System.Threading;

namespace BLL.Data_Objects
{
    public class LandingObj : IDataObj
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

        public LandingObj(string flightId)
        {

            _logic = new LandingLogic();
            InitLanding(flightId);
        }

        private void InitLanding(string flightId)
        {
            Flight = new FlightModel(flightId, FlightType.Landing);
            if (_logic.StartLanding(this))
            {
                //_dueTime = StationsPath.CurrentStation.StandbyPeriod;
                _dueTime = new TimeSpan(0, 0, 10);
                _periodTime = new TimeSpan(0);
                _timer = new Timer(OnTimerElapsed, null, _dueTime, _periodTime);
                //worker = new BackgroundWorker();
                //worker.DoWork += (s,e) => InitTimer;
            }
            else
            {
                // Add to TowerManager Landing Queue
            }
        }

        //private void InitTimer()
        //{
        //    _dueTime = new TimeSpan(0);
        //    _periodTime = StationsPath.CurrentStation.StandbyPeriod;
        //    _timer = new Timer(OnTimerElapsed, null, _dueTime, _periodTime);
        //}



        private void OnTimerElapsed(object state)
        {
            //Program.WriteToConsole($"Flight: {Flight.Id}, Station: {StationsPath.CurrentStation.Number}");
            Debug.WriteLine($"Flight: {Flight.Id}, Station: {StationsPath.CurrentStation.Number}");
            // Add setter for estimated landing time (with the time from StationsModelPath)
            // Add try catch for StationNotFoundException and other exceptions.

            if (_logic.FinishLanding(this))
            {
                Debug.WriteLine($"Flight {Flight.Id} has landed.");
                _timer.Dispose();
                return;
            }

            if (_logic.MoveToNextStation(this))
            {
                Debug.WriteLine($"FLight {Flight.Id} moved to station {StationsPath.CurrentStation.Number}");
                // Update the _periodTime to the station's StandbyTime.
                _periodTime = StationsPath.CurrentStation.StandbyPeriod;
                _timer.Change(new TimeSpan(0, 0, 10), new TimeSpan(0));
            }
            else
            {
                // Delay scedhualed landing.
                Flight.LandingTime += _periodTime;
            }
        }
    }
}
