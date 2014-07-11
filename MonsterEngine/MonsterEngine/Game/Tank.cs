using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace MonsterEngine.Game
{
    class Tank
    {
        Matrix4 location;
        public Tank(Matrix4 _location){
            location = _location;
        }
        public void Update()
        {
        }
        public void Draw()
        {
            Game.modelTank.SetModelViewMatrix(location);
            Game.modelTank.Draw();
        }
    }
}
