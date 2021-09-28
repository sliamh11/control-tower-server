﻿using BLL.Interfaces;
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

        // For DI - IServiceProvider as param to keep the DI graph connected.
        public DepartureObj(IServiceProvider provider, string flightId = "") : this(flightId)
        {
            _depLogic = provider.GetRequiredService<IDepartureLogic>();
            _stationsLogic = provider.GetRequiredService<IStationsLogic>();
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
            var id = Flight.Id.Substring(0, 7);
            Debug.WriteLine($"Flight: {id}, Station: {StationsPath.CurrentStation.Number}");
            // Add try catch for StationNotFoundException and other exceptions.
            if (await _depLogic.FinishDapertureAsync(this))
            {
                Debug.WriteLine($" ~~~ Flight {id} has finished landing proccess. ~~~");
                _timer.Dispose();
                return;
            }

            if (await _stationsLogic.MoveToNextStationAsync(this))
            {
                Debug.WriteLine($"+++ Flight {id} has moved to station {StationsPath.CurrentStation.Number}. +++");
                // Update the _periodTime to the station's StandbyTime.
                _delayTime = StationsPath.CurrentStation.StandbyPeriod;
                _timer.Change(_delayTime, _delayTime);
            }
            else
            {
                Debug.WriteLine($"--- Flight {id} was delayed. ---");
                // Delay scedhualed take off time.
                Flight.DepartureTime += _delayTime;
            }
        }
    }
}
