using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using MonsterEngine.Engine.Render;
using MonsterEngine.Engine;

namespace MonsterEngine.Game
{
    class Input
    {

        private KeyboardState kbState_old;
        private KeyboardState kbState_new;
        private MouseState msState;

        private Core core;

        public Input(Core _core)
        {
            core = _core;
        }

        public void inputUpdate(Camera camera)
        {
            kbState_new = Keyboard.GetState();
            msState = Mouse.GetState();

            if (kbState_new.IsKeyDown(Key.ShiftLeft)) camera.fMovementSpeed = 100.0f;
            else camera.fMovementSpeed = 10.0f;

            if (kbState_new.IsKeyDown(Key.W))
                camera.vMove = new Vector3((float)Math.Sin(Helper.degToRad(camera.fPitch)) * -camera.fMovementSpeed, 0.0f, (float)Math.Cos(Helper.degToRad(camera.fPitch)) * camera.fMovementSpeed);
            if (kbState_new.IsKeyDown(Key.S))
                camera.vMove = new Vector3((float)Math.Sin(Helper.degToRad(camera.fPitch)) * camera.fMovementSpeed, 0.0f, (float)Math.Cos(Helper.degToRad(camera.fPitch)) * -camera.fMovementSpeed);
            if (kbState_new.IsKeyDown(Key.A))
                camera.vMove = new Vector3((float)Math.Sin(Helper.degToRad(camera.fPitch + 90f)) * camera.fMovementSpeed, 0.0f, (float)Math.Cos(Helper.degToRad(camera.fPitch + 90f)) * -camera.fMovementSpeed);
            if (kbState_new.IsKeyDown(Key.D))
                camera.vMove = new Vector3((float)Math.Sin(Helper.degToRad(camera.fPitch - 90f)) * camera.fMovementSpeed, 0.0f, (float)Math.Cos(Helper.degToRad(camera.fPitch - 90f)) * -camera.fMovementSpeed);

            if (kbState_new.IsKeyDown(Key.D) && kbState_new.IsKeyDown(Key.W))
                camera.vMove = new Vector3((float)Math.Sin(Helper.degToRad(camera.fPitch + 45f)) * -camera.fMovementSpeed, 0.0f, (float)Math.Cos(Helper.degToRad(camera.fPitch + 45f)) * camera.fMovementSpeed);

            if (kbState_new.IsKeyDown(Key.A) && kbState_new.IsKeyDown(Key.W))
                camera.vMove = new Vector3((float)Math.Sin(Helper.degToRad(camera.fPitch - 45f)) * -camera.fMovementSpeed, 0.0f, (float)Math.Cos(Helper.degToRad(camera.fPitch - 45f)) * camera.fMovementSpeed);

            if (kbState_new.IsKeyUp(Key.W) && kbState_new.IsKeyUp(Key.S) && kbState_new.IsKeyUp(Key.A) && kbState_new.IsKeyUp(Key.D))
                camera.vMove = new Vector3(0.0f, 0.0f, 0.0f);

            if (kbState_new.IsKeyDown(Key.Q))
            {
                camera.vMove.Y = -camera.fMovementSpeed;
            }
            if (kbState_new.IsKeyDown(Key.E))
            {
                camera.vMove.Y = camera.fMovementSpeed;
            }
            if (kbState_new.IsKeyDown(Key.Escape))
                core.gameWindow.Exit();

            Point WindowCenter = new Point(core.gameWindow.X + core.gameWindow.Width / 2, core.gameWindow.Y + core.gameWindow.Height / 2);
            msState = Mouse.GetState();
            camera.fPitch = msState.X / 10.0f + 90f;
            camera.fYaw = msState.Y / 10.0f + 65f;
            Mouse.SetPosition(WindowCenter.X, WindowCenter.Y);
            kbState_old = kbState_new;
        }
    }
}
