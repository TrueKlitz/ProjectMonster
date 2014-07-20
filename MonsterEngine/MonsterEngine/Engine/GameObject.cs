using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using MonsterEngine.Engine.Render;

namespace MonsterEngine.Engine
{
    class GameObject
    {
        Matrix4 location;
        public Vector3 position, rotation;
        public float scale;
        public float drawDistance = 10.0f;
        private GameModel gameModel;

        public GameObject(Vector3 _Pos,ref GameModel _Model){
            gameModel = _Model;
            position = _Pos;
            rotation = new Vector3(0, 0, 0);
            scale = 1.0f;
        }
        public void Draw()
        {
            Vector3 camPos = -Core.game.camera.vPosition;
            if (Helper.Vec3Lenght(new Vector3(camPos.X - position.X, camPos.Y - position.Y, camPos.Z - position.Z)) <= drawDistance)
            {
                Matrix4 rotationMat = Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z);
                location = rotationMat * Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(position);
                gameModel.SetModelViewMatrix(location);
                gameModel.Draw();
            }
        }
    }
}
