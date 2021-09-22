using BLL.Data_Objects;
using Common.Models;

namespace BLL.Interfaces
{
    public interface IDepartureLogic : IStationsLogic
    {
        bool StartDeparture(DepartureObj departureObj);
        bool FinishDaperture(DepartureObj departureObj);
    }
}