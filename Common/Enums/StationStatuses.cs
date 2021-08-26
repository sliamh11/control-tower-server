using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enums
{
    public enum StationStatuses
    {  
        // Open
        Open = 0,

        // Generally closed
        Closed = 10,

        // Closed + Reason.
        Emergancy_Landing = 11,
        Emergancy_Fire_Outbreak = 12
    }
}
