using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;

namespace AntiAFK
{
    public class Configuration : IRocketPluginConfiguration
    {
        public int SecondsUntilKick;

        public void LoadDefaults()
        {
            SecondsUntilKick = 300;
        }
    }
}
