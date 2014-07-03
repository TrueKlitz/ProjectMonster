using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace MonsterEngine.Engine.Render
{
    class Camera
    {
        Core core;
        
        public Vector3 vMove;
        public Vector3 vPosition;

        public float fMovementSpeed;

        public float fPitch, fYaw;

        public Camera(Core core_)
        {
            core = core_;
            fMovementSpeed = 0.14f;
            vPosition = new Vector3(0.0f,0.0f,-5f);
        }
        public void update()
        {
            vPosition += vMove;
        }
    }
}
