using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Timers;

namespace Simulator
{
    internal class FlightSimulator
    {
        #region Private Fields
        private readonly Timer _timer;
        private readonly Random _rand;
        private readonly HttpClient _client;
        private readonly string _url;
        private int _minTime;
        private int _maxTime;
        #endregion
        #region Public Properties
        public int Interval { get; set; }
        #endregion

        public FlightSimulator(string url)
        {
            _url = url;
            _minTime = 10;
            _maxTime = 60;
            _client = new HttpClient();
            _rand = new Random();
            Interval = _rand.Next(_minTime, _maxTime) * 1000;
            _timer = new Timer(Interval);
            _timer.Elapsed += (s, e) => OnTimerElapsed();
            _timer.Start();
        }

        private async void OnTimerElapsed()
        {
            var flightId = Guid.NewGuid().ToString();
            var flightIdJson = JsonConvert.SerializeObject(flightId);
            var body = new StringContent(flightIdJson, Encoding.UTF8, "application/json");
            await _client.PostAsync(_url, body);
            Interval = _rand.Next(_minTime, _maxTime) * 1000;
            _timer.Interval = Interval;
        }
    }
}
