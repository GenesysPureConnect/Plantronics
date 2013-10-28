using Interop.Plantronics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantronicsClientAddIn.Plantronics
{
    internal interface IPlantronicsManager
    {
        IDevice GetDevice();
    }
}
