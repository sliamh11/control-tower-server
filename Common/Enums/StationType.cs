using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enums
{
    // Represents the specific type of the station - can I start a landing / departure proccess from it? Is it a combined station?
    public enum StationType
    {
        Normal,
        Combined,
        Landing,
        Departure,
        Runway
    }
}
