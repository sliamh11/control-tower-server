using System.Collections.Generic;

namespace Common.Models
{
    public class StationsPathModel
    {
        public Queue<int> Path { get; set; } // Indexs of stations
        public StationModel CurrentStation { get; set; }

        public StationsPathModel(Queue<int> stationsPath)
        {
            Path = stationsPath;
        }
    }
}
