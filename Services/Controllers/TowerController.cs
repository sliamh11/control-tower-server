using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Simulator;
using System;
using System.Threading.Tasks;

namespace Services.Controllers
{
    [Route("api/[controller]")]
    public class TowerController : Controller
    {
        private readonly ITowerManager _manager;
        private readonly SimulatorManager _simulator;

        public TowerController(ITowerManager manager)
        {
            _simulator = SimulatorManager.Instance;
            _manager = manager;
        }

        [Route("")]
        public IActionResult Index() => Ok("In tower controller");

        [HttpPost("start-departure")]
        public async Task<IActionResult> StartDeparture([FromBody] string flightId)
        {
            try
            {
                if (await _manager.StartDepartureAsync(flightId))
                    return Ok($"Flight: {flightId} Started departure proccess.");

                return Ok("Departure entered to queue.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception: {ex.Message}");
            }
        }

        [HttpPost("start-landing")]
        public async Task<IActionResult> StartLanding([FromBody] string flightId)
        {
            try
            {
                if (await _manager.StartLandingAsync(flightId))
                    return Ok($"Flight: {flightId} Started landing proccess.");

                return BadRequest($"Landing entered to queue.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception: {ex.Message}");
            }
        }
    }
}
