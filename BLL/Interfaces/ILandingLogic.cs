using BLL.Data_Objects;
using Common.Models;

namespace BLL.Interfaces
{
    internal interface ILandingLogic : IStationsLogic
    {
        bool StartLanding(LandingObj landingObj);
        bool FinishLanding(LandingObj landingObj);
    }
}