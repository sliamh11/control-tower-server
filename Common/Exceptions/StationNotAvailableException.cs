using System;

namespace Common.Exceptions
{
    public class StationNotAvailableException : Exception
    {
        private const string _message = "Station not available at the moment, please try again in a few minutes.";

        public StationNotAvailableException(string message = _message) : base(message)
        {
        }
    }
}
