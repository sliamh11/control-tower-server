using BLL.Interfaces;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Common.Structs;
using System;
using System.Threading.Tasks;

namespace BLL.Logic
{
    /// <summary>
    /// A class for similiar functionality of Departure & Landing objects.
    /// </summary>
    public class StationsLogic : IStationsLogic
    {
        private readonly IStationsState _stationsState;

        public StationsLogic(IStationsState stationsState)
        {
            _stationsState = stationsState;
        }

        #region Public Functions
        public bool MoveToNextStation(IDataObj dataObj)
        {
            try
            {
                bool result = TryMoveByPlannedPath(dataObj);
                return result ? result : TryMoveByNewPath(dataObj);
            }
            catch (Exception ex)
            {
                if (ex is StationNotAvailableException || ex is StationNotFoundException)
                    return HandleStationExceptions(dataObj);

                throw;
            }
        }
        public async Task<bool> MoveToNextStationAsync(IDataObj dataObj)
        {
            return await Task.Run(() => MoveToNextStation(dataObj));
        }
        #endregion

        #region Helper Functions
        private bool CanMoveToStation(StationModel toStation)
        {
            if (toStation == null)
                throw new StationNotFoundException();

            if (toStation.Status != StationStatuses.Open)
                throw new StationNotAvailableException();

            return _stationsState.IsStationEmpty(toStation);
        }
        private bool MoveStation(IDataObj dataObj)
        {
            var currStation = dataObj.StationsPath.CurrentStation;
            var nextStation = dataObj.StationsPath.Path.First.Next?.Value;
            if (_stationsState.MoveToStation(currStation, nextStation, dataObj.Flight))
            {
                dataObj.StationsPath.Path.RemoveFirst(); // Remove old station
                dataObj.StationsPath.CurrentStation = nextStation; // Update the current station.
                return true;
            }
            return false;
        }
        private bool HandleStationExceptions(IDataObj dataObj)
        {
            // Re-set the flight's stations path.
            var currStation = dataObj.StationsPath.CurrentStation;
            var finalStation = dataObj.StationsPath.Path.Last.Value;
            dataObj.StationsPath = _stationsState.FindFastestPath(currStation, finalStation);
            return MoveToNextStation(dataObj);
        }
        private bool TryMoveByPlannedPath(IDataObj dataObj)
        {
            var nextStation = dataObj.StationsPath.Path.First.Next?.Value;

            if (CanMoveToStation(nextStation))
                return MoveStation(dataObj);

            return false;
        }
        private bool TryMoveByNewPath(IDataObj dataObj)
        {
            var currStation = dataObj.StationsPath.Path.First.Value;

            var pathEdges = GetNewPathEdges(currStation);

            var newPath = _stationsState.FindFastestPath(pathEdges.StartStation, pathEdges.EndStation);
            if (newPath == null)
                return false;

            _stationsState.RemoveFlight(currStation); // Remove old record from stations graph
            dataObj.StationsPath = newPath;
            return MoveStation(dataObj);
        }

        private PathEdgesStruct GetNewPathEdges(StationModel currentStation)
        {
            if (currentStation == null)
                throw new StationNotFoundException();

            var stations = _stationsState.GetStationsState();
            PathEdgesStruct pathEdges = new PathEdgesStruct();
            foreach (var station in stations[currentStation.Number].Values)
            {
                foreach (var item in stations[station.NextStation].Values)
                {
                    if (item.CurrentFlight == null)
                    {
                        pathEdges.StartStation = station;
                        pathEdges.EndStation = item;
                        break;
                    }
                }
            }

            return pathEdges.StartStation == null || pathEdges.EndStation == null
                ? default
                : pathEdges;
        }
        #endregion
    }
}
