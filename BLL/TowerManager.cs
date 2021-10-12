using BLL.Data_Objects;
using BLL.Interfaces;
using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    /// <summary>
    /// This class manages the incoming API requests from the Services layer.
    /// </summary>
    public class TowerManager : ITowerManager
    {
        #region Private Fields
        private IStationsState _stationsState;
        private IServiceProvider _provider;
        private LinkedList<FlightModel> _waitingFlightsList;
        private Timer _queueTimer;
        #endregion

        public TowerManager(IServiceProvider provider, IStationsState stationsState)
        {
            // Providers
            _stationsState = stationsState;
            _provider = provider;

            // Init timer
            _waitingFlightsList = new LinkedList<FlightModel>();
            _queueTimer = new Timer(15000); // 15 seconds.
            _queueTimer.Elapsed += (s, e) => OnTimerElapsed();
            _queueTimer.Start();
        }

        #region Timer Handler
        private async void OnTimerElapsed()
        {
            try
            {
                if (_waitingFlightsList.Count > 0)
                {
                    var removalList = await GetRemovableFlights();
                    foreach (var flight in removalList)
                        _waitingFlightsList.Remove(flight);
                }
            }
            catch (Exception)
            {
                // log exception
            }
        }
        #endregion

        #region Public Functions
        public bool AddStation(Dictionary<string, StationModel> newStations)
        {
            if (newStations == null || newStations.Count == 0)
                throw new ArgumentException("New station is not valid.");

            return _stationsState.AddStation(newStations);
        }

        public IReadOnlyList<IReadOnlyDictionary<string, StationModel>> GetStationsState()
        {
            return _stationsState.GetStationsState();
        }

        public async Task<bool> StartDepartureAsync(string flightId)
        {
            if (_stationsState.CanAddFlight(FlightType.Departure))
            {
                var depLogic = _provider.GetRequiredService<IDepartureLogic>();
                var stationsLogic = _provider.GetRequiredService<IStationsLogic>();

                var depObj = new DepartureObj(depLogic, stationsLogic, flightId);
                if (await depObj.Start())
                    return true;
            }

            AddToWaitingList(new FlightModel(flightId, FlightType.Departure));
            return false;
        }

        public async Task<bool> StartLandingAsync(string flightId)
        {
            if (_stationsState.CanAddFlight(FlightType.Landing))
            {
                var landLogic = _provider.GetRequiredService<ILandingLogic>();
                var stationsLogic = _provider.GetRequiredService<IStationsLogic>();

                var landObj = new LandingObj(landLogic, stationsLogic, flightId);
                if (await landObj.Start())
                    return true;
            }

            AddToWaitingList(new FlightModel(flightId, FlightType.Landing));
            return false;
        }

        public void AddToWaitingList(FlightModel flight)
        {
            _waitingFlightsList.AddLast(flight);
        }
        #endregion

        #region Private Functions
        private async Task<LinkedList<FlightModel>> GetRemovableFlights()
        {
            bool canLand = true;
            bool canDeparture = true;
            var removableList = new LinkedList<FlightModel>();
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
                        await StartLandingAsync(flight.Id);
                    else
                        await StartDepartureAsync(flight.Id);

                    removableList.AddLast(flight);
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
            return removableList;
        }
        #endregion
    }
}
