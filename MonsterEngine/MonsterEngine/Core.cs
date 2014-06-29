﻿ using System;
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
        private Player player;
        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] texCoord;
        private KeyboardState kbState_old;
        private KeyboardState kbState_new;
        private MouseState msState;
        private Matrix4 mCamera, mProjection;
        private Level level;

        private static int tGrass,tRock;

        private float fDeltaTime;
        private float fConsoleUpdate;
        private int uniformCameraMatrixPointer, uniformProjectionMatrixPointer, vertexHandle, fragmentHandle, shaderProgramHandle;
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

            level = new Level(200, "Testlevel");
            player = new Player(this);
            
            CreateBuffers();

            mCamera = Matrix4.CreateTranslation(0f, 0f, -5f);

            Console.Write("\n"+GL.GetString(StringName.Version));

            tGrass = Texture.LoadTexture(".../.../textures/Grass.png");
            tRock = Texture.LoadTexture(".../.../textures/Rock.png");

            Console.Write("\n Texture ID: "+ tGrass + "," + tRock );

            mProjection = Matrix4.CreatePerspectiveFieldOfView(1.0f, 16/9.0f, 0.001f, 100f);

            loadShader();

        }

        void loadShader()
        {
            string vertexShaderSource = @"
            #version 140
 
            // object space to camera space transformation
            uniform mat4 camera_matrix;            
 
            // camera space to clip coordinates
            uniform mat4 projection_matrix;

            // incoming vertex position
            in vec3 vertex_position;
 
            // incoming vertex normal
            in vec3 vertex_normal;
 
            // transformed vertex normal
            out vec3 normal;
 
            void main(void)
            {
              //not a proper transformation if modelview_matrix involves non-uniform scaling
              normal = (vec4( vertex_normal, 0 ) ).xyz;
 
              // transforming the incoming vertex position
                gl_Position = projection_matrix * camera_matrix * vec4( vertex_position, 1 );
            }";
            string fragmentShaderSource = @"
            #version 140
 
            precision highp float;
 
            const vec3 ambient = vec3( 0.2, 0.2, 0.2 );
            const vec3 lightVecNormalized = normalize( vec3( 0.3, 0.25, 0.5 ) );
            const vec3 lightColor = vec3( 0.2, 0.7, 0.2 );
 
            //uniform sampler2D gSampler;
            in vec3 normal;
 
            out vec4 out_frag_color;

            void main(void)
            {
              //vec4 texture = texture2D(gSampler, TexCoord0.st);
              float diffuse = clamp( dot( lightVecNormalized, normalize( normal ) ), 0.0, 1.0 );
              out_frag_color = vec4( ambient + diffuse * lightColor, 1.0 );
            }";

            vertexHandle = GL.CreateShader(ShaderType.VertexShader);
            fragmentHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexHandle, vertexShaderSource);
            GL.ShaderSource(fragmentHandle, fragmentShaderSource);

            GL.CompileShader(vertexHandle);
            GL.CompileShader(fragmentHandle);

            shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexHandle);
            GL.AttachShader(shaderProgramHandle, fragmentHandle);

            GL.LinkProgram(shaderProgramHandle);
            GL.UseProgram(shaderProgramHandle);
            string programInfoLog;
            GL.GetProgramInfoLog(shaderProgramHandle, out programInfoLog);
            Console.WriteLine(programInfoLog); 
            
        }
        
        void CreateBuffers()
        {
            iMapSize = level.MapSize()-2;
            iTriangleCount = ((iMapSize - 1) * 6) * (iMapSize - 1);
            vertices = new Vector3[iMapSize * iMapSize];
            texCoord = new Vector2[iMapSize * iMapSize];
            normals = new Vector3[iMapSize * iMapSize];

            Random rnd = new Random();
            for (int x = 0; x < iMapSize ; x++)
            {
                for (int z = 0; z < iMapSize; z++)
                {
                    
                    vertices[x * iMapSize + z] = new Vector3(x / 8.0f, level.faHeightmap[x+1 , z+1 ], z / 8.0f);
                    texCoord[x * iMapSize + z] = new Vector2(x/10.0f, z/10.0f);

                    if (x == 0) vertices[x * iMapSize + z] = new Vector3((x + 1) / 8.0f, level.faHeightmap[x + 1, z + 1], z / 8.0f);
                    if (z == 0) vertices[x * iMapSize + z] = new Vector3((x) / 8.0f, level.faHeightmap[x + 1, z + 1], (z + 1) / 8.0f);
                    if (x == iMapSize-1) vertices[x * iMapSize + z] = new Vector3((x - 1) / 8.0f, level.faHeightmap[x + 1, z + 1], z / 8.0f);
                    if (z == iMapSize-1) vertices[x * iMapSize + z] = new Vector3((x) / 8.0f, level.faHeightmap[x + 1, z + 1], (z - 1) / 8.0f);

                    if (x == 0 && z == 0) vertices[x * iMapSize + z] = new Vector3((x + 1) / 8.0f, level.faHeightmap[x + 1, z + 1], (z+1) / 8.0f);
                    if (x == iMapSize-1 && z == 0) vertices[x * iMapSize + z] = new Vector3((x - 1) / 8.0f, level.faHeightmap[x + 1, z + 1], (z + 1) / 8.0f);
                    if (x == iMapSize-1 && z == iMapSize-1) vertices[x * iMapSize + z] = new Vector3((x - 1) / 8.0f, level.faHeightmap[x + 1, z + 1], (z - 1) / 8.0f);
                    if (x == 0 && z == iMapSize - 1) vertices[x * iMapSize + z] = new Vector3((x + 1) / 8.0f, level.faHeightmap[x + 1, z + 1], (z - 1) / 8.0f);
                }
            }

            for (int x = 0; x < iMapSize ; x++)
            {
                for (int z = 0; z < iMapSize ; z++)
                {
                    if (x > 1 && z > 1)
                    {
                        Vector3 u = new Vector3(1.0f, 0.0f, level.faHeightMapTemp[x + 1, z] - level.faHeightMapTemp[x - 1, z]);
                        Vector3 v = new Vector3(0.0f, 1.0f, level.faHeightMapTemp[x, z + 1] - level.faHeightMapTemp[x, z - 1]);
                        normals[x * iMapSize + z] = Vector3.Normalize(Vector3.Cross(u, v));
                    }
                    else
                        normals[x * iMapSize + z] = new Vector3(0.0f,0.0f,0.0f);

                }
            }

            short[] indices = new short[iMapSize * iMapSize * 6];
            int index = 0;

            for (int x = 0; x < iMapSize ; x++)
            {
               for (int z = 0; z < iMapSize -1; z++)
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

            level.disposeData();

            //Generate TexCoordinates
            GL.GenBuffers(1, out iTBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iTBO);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer,
                                   new IntPtr(texCoord.Length * Vector2.SizeInBytes),
                                   texCoord, BufferUsageHint.StaticDraw);

            //Generate Normals
            GL.GenBuffers(1, out iNBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iNBO);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(normals.Length * Vector3.SizeInBytes),
                                   normals, BufferUsageHint.StaticDraw);

            //Generate Vertices
            GL.GenBuffers(1, out iVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iVBO);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(vertices.Length * Vector3.SizeInBytes),
                                   vertices, BufferUsageHint.StaticDraw);

            //Generate Indices  
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
            GL.ClearColor(Color4.DarkBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref mCamera);

            uniformCameraMatrixPointer = GL.GetUniformLocation(shaderProgramHandle, "camera_matrix");
            uniformProjectionMatrixPointer = GL.GetUniformLocation(shaderProgramHandle, "projection_matrix");
            SetCameraMatrix();
            SetProjectionMatrix();

            //Aktiviert Das 2. Vertexattribut und bindet es zum NBO
            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iNBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);


            //Aktiviert Das 1. Vertexattribut und bindet es zum VBO
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iVBO);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.PolygonMode(MaterialFace.FrontAndBack,PolygonMode.Fill);
            GL.Enable(EnableCap.CullFace);

            GL.PushMatrix();
            GL.DrawElements(PrimitiveType.Triangles, iTriangleCount , DrawElementsType.UnsignedShort, 0);
            GL.PopMatrix();

            game.SwapBuffers();
        }

        private void SetCameraMatrix()
        {
            GL.UniformMatrix4(uniformCameraMatrixPointer, false, ref mCamera);
        }

        private void SetProjectionMatrix()
        {
            GL.UniformMatrix4(uniformProjectionMatrixPointer, false, ref mProjection);
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
            return degree * (float) ( Math.PI / 180.0f) ;
        }

    }
}
