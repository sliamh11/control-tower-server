using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Controllers
{
    [Route("api/[controller]")]
    public class LandingController : Controller
    {
        private ILandingLogic _logic;
        public LandingController(ILandingLogic logic)
        {
            _logic = logic;
        }

        public IActionResult Index()
        {
            return Ok("landing controller test");
        }
    }
}
