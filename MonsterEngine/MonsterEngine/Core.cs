 using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;

namespace MonsterEngine
{
    class Core
    {
        private GameWindow game;
        public Player player;
        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] texCoord;
        private KeyboardState kbState_old;
        private KeyboardState kbState_new;
        private MouseState msState;
        private Matrix4 mCamera;
        private Level level;

        private static int tGrass,tRock;

        public float fDeltaTime;
        private float fConsoleUpdate;

        private int iMapSize = 256;// 256 is the maximum
        private int iTriangleCount = 0;
        private int iVBO, iIBO, iTBO, iNBO;

        public void load(GameWindow game_)
        {
            //setup settings, load textures, sounds
            game = game_;
            game.VSync = VSyncMode.On;
            game.Title = "Monster Engine";
            game.Width = 1600;
            game.Height = 900;
            game.X = 1921;
            game.Y = 0;

            level = new Level(200, "level1");
            player = new Player(this);
            
            CreateBuffers();

            mCamera = Matrix4.CreateTranslation(0f, 0f, -5f);

            Console.Write("\n"+GL.GetString(StringName.Version));

            tGrass = Texture.LoadTexture(".../.../textures/Grass.png");
            tRock = Texture.LoadTexture(".../.../textures/Rock.png");

            Console.Write("\n Texture ID: "+ tGrass + "," + tRock );
            
        }
        
        void CreateBuffers()
        {
            iMapSize = level.MapSize();
            iTriangleCount = ((iMapSize - 1) * 6) * (iMapSize - 1);
            vertices = new Vector3[iMapSize * iMapSize];
            texCoord = new Vector2[iMapSize * iMapSize];
            normals = new Vector3[iMapSize * iMapSize];

            Random rnd = new Random();
            for (int x = 0; x < iMapSize ; x++)
            {
                for (int z = 0; z < iMapSize; z++)
                {
                    vertices[x * iMapSize + z] = new Vector3(x / 8.0f, level.faHeightmap[x,z], z / 8.0f);
                    normals[x * iMapSize + z] = new Vector3(0.0f,1.0f,0.0f);
                    texCoord[x * iMapSize + z] = new Vector2(x/10.0f, z/10.0f);
                }
            }
            short[] indices = new short[iMapSize * iMapSize * 6];
            int index = 0;

            for (int x = 0; x < iMapSize ; x++)
            {
               for (int z = 0; z < iMapSize-1; z++)
               {
                    int offset = x * iMapSize + z;
                    indices[index] = (short)(offset + 0);
                    indices[index + 1] = (short)(offset + 1);
                    indices[index + 2] = (short)(offset + iMapSize);
                    indices[index + 3] = (short)(offset + 1);
                    indices[index + 4] = (short)(offset + iMapSize + 1);
                    indices[index + 5] = (short)(offset + iMapSize);
                    index += 6;
                }
            }

            GL.GenBuffers(1, out iTBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iTBO);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer,
                                   new IntPtr(texCoord.Length * Vector2.SizeInBytes),
                                   texCoord, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.GenBuffers(1, out iNBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iNBO);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(normals.Length * Vector3.SizeInBytes),
                                   normals, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.GenBuffers(1, out iVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iVBO);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(vertices.Length * Vector3.SizeInBytes),
                                   vertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iVBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.GenBuffers(1, out iIBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, iIBO);
            GL.BufferData<short>(BufferTarget.ElementArrayBuffer,
                                   new IntPtr(indices.Length * sizeof(short) ),
                                   indices, BufferUsageHint.StaticDraw);
        }

        public void inputUpdate()
        {
            kbState_new = Keyboard.GetState();
            msState = Mouse.GetState();

            if (kbState_new.IsKeyDown(Key.W))
                player.vMove = new Vector3( (float)Math.Sin(degToRad(player.fPitch)) * -player.fMovementSpeed, 0.0f, (float)Math.Cos(degToRad(player.fPitch)) * player.fMovementSpeed );
            if (kbState_new.IsKeyDown(Key.S))
                player.vMove = new Vector3((float)Math.Sin(degToRad(player.fPitch)) * player.fMovementSpeed, 0.0f, (float)Math.Cos(degToRad(player.fPitch)) * -player.fMovementSpeed);
            if (kbState_new.IsKeyDown(Key.A))
                player.vMove = new Vector3((float)Math.Sin(degToRad(player.fPitch + 90f)) * player.fMovementSpeed, 0.0f, (float)Math.Cos(degToRad(player.fPitch + 90f)) * -player.fMovementSpeed);
            if (kbState_new.IsKeyDown(Key.D))
                player.vMove = new Vector3((float)Math.Sin(degToRad(player.fPitch - 90f)) * player.fMovementSpeed, 0.0f, (float)Math.Cos(degToRad(player.fPitch - 90f)) * -player.fMovementSpeed);

            if (kbState_new.IsKeyDown(Key.D) && kbState_new.IsKeyDown(Key.W))
                player.vMove = new Vector3((float)Math.Sin(degToRad(player.fPitch + 45f)) * -player.fMovementSpeed, 0.0f, (float)Math.Cos(degToRad(player.fPitch + 45f)) * player.fMovementSpeed);

            if (kbState_new.IsKeyDown(Key.A) && kbState_new.IsKeyDown(Key.W))
                player.vMove = new Vector3((float)Math.Sin(degToRad(player.fPitch - 45f)) * -player.fMovementSpeed, 0.0f, (float)Math.Cos(degToRad(player.fPitch - 45f)) * player.fMovementSpeed);

            if (kbState_new.IsKeyUp(Key.W) && kbState_new.IsKeyUp(Key.S) && kbState_new.IsKeyUp(Key.A) && kbState_new.IsKeyUp(Key.D))
                player.vMove = new Vector3(0.0f, 0.0f, 0.0f);

            if(kbState_new.IsKeyDown(Key.Q)){
                player.vMove.Y = -player.fMovementSpeed;
            }
            if (kbState_new.IsKeyDown(Key.E))
            {
                player.vMove.Y = player.fMovementSpeed;
            }
            if(kbState_new.IsKeyDown(Key.Escape))
                game.Exit();

            Point WindowCenter = new Point(game.X + game.Width/2 ,game.Y + game.Height/2 );
            msState = Mouse.GetState();
            player.fPitch = msState.X / 10.0f;
            player.fYaw = msState.Y / 10.0f;
            Mouse.SetPosition(WindowCenter.X, WindowCenter.Y);
            kbState_old = kbState_new;
        }
       
        public void update()
        {
            // add game logic, input handling
            if (game.Focused)
            {
                inputUpdate();
            }

            fDeltaTime = (float) (100.0 / game.RenderFrequency);

            fConsoleUpdate += fDeltaTime;

            player.update();

            if (fConsoleUpdate > 200)
            {
                Console.Write("\n Deltatime: " + fDeltaTime + " Playerspeed: " + player.vMove + " Playerposition: " + player.vPosition);

                fConsoleUpdate = 0;
            }

            mCamera = Matrix4.CreateTranslation(player.vPosition) * Matrix4.CreateRotationY( degToRad(player.fPitch) ) * Matrix4.CreateRotationX( degToRad(player.fYaw));           
        }
        
        public void draw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color4.DarkBlue);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref mCamera);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iVBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.PolygonMode(MaterialFace.FrontAndBack,PolygonMode.Line);
            GL.ActiveTexture(TextureUnit.Texture0 + tGrass);
            GL.DepthMask(true);
            GL.PushMatrix();
            GL.DrawElements(PrimitiveType.Triangles, iTriangleCount , DrawElementsType.UnsignedShort, 0);
            GL.PopMatrix();
            game.SwapBuffers();
        }

        public void resize()
        {
            GL.Viewport(0, 0, game.Width, game.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, game.Width / (float)game.Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }
        
        //Helper Functions:
        public float degToRad(float degree)
        {
            return degree * 0.0174532925f;
        }

    }
}
