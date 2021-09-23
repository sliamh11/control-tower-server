using BLL.Data_Objects;
using BLL.Interfaces;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace BLL
{
    public class StationsManager : IStationsManager, ITowerLogic
    {
        private IStationsState _stationsState;
        private LinkedList<FlightModel> _waitingFlightsList;
        private Timer _queueTimer;
        private bool _canLand;
        private bool _canDeparture;

        public StationsManager()
        {
            _stationsState = StationsState.Instance;
            _waitingFlightsList = new LinkedList<FlightModel>();
            _canLand = true;
            _canDeparture = true;
            _queueTimer = new Timer(60000);
            _queueTimer.Elapsed += (s, e) => OnTimerElapsed();
            _queueTimer.Start();
        }

        private void OnTimerElapsed()
        {
            // Reset canLand & canDeparture's status
            _canLand = true;
            _canDeparture = true;
            if (_waitingFlightsList.Count > 0)
            {
                foreach (var flight in _waitingFlightsList)
                {
                    bool isLanding = flight.Type == FlightType.Landing;
                    // Check if flight is relevant at all.
                    if (isLanding && !_canLand)
                        continue;

                    if (!isLanding && !_canDeparture)
                        continue;

                    if (_stationsState.CanAddFlight(flight.Type))
                    {
                        if (isLanding)
                            StartLanding(flight.Id);
                        else
                            StartDeparture(flight.Id);

                        _waitingFlightsList.Remove(flight);
                    }
                    else
                    {
                        if (isLanding)
                            _canLand = false;
                        else
                            _canDeparture = false;
                    }

                    // If no available spots at all - exit current timer's interval.
                    if (!_canLand && !_canDeparture)
                        return;
                }
            }
        }

        public void AddStation(List<StationModel> newStation)
        {
            if (newStation == null || newStation.Count == 0)
                throw new ArgumentException("New station is not valid.");

            _stationsState.AddStation(newStation);
        }

        public IReadOnlyList<IReadOnlyList<StationModel>> GetStationsState() => _stationsState.GetStationsState();

        public bool StartDeparture(string flightId)
        {
            if (_canDeparture)
            {
                _ = new DepartureObj(flightId, this); // Works on another thread with a timer
                return true;
            }
            else
            {
                var flight = new FlightModel(flightId, FlightType.Departure);
                _waitingFlightsList.AddLast(flight);
            }

            // SignalR notification to client side & DB
            return false;
        }

        public bool StartLanding(string flightId)
        {
            if (_canLand)
            {
                _ = new LandingObj(flightId, this); // Works on another thread with a timer
                return true;
            }
            else
            {
                var flight = new FlightModel(flightId, FlightType.Landing);
                _waitingFlightsList.AddLast(flight);
            }

            // SignalR notification to client side & DB
            return false;
        }

        public void AddToWaitingList(FlightModel flight) => _waitingFlightsList.AddLast(flight);

        private bool CanMoveToNextStation(IDataObj dataObj)
        {
            var nextStation = dataObj.StationsPath.Path.First.Next?.Value;
            if (nextStation == null)
                throw new StationNotFoundException();

            return _stationsState.IsStationEmpty(nextStation);
        }

        public bool MoveToNextStation(IDataObj dataObj)
        {
            var currStation = dataObj.StationsPath.CurrentStation;
            var targetStation = dataObj.StationsPath.Path.Last.Value;
            try
            {
                if (CanMoveToNextStation(dataObj))
                {
                    var nextStation = dataObj.StationsPath.Path.First.Next.Value;
                    _stationsState.FindFastestPath(nextStation, targetStation);
                    _stationsState.MoveToStation(currStation, nextStation, dataObj.Flight);
                    dataObj.StationsPath.Path.RemoveFirst(); // Remove old station
                    dataObj.StationsPath.CurrentStation = nextStation; // Update the current station.

                    // Call the StateUpdated() func.
                    // Update DB?
                    return true;
                }
            }
            catch (StationNotFoundException)
            {
                // Re-set the flight's stations path.
                dataObj.StationsPath = _stationsState.FindFastestPath(currStation, targetStation);
                return MoveToNextStation(dataObj);
            }
            return false;
            // Other exceptions will be cought in the service?
        }

        public async Task<bool> MoveToNextStationAsync(IDataObj dataObj)
        {
            return await Task.Run(() => MoveToNextStation(dataObj));
        }
    }
}
