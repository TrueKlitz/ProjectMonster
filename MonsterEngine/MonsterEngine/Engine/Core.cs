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

namespace MonsterEngine.Engine
{
    class Core
    {
        public GameWindow gameWindow;
        public static Game.Game game;

        public double dDeltaTime;
        private float fConsoleUpdate;

        public void load(GameWindow game_)
        {
           
            //setup settings, load textures, sounds
            gameWindow = game_;
            gameWindow.VSync = VSyncMode.On;
            gameWindow.Title = "Monster Engine";
            gameWindow.Width = 1600;
            gameWindow.Height = 900;
            gameWindow.X = 1921;
            gameWindow.Y = 0;
            gameWindow.CursorVisible = false;
            gameWindow.WindowState = WindowState.Fullscreen;

            game = new Game.Game(this);
            game.Load();

            Console.Write("\n"+GL.GetString(StringName.Version));
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
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            //GL.Enable(EnableCap.);

            game.Draw();

            gameWindow.SwapBuffers();
        }    

        public void resize()
        {
            GL.Viewport(0, 0, gameWindow.Width, gameWindow.Height);
        }
    }
}
