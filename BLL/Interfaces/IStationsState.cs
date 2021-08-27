using Common.Data_Structures;

namespace BLL.Interfaces
{
    public interface IStationsState
    {
        StationsGraph Stations { get; set; }
        StationsGraph GetStationsState();
        void LoadStations();
    }
}
