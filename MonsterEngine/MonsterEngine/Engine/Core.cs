 using System;
using System.Drawing;
using OpenTK;
using OpenTK.Platform;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;
using MonsterEngine.Engine.Render;
using MonsterEngine.Game;
using System.IO;

namespace MonsterEngine.Engine
{
    class Core
    {
        public GameWindow gameWindow;
        public static Game.Game game;

        public double dDeltaTime;
        private float fConsoleUpdate;

        public int WIDTH, HEIGHT, MSAA;
        public bool FULLSCREEN, VSYNC;
        public String NAME;

        public Core()
        {
            string config = File.ReadAllText(".../.../Game/config.txt");

            string[] config_line = config.Split('\n');
            for (int i = 0; i < config.Split('\n').Length; i++)
            {
                if (config_line[i].StartsWith("MSAA"))          MSAA = int.Parse(config_line[i].Split('=')[1]);
                if (config_line[i].StartsWith("WIDTH"))         WIDTH = int.Parse(config_line[i].Split('=')[1]);
                if (config_line[i].StartsWith("HEIGHT"))        HEIGHT = int.Parse(config_line[i].Split('=')[1]);
                if (config_line[i].StartsWith("NAME"))          NAME = config_line[i].Split('=')[1];
                if (config_line[i].StartsWith("FULLSCREEN"))    FULLSCREEN = bool.Parse(config_line[i].Split('=')[1]);
                if (config_line[i].StartsWith("VSYNC"))         VSYNC = bool.Parse(config_line[i].Split('=')[1]);
            }
        }
    
        public void load(GameWindow game_)
        {
           
            //setup settings, load textures, sounds
            gameWindow = game_;
            if(VSYNC)gameWindow.VSync = VSyncMode.On;
            gameWindow.Title = NAME;
            gameWindow.Width = WIDTH;
            gameWindow.Height = HEIGHT;
            gameWindow.X = 1921;
            gameWindow.Y = 0;
            gameWindow.CursorVisible = false;
            if(FULLSCREEN) gameWindow.WindowState = WindowState.Fullscreen;

            game = new Game.Game(this);
            game.Load();

            Console.WriteLine(GL.GetString(StringName.Version));

            GLEnable();

        }
  
        public void update()
        {
            dDeltaTime = 1000.0 / gameWindow.RenderFrequency;

            fConsoleUpdate += (float)dDeltaTime;

            game.Update();

            if (fConsoleUpdate > 1000)
            {
                //Console.Write("\n Deltatime: " + dDeltaTime + " Playerspeed: " + game.camera.vMove + " Playerposition: " + game.camera.vPosition + "TEST: " + gameWindow.RenderFrequency);
                float cameraTime = game.camera.sw.ElapsedTicks / (TimeSpan.TicksPerMillisecond * 1.0f);
                float terrainTime = game.terrain.sw.ElapsedTicks / (TimeSpan.TicksPerMillisecond * 1.0f);
                float gameUpdateTime = game.swUpdate.ElapsedTicks / (TimeSpan.TicksPerMillisecond * 1.0f);
                float gameDrawTime = game.swDraw.ElapsedTicks / (TimeSpan.TicksPerMillisecond * 1.0f);
                Console.WriteLine("Time spend: Camera["+cameraTime+"ms] Terrain["+terrainTime+"ms] Update["+gameUpdateTime+"ms] Draw["+gameDrawTime+"]" );
                fConsoleUpdate = 0;
            }    
        }
 
        public void draw()
        {
            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            game.Draw();

            gameWindow.SwapBuffers();
        }

        private void GLEnable()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        public void resize()
        {
            GL.Viewport(0, 0, gameWindow.Width, gameWindow.Height);
        }
    }
}
