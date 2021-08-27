using Common.Models;

namespace BLL.Interfaces
{
    public interface IStationsLogic
    {
        bool CanMoveToNextStation(FlightModel flight);
        bool MoveToNextStation(FlightModel flight);
    }
}
