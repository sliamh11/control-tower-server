using BLL.Data_Objects;
using BLL.Logic;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Test
{
    [TestClass]
    public class StationsStateTest
    {
        [TestMethod]
        public void AddStationTest()
        {
            var state = new StationsState();
            // Test 1: Empty list.
            var newStations = new Dictionary<string, StationModel>();
            bool result;
            try
            {
                state.AddStation(newStations); // Throws exception
                result = false; // If reached here - something's wrong.
            }
            catch (Exception ex)
            {
                result = ex is ArgumentException;
            }
            Assert.IsTrue(result);

            // Test 2: A dictionary with an invalid station
            var station = new StationModel(-5, new TimeSpan(0, 0, 30), StationType.Normal);
            newStations.Add(station.Id, station);
            result = state.AddStation(newStations);
            Assert.IsFalse(result);

            // Test 3: A dictionary with valid & invalid stations
            station = new StationModel(2, new TimeSpan(0, 0, 30));
            newStations.Add(station.Id, station);
            result = state.AddStation(newStations);
            Assert.IsFalse(result);

            // Test 4: A dictionary with valid stations.
            var stationA = new StationModel(5, new TimeSpan(0, 0, 30));
            var stationB = new StationModel(2, new TimeSpan(0, 0, 30), StationType.Landing);
            newStations = new Dictionary<string, StationModel>()
            {
                { stationA.Id, stationA},
                {stationB.Id,stationB }
            };
            result = state.AddStation(newStations);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsStationEmptyTest()
        {
            var state = new StationsState();

            // Test 1: test for ArgumentExcpetion
            var station = new StationModel(-2, new TimeSpan(0, 0, 5));
            bool isValid;
            try
            {
                state.IsStationEmpty(station);
                isValid = false;
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
                state.IsStationEmpty(station);
                isValid = false; // If reached here - something went wrong.
            }
            catch (Exception ex)
            {
                isValid = ex is StationNotFoundException;
            }
            Assert.IsTrue(isValid);

            // Test 3: test a station WITHOUT a flight
            var stations = state.GetStationsState();
            var firstStation = stations[0].Values.FirstOrDefault();
            isValid = state.IsStationEmpty(firstStation); // Is currently empty
            Assert.IsTrue(isValid);

            // Test 4: test a station WITH a flight (returns false)
            var landingObj = new LandingObj("IsStationEmptyTest");
            var landingLogic = new LandingLogic(state);
            try
            {
                landingLogic.StartLanding(landingObj);
                isValid = state.IsStationEmpty(firstStation); // Supposes to return false
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
            var state = new StationsState();

            // Test 1: Find the shortest path available from 0 to 7 (returns StationsModelPath)
            var path = state.FindFastestPath(0, 7);
            Assert.IsNotNull(path);

            // Test 2: Find the fastest path from 5 to 1 (returns null)
            path = state.FindFastestPath(5, 2);
            Assert.IsNull(path);
        }

        [TestMethod]
        public void FindFastestPathByStationsTest()
        {
            var state = new StationsState();

            // Test 1: Call func with false indexs (An exception will be thrown).
            var stationA = new StationModel(1, 5, new TimeSpan(0, 0, 40));
            var stationB = new StationModel(14, 20, new TimeSpan(0, 0, 45));
            bool isValid;
            try
            {
                state.FindFastestPath(stationA, stationB);
                isValid = false;
            }
            catch (Exception ex)
            {
                isValid = ex is ArgumentException;
            }
            Assert.IsTrue(isValid);

            // Test 2: Call func with valid indexs but stations doesn't exist in graph.
            stationA = new StationModel(1, 5, new TimeSpan(0, 0, 40));
            stationB = new StationModel(3, 7, new TimeSpan(0, 0, 30), StationType.LandingExit);
            try
            {
                state.FindFastestPath(stationA, stationB);
                isValid = false;
            }
            catch (Exception ex)
            {
                isValid = ex is StationNotFoundException;
            }
            Assert.IsTrue(isValid);

            // Test 3: Search for a possible path between 2 stations (returns StationsModelPath)
            var stations = state.GetStationsState();
            stationA = stations[0].Values.FirstOrDefault();
            stationB = stations[5].Values.FirstOrDefault(); // Find out why index 7 wont work
            var path = state.FindFastestPath(stationA, stationB);
            Assert.IsNotNull(path);

            // Test 4: Search for an imposssible path between 2 points (returns null)
            path = state.FindFastestPath(stationB, stationA);
            Assert.IsNull(path);
        }

        [TestMethod]
        public void MoveToStationTest()
        {
            var state = new StationsState();
            var stations = state.GetStationsState();
            var flight = new FlightModel("MoveToStationTest", FlightType.Landing);
            bool isValid;
            // Test 1: When toStation is null (throws StationNotFoundException)
            StationModel fromStation = new StationModel(1, 2, new TimeSpan(0, 0, 50), StationType.Departure);
            StationModel toStation = null;
            try
            {
                state.MoveToStation(fromStation, toStation, flight);
                isValid = false;
            }
            catch (Exception ex)
            {
                isValid = ex is ArgumentException;
            }
            Assert.IsTrue(isValid);

            // Test 2: When toStation is a random station (throws StationNotFoundException)
            toStation = new StationModel(2, 3, new TimeSpan(0, 0, 10));
            try
            {
                state.MoveToStation(fromStation, toStation, flight);
                isValid = false;
            }
            catch (Exception ex)
            {
                isValid = ex is StationNotFoundException;
            }
            Assert.IsTrue(isValid);

            // Test 3: When fromStation is a random station 
            fromStation = new StationModel(2, 4, new TimeSpan(0, 0, 30), StationType.Landing);
            try
            {
                state.MoveToStation(fromStation, toStation, flight);
                isValid = false;
            }
            catch (Exception ex)
            {
                isValid = ex is StationNotFoundException;
            }
            Assert.IsTrue(isValid);

            // Test 4: When fromStation == null & toStation is valid (means starting a proccess)
            fromStation = null;
            toStation = stations[0].Values.FirstOrDefault();
            isValid = state.MoveToStation(fromStation, toStation, flight);
            Assert.IsTrue(isValid);

            // Test 5: When movement is denied (because of Test 4)
            isValid = state.MoveToStation(fromStation, toStation, flight);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void GetPathEdgeStationsTest()
        {
            var state = new StationsState();
            var landingFlight = new FlightModel("landing", FlightType.Landing);

            // Test 1: When LandingFlight gets 2 points
            var edges = state.GetPathEdgeStations(landingFlight);
            Assert.IsNotNull(edges.StartStation);
            Assert.IsNotNull(edges.EndStation);

            // Test 2: When there's no room for a flight (returns default)
            state.MoveToStation(null, edges.StartStation, landingFlight);
            edges = state.GetPathEdgeStations(landingFlight);
            Assert.IsNull(edges.StartStation);
            Assert.IsNull(edges.EndStation);

            // Test 3: When DepartureFlight gets 2 points
            var departureFlight = new FlightModel("departure", FlightType.Departure);
            edges = state.GetPathEdgeStations(departureFlight);
            Assert.IsNotNull(edges.StartStation);
            Assert.IsNotNull(edges.EndStation);

            // Test 4: When there's no available points for a departure flight. (2 initial stations for departure)
            state.MoveToStation(null, edges.StartStation, departureFlight);
            edges = state.GetPathEdgeStations(departureFlight);
            state.MoveToStation(null, edges.StartStation, departureFlight);
            edges = state.GetPathEdgeStations(departureFlight);
            Assert.IsNull(edges.StartStation);
            Assert.IsNull(edges.EndStation);
        }

        [TestMethod]
        public void RemoveFlightTest()
        {
            var state = new StationsState();
            var flight = new FlightModel("test", FlightType.Landing);
            bool isValid;

            // Test 1: Remove with a non-existing station (throws StationNotFoundException)
            var station = new StationModel(3, 4, new TimeSpan(0, 0, 40), StationType.LandingExit);
            try
            {
                state.RemoveFlight(station);
                isValid = false;
            }
            catch (Exception ex)
            {
                isValid = ex is StationNotFoundException;
            }
            Assert.IsTrue(isValid);

            // Test 2: Remove a flight from an existing station
            var stations = state.GetStationsState();
            var toStation = stations[0].Values.FirstOrDefault();

            if (state.MoveToStation(null, toStation, flight))
                isValid = state.RemoveFlight(toStation);
            else
                isValid = false;

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void CanAddFlightTest()
        {
            var state = new StationsState();
            var landingFlight = new FlightModel("landingTest", FlightType.Landing);
            var departureFlight = new FlightModel("departureTest", FlightType.Departure);
            bool isValid;

            // Test 1: When theres a free spot for starting a landing proccess
            isValid = state.CanAddFlight(FlightType.Landing);
            Assert.IsTrue(isValid);

            // Test 2: When theres no spot for a new landing proccess
            var landingEdges = state.GetPathEdgeStations(landingFlight);
            if (state.MoveToStation(null, landingEdges.StartStation, landingFlight))
            {
                landingEdges = state.GetPathEdgeStations(landingFlight);
                isValid = landingEdges.StartStation == null;
            }
            else
            {
                isValid = false;
            }
            Assert.IsTrue(isValid);


            // Test 3: free spot for departure proccess
            isValid = state.CanAddFlight(FlightType.Departure);
            Assert.IsTrue(isValid);

            // Test 4: no spot for departure proccess
            // Filling both departure starting points
            var depEdges = state.GetPathEdgeStations(departureFlight);
            if (state.MoveToStation(null, depEdges.StartStation, departureFlight))
            {
                depEdges = state.GetPathEdgeStations(departureFlight);
                if (state.MoveToStation(null, depEdges.StartStation, departureFlight))
                {
                    depEdges = state.GetPathEdgeStations(departureFlight);
                    isValid = depEdges.StartStation == null;
                }
                else
                {
                    isValid = false;
                }
            }
            else
            {
                isValid = false;
            }
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void UpdateStationTest()
        {
            var state = new StationsState();
            var stations = state.GetStationsState();
            bool isValid;

            // Test 1: Update a non existing station
            var station = new StationModel(2, 3, new TimeSpan(0, 0, 5), StationType.Departure, StationType.Landing);
            try
            {
                isValid = state.UpdateStation(station);
            }
            catch (Exception ex)
            {
                isValid = ex is StationNotFoundException;
            }
            Assert.IsFalse(isValid);

            // Test 2: Update an existing station
            station = stations[0].Values.FirstOrDefault();
            station.StandbyPeriod = new TimeSpan(0, 0, 4);
            isValid = state.UpdateStation(station);
            Assert.IsTrue(isValid);
        }
    }
}
