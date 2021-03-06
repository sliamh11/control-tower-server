using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Simulator.Interfaces;
using System;
using System.Threading.Tasks;

namespace Services.Controllers
{
    [Route("api/[controller]")]
    public class TowerController : Controller
    {
        #region Private Fields
        private readonly ITowerManager _manager;
        private readonly ISimulatorManager _simulator;
        #endregion

        public TowerController(ITowerManager manager, ISimulatorManager simulator)
        {
            _manager = manager;
            _simulator = simulator;
        }

        #region Controllers
        [Route("")]
        public IActionResult Index() => Ok("In tower controller");

        [HttpGet("state")]
        public JsonResult GetState()
        {
            return Json(_manager.GetStationsState());
        }

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
        #endregion

        // AddStation - gets a list of objects from body as JSON and is translated to a StationModels, and then to a dictionary
    }
}
