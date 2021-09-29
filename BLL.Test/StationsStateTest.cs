using BLL.Data_Objects;
using BLL.Logic;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            var newStations = new Dictionary<string, StationModel>();
            bool result;
            try
            {
                _state.AddStation(newStations); // Throws exception
                result = false; // If reached here - something's wrong.
            }
            catch (Exception ex)
            {
                result = ex is ArgumentException;
            }
            Assert.IsTrue(result);

            // Test 2: A dictionary with an invalid station
            var station = new StationModel(-5, new TimeSpan(0, 0, 30), StationType.Normal);
            newStations.Add(station.StationId, station);
            result = _state.AddStation(newStations);
            Assert.IsFalse(result);

            // Test 3: A dictionary with valid & invalid stations
            station = new StationModel(2, new TimeSpan(0, 0, 30));
            newStations.Add(station.StationId, station);
            result = _state.AddStation(newStations);
            Assert.IsFalse(result);

            // Test 4: A dictionary with valid stations.
            var stationA = new StationModel(5, new TimeSpan(0, 0, 30));
            var stationB = new StationModel(2, new TimeSpan(0, 0, 30), StationType.Landing);
            newStations = new Dictionary<string, StationModel>()
            {
                { stationA.StationId, stationA},
                {stationB.StationId,stationB }
            };
            result = _state.AddStation(newStations);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsStationEmptyTest()
        {
            // Test 1: test for ArgumentExcpetion
            var station = new StationModel(-2, new TimeSpan(0, 0, 5));
            bool isValid;
            try
            {
                _state.IsStationEmpty(station);
                isValid = false; // If reached here - something went wrong.
            }
            catch (Exception ex)
            {
                isValid = ex is ArgumentException;
            }
            Assert.IsTrue(isValid);

            // Test 2: test for StationNotFoundExcpetion
            station = new StationModel(4, 6, new TimeSpan(0, 0, 15));
            try
            {
                _state.IsStationEmpty(station);
                isValid = false; // If reached here - something went wrong.
            }
            catch (Exception ex)
            {
                isValid = ex is StationNotFoundException;
            }
            Assert.IsTrue(isValid);

            // Test 3: test a station WITHOUT a flight
            var stations = _state.GetStationsState();
            var firstStation = stations[0].Values.FirstOrDefault();
            isValid = _state.IsStationEmpty(firstStation); // Is currently empty
            Assert.IsTrue(isValid);

            // Test 4: test a station WITH a flight (returns false)
            var landingObj = new LandingObj("IsStationEmptyTest");
            var landingLogic = new LandingLogic(_state);
            try
            {
                landingLogic.StartLanding(landingObj);
                isValid = _state.IsStationEmpty(firstStation); // Supposes to return false
            }
            catch (Exception)
            {
                isValid = true;
            }
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void FindFastestPathByIndexTest()
        {
            // Test 1: Find the shortest path available from 0 to 7 (returns StationsModelPath)
            var path = _state.FindFastestPath(0, 7);
            Assert.IsNotNull(path);

            // Test 2: Find the fastest path from 5 to 1 (returns null)
            path = _state.FindFastestPath(5, 2);
            Assert.IsNull(path);
        }

        [TestMethod]
        public void FindFastestPathByStationsTest()
        {
            var state = new StationsState();
            // Test 1: Call func with false indexs (An exception will be thrown).
            var stationA = new StationModel(1, 5, new TimeSpan(0, 0, 40));
            var stationB = new StationModel(14, 20, new TimeSpan(0, 0, 45));
            //bool isValid;
            //try
            //{
            //    state.FindFastestPath(stationA, stationB);
            //    isValid = false;
            //}
            //catch (Exception ex)
            //{
            //    isValid = ex is ArgumentException;
            //}
            //Assert.IsTrue(isValid);

            //// Test 2: Call func with valid indexs but stations doesn't exist in graph.
            //stationA = new StationModel(1, 5, new TimeSpan(0, 0, 40));
            //stationB = new StationModel(3, 7, new TimeSpan(0, 0, 30), StationType.LandingExit);
            //try
            //{
            //    state.FindFastestPath(stationA, stationB);
            //    isValid = false;
            //}
            //catch (Exception ex)
            //{
            //    isValid = ex is StationNotFoundException;
            //}
            //Assert.IsTrue(isValid);

            // Test 3: Search for a possible path between 2 stations (returns StationsModelPath)
            state = new StationsState();
            var stations = state.GetStationsState();
            stationA = stations[0].Values.FirstOrDefault();
            stationB = stations[7].Values.FirstOrDefault();
            var path = state.FindFastestPath(stationA, stationB);
            Assert.IsNotNull(path);

            // Test 4: Search for an imposssible path between 2 points (returns null)
            //path = _state.FindFastestPath(stationB, stationA);
            //Assert.IsNull(path);
        }

        //[TestMethod]
        public void MoveToStationTest()
        {
            // Test 1: When toStation is null (throws StationNotFoundException)
            // Test 2: When toStation is a random station
            // Test 3: When fromStation == null (means starting a proccess)
            // Test 4: When fromStation is a random station 
            // Test 5: When movement is enabled
            // Test 6: When movement is denied
        }

        //[TestMethod]
        public void GetPathEdgeStationsTest()
        {
            // Test 1: When LandingFlight gets 2 points
            // Test 2: When there's no room for a flight (returns null)
            // Test 3: When DepartureFlight gets 2 points
            // Test 4: When there's no available points for a departure flight.
        }

        //[TestMethod]
        public void RemoveFlightTest()
        {
            // Test 1: Remove with a random station (throws StationNotFoundException)
            // Test 2: Remove a flight from an existing station
        }

        //[TestMethod]
        public void CanAddFlight()
        {
            // Test 1: When theres a free spot for starting a landing proccess
            // Test 2: When theres no spot for a new landing proccess
            // Test 3: free spot for departure proccess
            // Test 4: no spot for departure proccess
        }
    }
}
