using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace MonsterEngine.Engine
{
    class Helper
    {
        static public float degToRad(float degree)
        {
            return degree * (float)(Math.PI / 180.0f);
        }
        static public float Vec3Lenght(Vector3 vec3)
        {
            return (float)Math.Sqrt(Math.Pow(vec3.X, 2) + Math.Pow(vec3.Y, 2) + Math.Pow(vec3.Z, 2));
        }
    }
}
