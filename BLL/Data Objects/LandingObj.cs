using BLL.Interfaces;
using Common.Enums;
using Common.Models;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Data_Objects
{
    public class LandingObj : ILandingObj
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

        // For params
        public LandingObj(string flightId)
        {
            Flight = new FlightModel(flightId, FlightType.Landing);
        }

        // For DI
        public LandingObj(ILandingLogic landLogic, IStationsLogic stationsLogic, string flightId) : this(flightId)
        {
            _landLogic = landLogic;
            _stationsLogic = stationsLogic;
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
                // Update the _delayTime to the station's StandbyTime.
                _delayTime = StationsPath.CurrentStation.StandbyPeriod;
                _timer.Change(_delayTime, _delayTime);
            }
            else
            {
                // Delay scheduled landing.
                Flight.LandingTime += _delayTime;
            }
        }
    }
}
