﻿using BLL.Interfaces;
using Common.Enums;
using Common.Exceptions;
using System;
using System.Threading.Tasks;

namespace BLL.Logic
{
    /// <summary>
    /// A class for generic functions which are shared between Departure & Landing objects.
    /// </summary>
    public class StationsLogic : IStationsLogic
    {
        private readonly IStationsState _stationsState;

        public StationsLogic()
        {
            _stationsState = StationsState.Instance;
        }

        private bool CanMoveToNextStation(IDataObj dataObj)
        {
            var nextStation = dataObj.StationsPath.Path.First.Next?.Value;
            if (nextStation == null)
                throw new StationNotFoundException();

            if (nextStation.Status != StationStatuses.Open)
                throw new StationNotAvailableException();

            return _stationsState.IsStationEmpty(nextStation);
        }

        public bool MoveToNextStation(IDataObj dataObj)
        {
            var currStation = dataObj.StationsPath.CurrentStation;
            try
            {
                if (CanMoveToNextStation(dataObj))
                {
                    var nextStation = dataObj.StationsPath.Path.First.Next.Value;
                    if (_stationsState.MoveToStation(currStation, nextStation, dataObj.Flight))
                    {
                        dataObj.StationsPath.Path.RemoveFirst(); // Remove old station
                        dataObj.StationsPath.CurrentStation = nextStation; // Update the current station.
                        // Call the StateUpdated() func.
                        // Update DB?
                        return true;
                    }
                }
                    return false;
            }
            catch (Exception ex) // Needs to be StationNotAvailableException
            {
                if (ex is StationNotAvailableException || ex is StationNotFoundException)
                {
                    // Re-set the flight's stations path.
                    var finalStation = dataObj.StationsPath.Path.Last.Value;
                    dataObj.StationsPath = _stationsState.FindFastestPath(currStation, finalStation);
                    return MoveToNextStation(dataObj);
                }
                throw;
            }
        }

        public async Task<bool> MoveToNextStationAsync(IDataObj dataObj)
        {
            return await Task.Run(() => MoveToNextStation(dataObj));
        }
    }
}
