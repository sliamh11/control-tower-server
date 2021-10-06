using Common.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlTowerHub
{
    public class TowerHub : Hub<ITowerHub>
    {
        public async Task StateUpdated(IReadOnlyList<IReadOnlyDictionary<string, StationModel>> stationsState)
        {
            await Clients.All.StateUpdated(stationsState);
        }
    }
}
