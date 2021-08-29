using Common.Models;
using System;

namespace BLL.Interfaces
{
    internal interface IStationsLogic
    {
        bool MoveToNextStation(IDataObj dataObj);
        DateTime CalcEndTime(StationsPathModel path);
    }
}
