using Common.Models;

namespace BLL.Interfaces
{
    public interface IDepartureLogic : IStationsLogic
    {
        StationModel StartDeparture(FlightModel flight);
        bool CanFinishDeparture();
        void FinishDaperture();
    }
}