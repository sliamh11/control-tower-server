using Common.Enums;
using Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Test
{
    [TestClass]
    public class StationsStateTest
    {
        private readonly StationsState _state;
        public StationsStateTest()
        {
            _state = new StationsState();
        }

        [TestMethod]
        public void AddStationTest()
        {
            // Test 1: Empty list.
            var newStation = new List<StationModel>();
            bool result;
            try
            {
                _state.AddStation(newStation); // Throws exception
                result = false; // If reached here - something's wrong.
            }
            catch (Exception ex)
            {
                result = ex is ArgumentException;
            }
            Assert.IsTrue(result);

            // Test 2: A list with an invalid station
            newStation.Add(new StationModel(-5, new TimeSpan(0, 0, 30), StationType.Normal));
            result = _state.AddStation(newStation);
            Assert.IsFalse(result);

            // Test 3: A list with valid & invalid stations
            newStation.Add(new StationModel(2, new TimeSpan(0, 0, 30)));
            result = _state.AddStation(newStation);
            Assert.IsFalse(result);

            // Test 4: A list with valid stations.
            newStation = new List<StationModel>()
            {
                new StationModel(5,new TimeSpan(0,0,30)),
                new StationModel(2,new TimeSpan(0,0,30),StationType.Landing)
            };
            result = _state.AddStation(newStation);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsStationEmptyTest()
        {
            // Test 1: test for exception (with new random station)
            // Test 2: test a station WITH a flight (returns false)
            // Test 3: test a station WITHOUT a flight
        }

        [TestMethod]
        public void FindFastestPathByIndexTest()
        {
            // Test 1: Find the shortest path available from 0 to 7 (returns StationsModelPath)
            // Test 2: Find the fastest path from 5 to 1 (returns null)
        }

        [TestMethod]
        public void FindFastestPathByStationsTest()
        {
            // Test 1: Find the shortest path available from first station to last station (returns StationsModelPath)
            // Test 2: Find the fastest path from station 5 to first station (returns null)
        }

        [TestMethod]
        public void MoveToStationTest()
        {
            // Test 1: When toStation is null (throws StationNotFoundException)
            // Test 2: When toStation is a random station
            // Test 3: When fromStation == null (means starting a proccess)
            // Test 4: When fromStation is a random station 
            // Test 5: When movement is enabled
            // Test 6: When movement is denied
        }

        [TestMethod]
        public void GetPathEdgeStationsTest()
        {
            // Test 1: When LandingFlight gets 2 points
            // Test 2: When there's no room for a flight (returns null)
            // Test 3: When DepartureFlight gets 2 points
            // Test 4: When there's no available points for a departure flight.
        }

        [TestMethod]
        public void RemoveFlightTest()
        {
            // Test 1: Remove with a random station (throws StationNotFoundException)
            // Test 2: Remove a flight from an existing station
        }

        [TestMethod]
        public void CanAddFlight()
        {
            // Test 1: When theres a free spot for starting a landing proccess
            // Test 2: When theres no spot for a new landing proccess
            // Test 3: free spot for departure proccess
            // Test 4: no spot for departure proccess
        }
    }
}
