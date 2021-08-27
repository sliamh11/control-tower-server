using Common.Models;

namespace BLL.Interfaces
{
    public interface ILandingLogic : IStationsLogic
    {
        StationModel StartLanding(FlightModel flight);
        bool CanFinishLanding();
        void FinishLanding();
    }
}