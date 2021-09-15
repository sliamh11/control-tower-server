using System;

namespace Common.Exceptions
{
    public class StationNotFoundException : Exception
    {
        private const string _message = "Station not found.";

        public StationNotFoundException(string message = _message) : base(message)
        {
        }
    }
}
