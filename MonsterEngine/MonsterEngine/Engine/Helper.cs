using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonsterEngine.Engine
{
    class Helper
    {
        public Helper()
        {

        }
        public float degToRad(float degree)
        {
            return degree * (float)(Math.PI / 180.0f);
        }
    }
}
