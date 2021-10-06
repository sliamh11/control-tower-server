using BLL.Interfaces;
using Common.Enums;
using Common.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace BLL.Data_Objects
{
    public class DepartureObj : IDepartureObj
    {
        #region Private Fields
        private IDepartureLogic _depLogic;
        private IStationsLogic _stationsLogic;
        private Timer _timer;
        private TimeSpan _delayTime;
        #endregion

        #region Public Properties
        public FlightModel Flight { get; set; }
        public StationsPathModel StationsPath { get; set; }
        #endregion

        // For params
        public DepartureObj(string flightId)
        {
            Flight = new FlightModel(flightId, FlightType.Departure);
        }

        // For DI
        public DepartureObj(IDepartureLogic depLogic, IStationsLogic stationsLogic, string flightId = "") : this(flightId)
        {
            _depLogic = depLogic;
            _stationsLogic = stationsLogic;
        }

        public async Task<bool> Start()
        {
            if (await _depLogic.StartDepartureAsync(this))
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
            if (await _depLogic.FinishDapertureAsync(this))
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
                // Delay scheduled take off time.
                Flight.DepartureTime += _delayTime;
            }
        }
    }
}
