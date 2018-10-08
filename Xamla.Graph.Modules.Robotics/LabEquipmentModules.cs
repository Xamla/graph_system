using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uml.Robotics.Ros;
using Xamla.Graph.MethodModule;
using Xamla.Robotics.Motion;

namespace Xamla.Graph.Modules.Robotics
{
    using papouch_papago_driver = Messages.papouch_papago_driver;

    public static partial class StaticModules
    {
        const string DEFAULT_PAPAGO_MEASURE_NAME = "/papago/channel_a";

        /// <summary>
        /// Read the value of one channel of a Papouch Papago ethernet thermo-/hygrometer.
        /// </summary>
        /// <param name="serviceName">The name of the channel reading service of the Papago driver node.</param>
        /// <returns>
        /// <return name="temperature"></return>
        /// <return name="humidity"></return>
        /// </returns>
        [StaticModule(ModuleType = "Xamla.LabEquipment.Papago.Measure", Flow = true)]
        public static async Task<Tuple<double, double>> Measure(
           [InputPin(PropertyMode = PropertyMode.Default, DefaultValue = DEFAULT_PAPAGO_MEASURE_NAME)] string serviceName)
        {
            using (var client = rosClient.GlobalNodeHandle.ServiceClient<papouch_papago_driver.PapagoChannel>(serviceName))
            {
                var srv = new papouch_papago_driver.PapagoChannel();
                if (!await client.CallAsync(srv))
                    throw new ServiceCallFailedException(serviceName);

                return Tuple.Create(srv.resp.temperature, srv.resp.humidity);
            }
        }
    }
}
