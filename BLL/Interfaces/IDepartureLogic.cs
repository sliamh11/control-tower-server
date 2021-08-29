using BLL.Data_Objects;
using Common.Models;

namespace BLL.Interfaces
{
    internal interface IDepartureLogic : IStationsLogic
    {
        StationsPathModel StartDeparture(DepartureObj departureObj);
        bool FinishDaperture(DepartureObj departureObj);
    }
}