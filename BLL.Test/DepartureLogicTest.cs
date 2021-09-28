using BLL.Data_Objects;
using BLL.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Test
{
    [TestClass]
    public class DepartureLogicTest
    {
        private readonly DepartureLogic _logic;

        public DepartureLogicTest()
        {
            _logic = new DepartureLogic(new StationsState());
        }

        [TestMethod]
        public void StartDepartureTest()
        {
            // Arrange
            var depObj = new DepartureObj("StartDepartureTest");

            // Act
            var result = _logic.StartDeparture(depObj);

            // Assert
            Assert.IsTrue(result);
        }

        // When flight is NOT at the last station
        [TestMethod]
        public void CantFinishDepartureTest()
        {
            // Arrange
            var depObj = new DepartureObj("CantFinishDepartureTest");
            var result = _logic.StartDeparture(depObj);
            Assert.IsTrue(result);

            // Act - The flight just started it's path, therefor cant finish and returns false
            result = _logic.FinishDaperture(depObj);

            // Assert
            Assert.IsFalse(result);
        }

        // When flight is AT the last station
        [TestMethod]
        public void CanFinishDepartureTest()
        {
            //Arrange - Remove all stations but the last one
            var depObj = new DepartureObj("CanFinishDepartureTest");
            var result = _logic.StartDeparture(depObj);
            Assert.IsTrue(result);

            var currStation = depObj.StationsPath.Path.First;
            while (currStation != null && currStation.Value != depObj.StationsPath.Path.Last.Value)
            {
                var nextStation = currStation.Next;
                depObj.StationsPath.Path.Remove(currStation);
                currStation = nextStation;
            }
            depObj.StationsPath.CurrentStation = currStation.Value;

            // Act
            result = _logic.FinishDaperture(depObj);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
