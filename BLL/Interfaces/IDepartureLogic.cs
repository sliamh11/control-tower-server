using BLL.Data_Objects;
using Common.Models;

namespace BLL.Interfaces
{
    public interface IDepartureLogic : IStationsLogic
    {
        StationsPathModel StartDeparture(DepartureObj departureObj);
        bool FinishDaperture(DepartureObj departureObj);
    }
}