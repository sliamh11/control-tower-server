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
    public class LandingLogicTest
    {
        private readonly LandingLogic _logic;

        public LandingLogicTest()
        {
            _logic = new LandingLogic();
        }
        
        [TestMethod]
        public void StartLandingTest()
        {
            // Arrange
            var depObj = new LandingObj("StartDepartureTest");

            // Act
            var result = _logic.StartLanding(depObj);

            // Assert
            Assert.IsTrue(result);
        }

        // When flight is not at the last station:
        [TestMethod]
        public void CantFinishLandingTest()
        {
            // Arrange
            var landObj = new LandingObj("CantFinishDepartureTest");
            _logic.StartLanding(landObj);

            // Act - The flight just started it's path, therefor cant finish and returns false
            var result = _logic.FinishLanding(landObj);

            // Assert
            Assert.IsFalse(result);
        }

        // When flight is at the last station:
        [TestMethod]
        public void CanFinishLandingTest()
        {
            //Arrange - Remove all stations but the last one
            var landObj = new LandingObj("CanFinishDepartureTest");
            _logic.StartLanding(landObj);
            var currStation = landObj.StationsPath.Path.First;
            while (currStation != null && currStation.Value != landObj.StationsPath.Path.Last.Value)
            {
                var nextStation = currStation.Next;
                landObj.StationsPath.Path.Remove(currStation);
                currStation = nextStation;
            }
            landObj.StationsPath.CurrentStation = currStation.Value;

            // Act
            var result = _logic.FinishLanding(landObj);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
