using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL.Data_Objects;
using BLL.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Test
{
    [TestClass]
    public class StationsLogicTest
    {
        private readonly StationsLogic _stationsLogic;
        private readonly StationsState _state;

        public StationsLogicTest()
        {
            _state = new StationsState();
            _stationsLogic = new StationsLogic(_state);
        }

        [TestMethod]
        public async Task MoveLandingToNextStationTest()
        {
            // Arrange
            var landObj = new LandingObj(new LandingLogic(_state),_stationsLogic,"MoveToNextStationTest");
            var result = await landObj.Start();
            Assert.IsTrue(result);

            // Act
            result = _stationsLogic.MoveToNextStation(landObj);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task MoveDepartureToNextStationTest()
        {
            // Arrange
            var depObj = new DepartureObj(new DepartureLogic(_state), _stationsLogic,"MoveToNextStationTest");
            var result = await depObj.Start();
            Assert.IsTrue(result);

            // Act
            result = _stationsLogic.MoveToNextStation(depObj);

            // Assert
            Assert.IsTrue(result);
        }

    }
}
