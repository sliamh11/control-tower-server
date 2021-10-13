using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class TowerRepository : ITowerRepository
    {
        private readonly TowerContext _context;
        public TowerRepository(TowerContext context)
        {
            _context = context;
        }

        public void SaveMovementHistory(StationModel fromStation, StationModel toStation)
        {
            UpdateStation(fromStation);
            UpdateStation(toStation);
            UpdateHistory(fromStation, toStation);
            _context.SaveChanges();
        }

        public void SaveNewFlight(FlightModel flight)
        {
            if (flight == null)
                return;

            _context.Flights.Add(flight);
            _context.SaveChanges();
        }

        public void SaveNewStation(StationModel station)
        {
            if (station == null)
                return;

            _context.Stations.Add(station);
            _context.SaveChanges();
        }

        private void UpdateStation(StationModel updatedStation)
        {
            if (updatedStation != null)
            {
                var station = _context.Stations.SingleOrDefault(s => s.Id == updatedStation.Id);
                if (station != null)
                {
                    station = updatedStation;
                    _context.Stations.Update(station);
                }
            }
        }
        private void UpdateHistory(StationModel from, StationModel to)
        {
            var fromStation = _context.History.SingleOrDefault(s =>
            s.StationNumber == from.Number
            && from.CurrentFlight.Id == s.FlightId
            && s.ExitTime == null);

            fromStation.ExitTime = DateTime.Now;
            _context.Update(fromStation);

            TrafficHistory newTraffic = new TrafficHistory() 
            {
                StationNumber = to.Number,
                FlightId = to.CurrentFlight.Id,
                EntryTime = DateTime.Now
            };

            _context.History.Add(newTraffic);
        }

        public IEnumerable<StationModel> GetStations()
        {
            return _context.Stations.Include(s => s.CurrentFlight);
        }
    }
}
