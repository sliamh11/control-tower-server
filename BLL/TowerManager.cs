using BLL.Data_Objects;
using BLL.Interfaces;
using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace BLL
{
    public class TowerManager : ITowerManager, ITowerLogic
    {
        private IStationsState _stationsState;
        private LinkedList<FlightModel> _waitingFlightsList;
        private Timer _queueTimer;

        public TowerManager()
        {
            _stationsState = StationsState.Instance;
            _waitingFlightsList = new LinkedList<FlightModel>();
            _queueTimer = new Timer(30000); // 30 Seconds
            _queueTimer.Elapsed += (s, e) => OnTimerElapsed();
            _queueTimer.Start();
        }

        private void OnTimerElapsed()
        {
            // Reset canLand & canDeparture's status
            bool canLand = true;
            bool canDeparture = true;
            if (_waitingFlightsList.Count > 0)
            {
                LinkedList<FlightModel> removalList = new LinkedList<FlightModel>();
                foreach (var flight in _waitingFlightsList)
                {
                    bool isLanding = flight.Type == FlightType.Landing;
                    // Check if flight is relevant at all.
                    if (isLanding && !canLand)
                        continue;

                    if (!isLanding && !canDeparture)
                        continue;

                    if (_stationsState.CanAddFlight(flight.Type))
                    {
                        if (isLanding)
                            StartLanding(flight.Id);
                        else
                            StartDeparture(flight.Id);

                        removalList.AddLast(flight);
                    }
                    else
                    {
                        if (isLanding)
                            canLand = false;
                        else
                            canDeparture = false;
                    }

                    // If no available spots at all - exit current timer's interval.
                    if (!canLand && !canDeparture)
                        break;
                }

                foreach (var flight in removalList)
                    _waitingFlightsList.Remove(flight);
            }
        }

        public void AddStation(List<StationModel> newStation)
        {
            if (newStation == null || newStation.Count == 0)
                throw new ArgumentException("New station is not valid.");

            _stationsState.AddStation(newStation);
        }

        public IReadOnlyList<IReadOnlyList<StationModel>> GetStationsState() => _stationsState.GetStationsState();

        // The flight arrives to the function but still not shows, like the timer wouldn't even run. (maybe something with StartDepartureAsync function?)
        public bool StartDeparture(string flightId)
        {
            if (_stationsState.CanAddFlight(FlightType.Departure))
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
            if (_stationsState.CanAddFlight(FlightType.Landing))
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
    }
}
