using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using MonsterEngine.Engine;

namespace MonsterEngine.Game
{
    class Tank
    {
        private GameObject obj;
        public Tank(Vector3 _Pos){
            obj = new GameObject(_Pos, ref Game.modelTank);
            obj.scale = 1f;
            //obj.rotation.Y = MathHelper.DegreesToRadians(90);
            obj.drawDistance = 100;
        }
        public void Update()
        {
        }
        public void Draw()
        {
            obj.Draw();
        }
    }
}
