using BLL.Interfaces;
using Common.Exceptions;
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
                    _stationsState.FindFastestPath(nextStation, targetStation); // Check if needed at all
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
