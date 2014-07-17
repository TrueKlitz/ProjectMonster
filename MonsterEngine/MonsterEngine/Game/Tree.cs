using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MonsterEngine.Engine;

namespace MonsterEngine.Game
{
    class Tree
    {
        private GameObject obj;
        Random rnd;
        public Tree(Vector3 _Pos){
            rnd = new Random();
            obj = new GameObject(_Pos, ref Game.modelTank);
            obj.scale = rnd.Next(1000) / 6000.0f + 0.1f;
            obj.rotation.Y = Helper.degToRad(rnd.Next(360));
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
