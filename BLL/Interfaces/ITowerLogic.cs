using Common.Models;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ITowerLogic
    {
        Task<bool> MoveToNextStationAsync(IDataObj dataObj);
        void AddToWaitingList(FlightModel flight);
    }
}
