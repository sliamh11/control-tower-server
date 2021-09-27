using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task MoveToNextStationTest()
        {
            // Arrange
            var landObj = new LandingObj("MoveToNextStationTest");
            var result = await landObj.Start();
            Assert.IsTrue(result);

            // Act
            // Activate the function
            result = await _logic.MoveToNextStationAsync(landObj);

            // Assert
            Assert.IsTrue(result);
        }

    }
}
