using BLL.Data_Objects;
using BLL.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

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

        // When flight is not at the last station
        [TestMethod]
        public void CantFinishLandingTest()
        {
            // Arrange
            var landObj = new LandingObj("CantFinishDepartureTest");

            var result = _logic.StartLanding(landObj);
            Assert.IsTrue(result);

            // Act - The flight just started it's path, therefor cant finish and returns false
            result = _logic.FinishLanding(landObj);

            // Assert
            Assert.IsFalse(result);
        }

        // When flight is at the last station
        [TestMethod]
        public void CanFinishLandingTest()
        {
            //Arrange - Remove all stations but the last one
            var landObj = new LandingObj("CanFinishDepartureTest");
            var result = _logic.StartLanding(landObj);
            Assert.IsTrue(result);

            var currStation = landObj.StationsPath.Path.First;
            
            while (currStation != null && currStation.Value != landObj.StationsPath.Path.Last.Value)
            {
                var nextStation = currStation.Next;
                landObj.StationsPath.Path.Remove(currStation);
                currStation = nextStation;
            }
            landObj.StationsPath.CurrentStation = currStation.Value;

            // Act
            result = _logic.FinishLanding(landObj);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
