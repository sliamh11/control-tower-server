using Common.Models;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ITowerLogic
    {
        void AddToWaitingList(FlightModel flight);
    }
}
