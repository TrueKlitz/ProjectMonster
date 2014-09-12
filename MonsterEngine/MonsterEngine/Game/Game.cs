using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonsterEngine.Engine;
using MonsterEngine.Engine.Render;
using MonsterEngine.Game;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using System.Diagnostics;

namespace MonsterEngine.Game
{
    class Game
    {
        private Core core;
        private Input input;
        public Camera camera;
        public Shaders shader;

        public Stopwatch swUpdate = new Stopwatch(),swDraw = new Stopwatch();

        public ProcedualLevel pL;

        public Game(Core _core)
        {
            core = _core;
            shader = new Shaders();
            camera = new Camera(_core, Matrix4.CreateTranslation(10f, 0f, -5f), Matrix4.CreatePerspectiveFieldOfView(0.75f, _core.gameWindow.Width / (_core.gameWindow.Height * 1.0f), 0.01f, 2000f)); ;
            input = new Input(core);
        }

        public void Load()
        {
            pL = new ProcedualLevel();
        }

        public void Update()
        {
            swUpdate.Restart();
            if (core.gameWindow.Focused)
            {
                input.inputUpdate(camera);
                camera.update();
            }
            swUpdate.Stop();
        }

        public void Draw()
        {
            swDraw.Restart();
            if (core.gameWindow.Focused)
            {
                pL.Draw();
            }
            swDraw.Stop();
        }
    }
}
