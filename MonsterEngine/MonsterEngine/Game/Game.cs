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
        
        public static GameModel modelWall, modelGround;

        Wall[] wallTile;
        Ground[] groundTile;

        private int iMapsize = 16;

        private int[] iaMap;

        public Game(Core _core)
        {
            core = _core;
            shader = new Shaders();
            camera = new Camera(_core, Matrix4.CreateTranslation(10f, 0f, -5f), Matrix4.CreatePerspectiveFieldOfView(0.75f, _core.gameWindow.Width / (_core.gameWindow.Height * 1.0f), 0.01f, 500f)); ;
            input = new Input(core);
        }

        public void Load()
        {
            iaMap = new int[iMapsize*iMapsize];

            modelWall = new GameModel("WallCobble");
            modelGround = new GameModel("Ground");
            modelGround.specluar = 0f;
            modelWall.specluar = 0f;

            LoadMap();
        }

        private void LoadMap()
        {
            Random rnd = new Random();
            for (int x = 0; x < iMapsize; x++)
            {
                for (int y = 0; y < iMapsize; y++)
                {
                    if (x == 0 | y == 0 | x == iMapsize - 1 | y == iMapsize - 1)
                    {
                        iaMap[x * iMapsize + y] = 2;
                    }
                    else
                    {
                        iaMap[x * iMapsize + y] = 1;
                    }
                }
            }

            int groundNum = 0;
            int wallNum = 0;

            for (int i = 0; i < iaMap.Length; i++)
            {
                switch (iaMap[i])
                {
                    case 1:
                        groundNum++;
                        break;
                    case 2:
                        wallNum++;
                        break;
                }
            }

            groundTile = new Ground[groundNum];
            wallTile = new Wall[wallNum];

            groundNum = 0;
            wallNum = 0;
            for (int x = 0; x < iMapsize; x++)
            {
                for (int y= 0; y < iMapsize; y++)
                {
                    if (iaMap[x * iMapsize + y] == 1)
                    {
                        groundTile[groundNum] = new Ground(new Vector3(x*2,-1,y*2));
                        groundNum++;
                    }
                    if (iaMap[x * iMapsize + y] == 2)
                    {
                        wallTile[wallNum] = new Wall(new Vector3(x * 2, 0, y * 2));
                        wallNum++;
                    }
                }
            }

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

                modelGround.BindDraw();
                for (int i = 0; i < groundTile.Length; i++)
                {
                    groundTile[i].Draw();
                }
                modelWall.BindDraw();
                for (int i = 0; i < wallTile.Length; i++)
                {
                    wallTile[i].Draw();
                }
            }
            swDraw.Stop();
        }
    }
}
