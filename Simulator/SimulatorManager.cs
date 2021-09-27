namespace Simulator
{
    public class SimulatorManager
    {
        private static FlightSimulator _landingSimulator;
        private static FlightSimulator _departureSimulator;
        private static SimulatorManager _instance;
        private const string _landingUrl = "http://localhost:61270/api/tower/start-landing";
        private const string _departureUrl = "http://localhost:61270/api/tower/start-departure";

        public static SimulatorManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SimulatorManager();

                return _instance;
            }
        }

        private SimulatorManager()
        {
            _landingSimulator = new FlightSimulator(_landingUrl);
            _departureSimulator = new FlightSimulator(_departureUrl);
        }

    }
}
