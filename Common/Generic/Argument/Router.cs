using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Maintains different argument routes and uses <see cref="MyCommandLine"/> 
        /// to parse the argument and select the correct route.
        /// </summary>
        public class Router
        {
            private readonly Action<string> notification;
            private readonly Dictionary<string, Action<MyCommandLine>> routes;

            private readonly MyCommandLine _cl = new MyCommandLine();

            /// <summary>
            /// New Router Instance.
            /// </summary>
            /// <param name="notification">Action to use to notify the user about argument errors.</param>
            /// <param name="routes">Routes to use.</param>
            public Router(Action<string> notification, Dictionary<string, Action<MyCommandLine>> routes)
            {
                this.notification = notification;
                this.routes = routes;
            }

            /// <summary>
            /// Parse the provided argument and find the corresponding route.
            /// </summary>
            /// <param name="argument">The argument to parse</param>
            /// <returns>If the arguement was succesfully parsed, and a route was found, then true, otherwise false.</returns>
            public bool ParseAndRoute(string argument)
            {
                Debugger.Debug("Arg: " + argument);
                if (string.IsNullOrEmpty(argument))
                {
                    notification("No Argument Provided!");
                    return false;
                }
                else if (_cl.TryParse(argument))
                {
                    routes.GetValueOrDefault(_cl.Argument(0), p => notification("Argument not found: " + argument))
                        .Invoke(_cl);
                    return true;
                }
                else
                {
                    notification("Argument can't be parse: " + argument);
                    return false;
                }
            }
        }
    }
}
