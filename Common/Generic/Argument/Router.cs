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
        public class Router
        {
            private readonly Action<string> notification;
            private readonly Dictionary<string, Action<MyCommandLine>> routes;

            private readonly MyCommandLine cl = new MyCommandLine();

            public Router(Action<string> notification, Dictionary<string, Action<MyCommandLine>> routes)
            {
                this.notification = notification;
                this.routes = routes;
            }

            public void ParseAndRoute(string argument)
            {
                if (string.IsNullOrEmpty(argument))
                {
                    notification("No Argument Provided!");
                }
                else if (cl.TryParse(argument))
                {
                    routes.GetValueOrDefault(cl.Argument(0), p => notification("Argument not found: " + argument))
                        .Invoke(cl);
                }
                else
                {
                    notification("Argument can't be parse: " + argument);
                }
            }
        }
    }
}
