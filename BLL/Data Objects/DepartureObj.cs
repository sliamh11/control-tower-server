using BLL.Interfaces;
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
        private IDepartureLogic _logic;
        private Timer _timer;
        private TimeSpan _dueTime;
        private TimeSpan _periodTime;
        #endregion

        #region Public Properties
        public FlightModel Flight { get; set; }
        public StationsPathModel StationsPath { get; set; }
        #endregion
        // For DI
        public DepartureObj(IDepartureLogic departureLogic)
        {
            _logic = departureLogic;
        }
        
        // For params
        public DepartureObj(string flightId)
        {
            Flight = new FlightModel(flightId, FlightType.Departure);
            if (_logic.StartDeparture(this))
            {
                //_dueTime = StationsPath.CurrentStation.StandbyPeriod;
                _dueTime = new TimeSpan(0, 0, 10);
                _periodTime = new TimeSpan(0);
                _timer = new Timer(OnTimerElapsed, null, _dueTime, _periodTime);
            }
            else
            {
                // Add to TowerManager Departure Queue
            }
        }

        private void OnTimerElapsed(object state)
        {
            Debug.WriteLine($"Flight: {Flight.Id}, Station: {StationsPath.CurrentStation.Number}");
            // Add try catch for StationNotFoundException and other exceptions.

            if (_logic.FinishDaperture(this))
            {
                Debug.WriteLine($"Flight {Flight.Id} has departured.");
                _timer.Dispose();
                return;
            }

            if (_logic.MoveToNextStation(this))
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
