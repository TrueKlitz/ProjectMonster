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
        public Shaders shader;
        public Terrain terrain;

        public Stopwatch swUpdate = new Stopwatch(),swDraw = new Stopwatch();
        public static GameModel modelTank,modelWater;

        public static Tank tank;
        public static Water water;

        public Game(Core _core)
        {
            core = _core;
            shader = new Shaders();
            camera = new Camera(_core, Matrix4.CreateTranslation(10f, 0f, -5f), Matrix4.CreatePerspectiveFieldOfView(0.75f, _core.gameWindow.Width / (_core.gameWindow.Height * 1.0f), 0.01f, 500f)); ;
            input = new Input(core);
            level = new Level(256, "Test1");
        }

        public void Load()
        {
            level.Load();
            terrain = new Terrain(level.faHeightmap , level.faHeightMapNormalGen, 0.0f,0.0f);
            level.disposeData();
            modelTank = new GameModel("Tank");
            modelWater = new GameModel("Water");
            tank = new Tank(new Vector3(2,2,11));
            water = new Water(new Vector3(21.0f,1.1f,21.0f));
        }

        public void Update()
        {
            swUpdate.Restart();
            if (core.gameWindow.Focused)
            {
                input.inputUpdate(camera);
                camera.update();
                tank.Update();
                water.Update();
            }
            swUpdate.Stop();
        }

        public void Draw()
        {
            swDraw.Restart();
            if (core.gameWindow.Focused)
            {
                modelTank.BindDraw();
                tank.Draw();
                terrain.Draw();
                modelWater.BindDraw();
                water.Draw();
                
               
            }
            swDraw.Stop();
        }
    }
}
