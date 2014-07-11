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
        private Level level;
        private Core core;
        private Input input;
        public Camera camera;

        public Terrain terrain;

        private GameObject test;

        public Game(Core _core)
        {
            core = _core;

            camera = new Camera(_core, Matrix4.CreateTranslation(10f, 0f, -5f), Matrix4.CreatePerspectiveFieldOfView(0.75f, _core.gameWindow.Width / (_core.gameWindow.Height * 1.0f), 0.01f, 500f)); ;
            input = new Input(core);
            level = new Level(256, "Test1");
            Load();
        }

        public void Load()
        {

            level.Load();
            terrain = new Terrain(level.faHeightmap , level.faHeightMapNormalGen, 0.0f,0.0f);
            level.disposeData();
            test = new GameObject("TestObj");

            camera.SetShaderPointer(terrain.shaderProgramHandle);
            
        }

        public void Update()
        {

            if (core.gameWindow.Focused)
            {
                input.inputUpdate(camera);
                camera.update();
            }

        }

        public void Draw()
        {
            camera.SetShaderPointer(terrain.shaderProgramHandle);
            terrain.Draw();
            camera.SetShaderPointer(test.shaderProgramHandle);
            test.BindDraw();
            test.Draw();
        }

    }
}
