using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using MonsterEngine.Engine;

namespace MonsterEngine.Game
{
    class Ground
    {
        private GameObject obj;
        public Ground(Vector3 _Pos){
            obj = new GameObject(_Pos, ref Game.modelWall);
            obj.scale = 1f;
            obj.drawDistance = 50;
            obj.gameModel.specluar = 0f;
            obj.gameModel.normalMapping = true;
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
