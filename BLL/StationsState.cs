﻿using BLL.Interfaces;
using Common.Data_Structures;
using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    // Holds the Data Structure.
    public class StationsState : IStationsState
    {
        private readonly StationsGraph _stations;
        private readonly object _stationsLock = new object();

        public StationsState()
        {
            _stations = new StationsGraph();
        }


        public void LoadStations()
        {
            // Waiting stations
            AddStation(new LinkedList<StationModel>(new StationModel[] { new StationModel(0, 1, new TimeSpan(0, 1, 0), StationType.Landing) }));
            AddStation(new LinkedList<StationModel>(new StationModel[] { new StationModel(1, 2, new TimeSpan(0, 0, 45)) }));
            AddStation(new LinkedList<StationModel>(new StationModel[] { new StationModel(2, 3, new TimeSpan(0, 0, 40)) }));
            // Runway
            AddStation(new LinkedList<StationModel>(new StationModel[] { new StationModel(3, 4, new TimeSpan(0, 1, 30), StationType.Runway) }));
            // Airport & Depratures
            AddStation(new LinkedList<StationModel>(new StationModel[] {
                new StationModel(4, 5, new TimeSpan(0, 0, 45)),
                new StationModel(4, 6, new TimeSpan(0, 0, 45)) }));
            AddStation(new LinkedList<StationModel>(new StationModel[] { new StationModel(5, 7, new TimeSpan(0, 1, 0), StationType.Combined) }));// Also has an exit
            AddStation(new LinkedList<StationModel>(new StationModel[] { new StationModel(6, 7, new TimeSpan(0, 1, 0), StationType.Combined) }));// Also has an exit
            AddStation(new LinkedList<StationModel>(new StationModel[] { new StationModel(7, 3, new TimeSpan(0, 0, 35)) })); // Station before runway.
        }

        public void StateUpdated()
        {
            // Called each time something updated the _stations field.
            // Emits the updated state to all subscribers (client + DB).

            // Maybe somehow updated it in 'set' if its even possible?
            throw new NotImplementedException();
        }

        public IReadOnlyList<IReadOnlyList<StationModel>> GetStationsState()
        {
            lock (_stationsLock)
            {
                return _stations.GetStationsState();
            }
        }

        public void AddStation(List<StationModel> newStation)
        {
            lock (_stationsLock)
            {
                _stations.AddStation(newStation);
            }
        }

        //public bool UpdateStation(int index, object updatedStation)
        //{
        //    lock (_stationsLock)
        //    {

        //    }
        //}
    }
}