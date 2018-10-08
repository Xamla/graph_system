using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Graph.MethodModule
{
    public interface IPinInfo
    {
        // this is the type of the parameter from the invoked method
        Type ParameterType { get; set; }

        // this is the type wich will be exposed in the graph
        Type PinType { get; set; }

        // Id of the pin
        string Name { get; set; }

        string Description { get; set; }
    }
}
