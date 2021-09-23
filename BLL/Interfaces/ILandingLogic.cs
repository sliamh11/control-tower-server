using BLL.Data_Objects;
using Common.Models;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ILandingLogic
    {
        Task<bool> StartLandingAsync(LandingObj landingObj);
        Task<bool> FinishLandingAsync(LandingObj landingObj);
    }
}