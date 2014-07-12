using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using MonsterEngine.Engine;

namespace MonsterEngine.Game
{
    class GameObject
    {
        Matrix4 location;
        public Vector3 position, rotation;
        public float scale;

        private GameModel gameModel;

        public GameObject(Vector3 _Pos,ref GameModel _Model){
            gameModel = _Model;
            position = _Pos;
            rotation = new Vector3(0, 0, 0);
            scale = 1.0f;
        }
        public void Update()
        {
            Matrix4 rotationMat = Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z);
            location = rotationMat * Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(position);
        }
        public void Draw()
        {
            gameModel.SetModelViewMatrix(location);
            gameModel.Draw();
        }
    }
}
