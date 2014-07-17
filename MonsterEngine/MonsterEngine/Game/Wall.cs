using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using MonsterEngine.Engine;

namespace MonsterEngine.Game
{
    class Wall
    {
        private GameObject obj;
        public Wall(Vector3 _Pos){
            obj = new GameObject(_Pos, ref Game.modelWall);
            obj.scale = 6.5f;
            obj.drawDistance = 10.0f;
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
