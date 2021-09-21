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
        private readonly IStationsManager _manager;
        public TowerController(IStationsManager manager)
        {
            _manager = manager;
        }

        public IActionResult Index()
        {
            return Ok("In tower controller");
        }

        [Route("start-landing/{flightId}")]
        public IActionResult StartLanding(string flightId)
        {
            if (_manager.StartLanding(flightId))
                return Ok($"Flight: {flightId} Started landing proccess.");

            return Ok($"Landing failed.");
        }
    }
}
