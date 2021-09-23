using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Controllers
{
    [Route("api/[controller]")]
    public class TowerController : Controller
    {
        private readonly ITowerManager _manager;
        public TowerController(ITowerManager manager)
        {
            _manager = manager;
        }

        public IActionResult Index() => Ok("In tower controller");

        [Route("start-departure/{flightId}")]
        public IActionResult StartDeparture(string flightId)
        {
            try
            {
                if (_manager.StartDeparture(flightId))
                    return Ok($"Flight: {flightId} Started departure proccess.");

                return Ok("Departure entered to queue.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception: {ex.Message}");
            }
        }

        [Route("start-landing/{flightId}")]
        public IActionResult StartLanding(string flightId)
        {
            try
            {
                if (_manager.StartLanding(flightId))
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
