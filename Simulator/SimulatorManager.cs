using Simulator.Interfaces;

namespace Simulator
{
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
