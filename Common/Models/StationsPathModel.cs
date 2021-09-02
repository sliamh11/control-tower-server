using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class StationsPathModel
    {
        public IReadOnlyList<StationModel> Path { get; set; }
        public StationModel CurrentStation { get; set; }
        public TimeSpan OverallTime { get; set; }

        public StationsPathModel(IReadOnlyList<StationModel> stationsPath, TimeSpan time)
        {
            Path = stationsPath;
            CurrentStation = Path[0];
            OverallTime = time;
        }
    }
}
