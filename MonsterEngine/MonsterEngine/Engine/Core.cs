 using System;
using System.Drawing;
using OpenTK;
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
        private GameWindow gameWindow;
        private Game.Game game;
        private Camera camera;
        public Helper helper;

        private KeyboardState kbState_old;
        private KeyboardState kbState_new;
        private MouseState msState;

        private float fDeltaTime;
        private float fConsoleUpdate;

        public void load(GameWindow game_)
        {
            game = new Game.Game(this);
            //setup settings, load textures, sounds
            gameWindow = game_;
            gameWindow.VSync = VSyncMode.Off;
            gameWindow.Title = "Monster Engine";
            gameWindow.Width = 1600;
            gameWindow.Height = 900;
            gameWindow.X = 0;
            gameWindow.Y = 0;
            gameWindow.CursorVisible = false;

            helper = new Helper();
            camera = new Camera(this,Matrix4.CreateTranslation(10f, 0f, -5f),Matrix4.CreatePerspectiveFieldOfView(0.75f, gameWindow.Width/(gameWindow.Height*1.0f), 0.01f, 50f));

            Console.Write("\n"+GL.GetString(StringName.Version));
        }

        public void inputUpdate()
        {
            kbState_new = Keyboard.GetState();
            msState = Mouse.GetState();

            if (kbState_new.IsKeyDown(Key.W))
                camera.vMove = new Vector3((float)Math.Sin(helper.degToRad(camera.fPitch)) * -camera.fMovementSpeed, 0.0f, (float)Math.Cos(helper.degToRad(camera.fPitch)) * camera.fMovementSpeed);
            if (kbState_new.IsKeyDown(Key.S))
                camera.vMove = new Vector3((float)Math.Sin(helper.degToRad(camera.fPitch)) * camera.fMovementSpeed, 0.0f, (float)Math.Cos(helper.degToRad(camera.fPitch)) * -camera.fMovementSpeed);
            if (kbState_new.IsKeyDown(Key.A))
                camera.vMove = new Vector3((float)Math.Sin(helper.degToRad(camera.fPitch + 90f)) * camera.fMovementSpeed, 0.0f, (float)Math.Cos(helper.degToRad(camera.fPitch + 90f)) * -camera.fMovementSpeed);
            if (kbState_new.IsKeyDown(Key.D))
                camera.vMove = new Vector3((float)Math.Sin(helper.degToRad(camera.fPitch - 90f)) * camera.fMovementSpeed, 0.0f, (float)Math.Cos(helper.degToRad(camera.fPitch - 90f)) * -camera.fMovementSpeed);

            if (kbState_new.IsKeyDown(Key.D) && kbState_new.IsKeyDown(Key.W))
                camera.vMove = new Vector3((float)Math.Sin(helper.degToRad(camera.fPitch + 45f)) * -camera.fMovementSpeed, 0.0f, (float)Math.Cos(helper.degToRad(camera.fPitch + 45f)) * camera.fMovementSpeed);

            if (kbState_new.IsKeyDown(Key.A) && kbState_new.IsKeyDown(Key.W))
                camera.vMove = new Vector3((float)Math.Sin(helper.degToRad(camera.fPitch - 45f)) * -camera.fMovementSpeed, 0.0f, (float)Math.Cos(helper.degToRad(camera.fPitch - 45f)) * camera.fMovementSpeed);

            if (kbState_new.IsKeyUp(Key.W) && kbState_new.IsKeyUp(Key.S) && kbState_new.IsKeyUp(Key.A) && kbState_new.IsKeyUp(Key.D))
                camera.vMove = new Vector3(0.0f, 0.0f, 0.0f);

            if(kbState_new.IsKeyDown(Key.Q)){
                camera.vMove.Y = -camera.fMovementSpeed;
            }
            if (kbState_new.IsKeyDown(Key.E))
            {
                camera.vMove.Y = camera.fMovementSpeed;
            }
            if(kbState_new.IsKeyDown(Key.Escape))
                gameWindow.Exit();

            Point WindowCenter = new Point(gameWindow.X + gameWindow.Width/2 ,gameWindow.Y + gameWindow.Height/2 );
            msState = Mouse.GetState();
            camera.fPitch = msState.X / 10.0f + 90f;
            camera.fYaw = msState.Y / 10.0f + 65f;
            Mouse.SetPosition(WindowCenter.X, WindowCenter.Y);
            kbState_old = kbState_new;
        }
       
        public void update()
        {
            // add game logic, input handling
            if (gameWindow.Focused)
            {
                inputUpdate();
                camera.update();
            }

            fDeltaTime = (float) (100.0 / gameWindow.RenderFrequency);

            fConsoleUpdate += fDeltaTime;     

            if (fConsoleUpdate > 200)
            {
                Console.Write("\n Deltatime: " + fDeltaTime + " Playerspeed: " + camera.vMove + " Playerposition: " + camera.vPosition);
                fConsoleUpdate = 0;
            }    
        }
 
        public void draw()
        {
            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            camera.SetShaderPointer(game.terrain.shaderProgramHandle);

            game.Draw();

            gameWindow.SwapBuffers();
        }    

        public void resize()
        {
            GL.Viewport(0, 0, gameWindow.Width, gameWindow.Height);
        }

    }
}
