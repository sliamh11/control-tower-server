using BLL.Interfaces;
using BLL.Logic;
using Common.Enums;
using Common.Models;
using System;
using System.Diagnostics;
using System.Threading;

namespace BLL.Data_Objects
{
    public class DepartureObj : IDataObj
    {
        #region Private Fields
        private IDepartureLogic _depLogic;
        private IStationsLogic _stationsLogic;
        private ITowerLogic _towerLogic;
        private Timer _timer;
        private TimeSpan _dueTime;
        private TimeSpan _periodTime;
        #endregion

        #region Public Properties
        public FlightModel Flight { get; set; }
        public StationsPathModel StationsPath { get; set; }
        #endregion

        // For params
        public DepartureObj(string flightId, ITowerLogic towerLogic)
        {
            _depLogic = new DepartureLogic();
            _stationsLogic = new StationsLogic();
            _towerLogic = towerLogic;
            Flight = new FlightModel(flightId, FlightType.Departure);
            InitDeparture();
        }

        private async void InitDeparture()
        {
            if (await _depLogic.StartDepartureAsync(this))
            {
                //_dueTime = StationsPath.CurrentStation.StandbyPeriod;
                _dueTime = new TimeSpan(0, 0, 10);
                _periodTime = new TimeSpan(0);
                _timer = new Timer(OnTimerElapsed, null, _dueTime, _periodTime);
            }
            else
            {
                // If somehow it isn't possible to get a path - send back to queue
                // Will accure only in specific timings like an object has been made and at the same time an important station was blocked
                _towerLogic.AddToWaitingList(Flight);
                // Send an update to the plane / client
            }
        }

        private async void OnTimerElapsed(object state)
        {
            Debug.WriteLine($"Flight: {Flight.Id}, Station: {StationsPath.CurrentStation.Number}");
            // Add try catch for StationNotFoundException and other exceptions.

            if (await _depLogic.FinishDapertureAsync(this))
            {
                Debug.WriteLine($"Flight {Flight.Id} has departured.");
                _timer.Dispose();
                return;
            }

            if (await _stationsLogic.MoveToNextStationAsync(this))
            {
                Debug.WriteLine($"Flight {Flight.Id} moved to station {StationsPath.CurrentStation.Number}");
                // Update the _periodTime to the station's StandbyTime.
                _periodTime = StationsPath.CurrentStation.StandbyPeriod;
                _timer.Change(new TimeSpan(0, 0, 10), new TimeSpan(0));
            }
            else
            {
                // Delay scedhualed take off time.
                Flight.DepartureTime += _periodTime;
            }
        }
    }
}
