using System;

namespace Common.Models
{
    /// <summary>
    /// This model's purpose is to present data in the database.
    /// </summary>
    public class TrafficHistory
    {
        public int Id { get; set; }
        public int StationNumber { get; set; }
        public string FlightId { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
    }
}
