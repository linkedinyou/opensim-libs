using OpenSim.Region.ScriptEngine.Common;
using System.Collections.Generic;

namespace SecondLife
{
    public class Script : OpenSim.Region.ScriptEngine.Common
    {
        public void default_event_touch_start(LSL_Types.LSLInteger num_detected)
        {
            LSL_Types.LSLInteger x = 1;
            LSL_Types.LSLInteger y = 0;
            for (x = 10; x >= 0; x--)
            {
                llOwnerSay("Launch in T minus " + x);
                IncreaseRocketPower();
            }
            for (x = 0, y = 6; y > 0 && x != y; x++, y--)
                llOwnerSay("Hi " + x + ", " + y);
            for (x = 0, y = 6; !y; x++, y--)
                llOwnerSay("Hi " + x + ", " + y);
        }
    }
}
