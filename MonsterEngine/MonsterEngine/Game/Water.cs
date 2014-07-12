using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MonsterEngine.Game
{
    class Water
    {
        private GameObject obj;
        public Water(Vector3 _Pos){
            obj = new GameObject(_Pos, ref Game.modelWater);
            obj.scale = 21f;
        }
        private float rotation = 0;
        public void Update()
        {
            rotation += 0.01f;
            obj.position.Y = 1.0f + (float)((1.0f + Math.Cos(rotation)) / 2.0f) / 3.0f;
            obj.Update();
        }
        public void Draw()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            obj.Draw();
            GL.Disable(EnableCap.Blend);        
        }
    }
}
