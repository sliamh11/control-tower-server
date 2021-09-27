using BLL.Interfaces;
using BLL.Logic;
using Common.Enums;
using Common.Models;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Data_Objects
{
    public class LandingObj : IDataObj
    {
        #region Private Fields
        private ILandingLogic _landLogic;
        private IStationsLogic _stationsLogic;
        private Timer _timer;
        private TimeSpan _delayTime;
        #endregion

        #region Public Properties
        public FlightModel Flight { get; set; }
        public StationsPathModel StationsPath { get; set; }
        #endregion

        public LandingObj(string flightId)
        {
            _landLogic = new LandingLogic();
            _stationsLogic = new StationsLogic();
            Flight = new FlightModel(flightId, FlightType.Landing);
        }

        public async Task<bool> Start()
        {
            if (await _landLogic.StartLandingAsync(this))
            {
                _delayTime = StationsPath.CurrentStation.StandbyPeriod;
                _timer = new Timer(OnTimerElapsed, null, _delayTime, _delayTime);
                return true;
            }
            return false;
        }

        private async void OnTimerElapsed(object state)
        {
            // Add try catch for StationNotFoundException and other exceptions.

            if (await _landLogic.FinishLandingAsync(this))
            {
                _timer.Dispose();
                return;
            }

            if (await _stationsLogic.MoveToNextStationAsync(this))
            {
                // Update the _periodTime to the station's StandbyTime.
                _delayTime = StationsPath.CurrentStation.StandbyPeriod;
                _timer.Change(_delayTime, _delayTime);
            }
            else
            {
                // Delay scedhualed landing.
                Flight.LandingTime += _delayTime;
            }
        }
    }
}
