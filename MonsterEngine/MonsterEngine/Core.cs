 using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
//using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System.Collections.Generic;

namespace MonsterEngine
{
    class Core
    {
        private GameWindow game;
        public Player player;
        private Vector3[] vertices;// new Vector3[iMapSize * iMapSize];
        private KeyboardState kbState_old;
        private KeyboardState kbState_new;
        private MouseState msState;
        private Matrix4 mCamera;

        public float fDeltaTime;
        private float fConsoleUpdate;

        private int iMapSize = 128;
        private int iTriangleCount = 0;
        private int iVBO, iIBO;
        private int iTest = 0;

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

            string sVertex = ".../.../shader/vertex.glsl";
            string sFragement = ".../.../shader/fragment.glsl";
            
            int vShaderHandle, fShaderHandle, programmHandle;
            vShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            fShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            programmHandle = GL.CreateProgram();  //create shader program

            sVertex = System.IO.File.ReadAllText(@sVertex);
            sFragement = System.IO.File.ReadAllText(@sFragement);

            Console.Write(sVertex + "\n" + sFragement);

            GL.ShaderSource(vShaderHandle, sVertex);  //attach file to the shader handle
            GL.ShaderSource(fShaderHandle, sFragement);

            GL.CompileShader(vShaderHandle); //compile the shaders
            GL.CompileShader(fShaderHandle);

            GL.AttachShader(programmHandle, vShaderHandle);
            GL.AttachShader(programmHandle, fShaderHandle);

            GL.LinkProgram(programmHandle);

            GL.UseProgram(programmHandle);

            player = new Player(this);
            
            CreateVertexBuffer();
            mCamera = Matrix4.CreateTranslation(0f, 0f, -5f);
        }
        
        void CreateVertexBuffer()
        {
            iTriangleCount = ((iMapSize - 1) * 6) * (iMapSize - 1);
            vertices = new Vector3[iMapSize * iMapSize];

            for (int x = 0; x < iMapSize; x++)
            {
                for (int z = 0; z < iMapSize; z++)
                {
                    vertices[x * iMapSize + z] = new Vector3(x/32.0f, (float)(Math.Cos(x/10.0f+iTest)),z/32.0f); 
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

            GL.GenBuffers(1, out iVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iVBO);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(vertices.Length * Vector3.SizeInBytes),
                                   vertices, BufferUsageHint.StaticDraw);
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
            iTest++;
            for (int x = 0; x < iMapSize; x++)
            {
                for (int z = 0; z < iMapSize; z++)
                {
                    vertices[x * iMapSize + z] = new Vector3(x / 32.0f, (float)((Math.Sin(x / 20.0f) / 4.0f) * (Math.Cos((iTest) / 100.0f) / 3.0f) + (Math.Sin(x) * Math.Cos(z * x) / 10.0f)), z / 32.0f);
                }
            }
            for (int x = 1; x < iMapSize -1; x++)
            {
                for (int z = 1; z < iMapSize -1; z++)
                {
                    vertices[(x) * iMapSize + (z)].Y = (
                        vertices[(x + 1) * iMapSize + (z)].Y +
                        vertices[(x - 1) * iMapSize + (z)].Y +
                        vertices[(x) * iMapSize + (z + 1)].Y +
                        vertices[(x) * iMapSize + (z - 1)].Y
                        ) / 4;
                }
            }
            GL.GenBuffers(1, out iVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iVBO);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(vertices.Length * Vector3.SizeInBytes),
                                   vertices, BufferUsageHint.StaticDraw);
            // add game logic, input handling
            if (game.Focused)
            {
                inputUpdate();
            }

            fDeltaTime = (float) (100.0 / game.RenderFrequency);

            fConsoleUpdate += fDeltaTime;

            player.update();

            if (fConsoleUpdate > 100)
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

            GL.CullFace(CullFaceMode.Back);
            
            GL.PolygonMode(MaterialFace.FrontAndBack,PolygonMode.Point);
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
