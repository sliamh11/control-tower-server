using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    internal interface IDataObj
    {
        public FlightModel Flight { get; set; }
        public StationsPathModel StationsPath { get; set; }
    }
}
