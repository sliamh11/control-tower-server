using Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlTowerHub
{
    public interface ITowerHub
    {
        Task StateUpdated(IReadOnlyList<IReadOnlyDictionary<string, StationModel>> stationsState);
    }
}