using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace MonsterEngine.Engine.Physic
{
    class Collision
    {
        public static bool CubeCollision(Vector3 aPos, Vector3 aSize, Vector3 bPos, Vector3 bSize)
        {
            //check the X axis
            if (Math.Abs(aPos.X- bPos.X) < aSize.X + bSize.X)
            {
                //check the Y axis
                if (Math.Abs(aPos.Y - bPos.Y) < aSize.Y + bSize.Y)
                {
                    //check the Z axis
                    if (Math.Abs(aPos.Z - bPos.Z) < aSize.Z + bSize.Z)
                    {
                        return true;
                    }
                }
            }

            return true;
        }
    }
}
