using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Controllers
{
    [Route("api/[controller]")]
    public class DepartureController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
