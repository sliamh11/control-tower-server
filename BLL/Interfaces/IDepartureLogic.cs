using BLL.Data_Objects;
using Common.Models;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IDepartureLogic
    {
        Task<bool> StartDepartureAsync(DepartureObj departureObj);
        Task<bool> FinishDapertureAsync(DepartureObj departureObj);
    }
}