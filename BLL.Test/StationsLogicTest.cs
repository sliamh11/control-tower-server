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
        private readonly StationsLogic _logic;

        public StationsLogicTest()
        {
            _logic = new StationsLogic();
        }

        [TestMethod]
        public async Task MoveLandingToNextStationTest()
        {
            // Arrange
            var landObj = new LandingObj("MoveToNextStationTest");
            var result = await landObj.Start();
            Assert.IsTrue(result);

            // Act
            // Activate the function
            result = _logic.MoveToNextStation(landObj);

            // Assert
            Thread.Sleep(1);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task MoveDepartureToNextStationTest()
        {
            // Arrange
            var depObj = new DepartureObj("MoveToNextStationTest");
            var result = await depObj.Start();
            Assert.IsTrue(result);

            // Act
            // Activate the function
            result = _logic.MoveToNextStation(depObj);

            // Assert
            Assert.IsTrue(result);
        }

    }
}
