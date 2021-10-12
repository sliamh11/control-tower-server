using Simulator.Interfaces;

namespace Simulator
{
    /// <summary>
    /// This class is managing the project's simulator - it sends new flights every 5-10 seconds, through an API request.
    /// </summary>
    public class SimulatorManager : ISimulatorManager
    {
        private static FlightSimulator _landingSimulator;
        private static FlightSimulator _departureSimulator;
        private const string _landingUrl = "http://localhost:61270/api/tower/start-landing";
        private const string _departureUrl = "http://localhost:61270/api/tower/start-departure";

        public SimulatorManager() 
        {
            _landingSimulator = new FlightSimulator(_landingUrl);
            _departureSimulator = new FlightSimulator(_departureUrl);
        }
    }
}
