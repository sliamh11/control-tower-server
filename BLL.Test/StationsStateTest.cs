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
    }
}
