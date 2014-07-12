using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace MonsterEngine.Game
{
    class Tank
    {
        private GameObject obj;
        public Tank(Vector3 _Pos){
            obj = new GameObject(_Pos, ref Game.modelTank);
        }
        public void Update()
        {
            obj.Update();
        }
        public void Draw()
        {
            obj.Draw();
        }
    }
}
