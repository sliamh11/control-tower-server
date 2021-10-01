using Common.Models;

namespace Common.Structs
{
    public struct PathEdgesStruct
    {
        public StationModel StartStation { get; set; }
        public StationModel EndStation { get; set; }

        public PathEdgesStruct(StationModel start, StationModel end)
        {
            StartStation = start;
            EndStation = end;
        }
    }
}
