using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class StationsPathModel
    {
        public LinkedList<StationModel> Path { get; set; }
        public StationModel CurrentStation { get; set; }
        public TimeSpan OverallTime { get; set; }

        public StationsPathModel(LinkedList<StationModel> stationsPath, TimeSpan time)
        {
            Path = stationsPath;
            CurrentStation = Path.First.Value;
            OverallTime = time;
        }
    }
}
