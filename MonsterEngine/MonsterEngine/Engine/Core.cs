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
        
        private static int tGrass,tRock,tSand,tGrassRock, tDirt;

        private float fDeltaTime;
        private float fConsoleUpdate;
        public int uniformCameraMatrixPointer, uniformProjectionMatrixPointer, vertexHandle, fragmentHandle, shaderProgramHandle;

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

            tGrass = Texture.LoadTexture(".../.../Textures/Grass.png");
            tRock = Texture.LoadTexture(".../.../Textures/Rock.png");
            tSand = Texture.LoadTexture(".../.../Textures/Sand.png");
            tGrassRock = Texture.LoadTexture(".../.../Textures/GrassRock.png");
            tDirt = Texture.LoadTexture(".../.../Textures/Dirt.png");

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
 
            in vec2 vertex_texCoord;

            // transformed vertex normal
            out vec3 normal;
            out vec2 TexCoord0;
            out float height;

            void main(void)
            {
              //not a proper transformation if modelview_matrix involves non-uniform scaling
              normal = (vec4( vertex_normal, 0 ) ).xyz;
              height = vertex_position.y;
              TexCoord0 = vertex_texCoord;
              // transforming the incoming vertex position
                gl_Position = projection_matrix * camera_matrix * vec4( vertex_position, 1 );
            }";
            string fragmentShaderSource = @"
            #version 140
 
            precision highp float;
 
            const vec3 ambient = vec3( 0.1, 0.1, 0.1 );
            const vec3 lightVecNormalized = normalize( vec3( 0.3, 0.25, 0.5 ) );
            const vec3 lightColor = vec3( 0.6, 0.6, 0.6 );
 
            uniform sampler2D texGrass;
            uniform sampler2D texRock;
            uniform sampler2D texSand;
            uniform sampler2D texGrassRock;
            uniform sampler2D texDirt;

            in vec3 normal;
            in vec2 TexCoord0;
            in float height;

            out vec4 out_frag_color;

            void main(void)
            {
              vec4 texture = vec4(1,1,1,1);
              if(height >= 0.00f && height <= 1.0f){ texture = texture2D(texSand, TexCoord0.xy);}
              if(height >= 1.0f && height <= 1.25f){
                float lHeight = (height - 1.0f) * 4.0f;
                texture = ( ( texture2D(texGrass, TexCoord0.xy) * lHeight ) + ( texture2D(texSand, TexCoord0.xy) * (1.0f-lHeight) ) );
              }
              if(height >= 1.25f  && height <= 2.0f){ texture = texture2D(texGrass, TexCoord0.xy);}
              if(height >= 2.0f && height <= 2.25f){
                float lHeight = (height - 2.0f) * 4.0f;
                texture = ( ( texture2D(texGrassRock, TexCoord0.xy) * lHeight ) + ( texture2D(texGrass, TexCoord0.xy) * (1.0f-lHeight) ) );
              }
              if(height >= 2.25f && height <= 3.0f){ texture = texture2D(texGrassRock, TexCoord0.xy);}
              if(height >= 3.0f && height <= 3.25f){
                float lHeight = (height - 3.0f) * 4.0f;
                texture = ( ( texture2D(texRock, TexCoord0.xy) * lHeight ) + ( texture2D(texGrassRock, TexCoord0.xy) * (1.0f-lHeight) ) );
              }
              if(height >= 3.25f){ texture = texture2D(texRock, TexCoord0.xy);}
              if(TexCoord0.x < 0.0f || TexCoord0.y < 0.0f){
                texture = vec4(0,0,0,1);
              }   
              
              float diffuse = clamp( dot( lightVecNormalized, normalize( normal ) ), 0.0, 1.0 );
              out_frag_color = texture * vec4( ambient + diffuse * lightColor, 1.0 );
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
            Console.WriteLine("\n"+programInfoLog);

            GL.BindAttribLocation(shaderProgramHandle, 0, "vertex_position");
            GL.BindAttribLocation(shaderProgramHandle, 1, "vertex_normal");
            GL.BindAttribLocation(shaderProgramHandle, 2, "vertex_texCoord");
            BindTexture(ref tGrass, TextureUnit.Texture0, "texGrass");
            BindTexture(ref tRock, TextureUnit.Texture1, "texRock");
            BindTexture(ref tSand, TextureUnit.Texture2, "texSand");
            BindTexture(ref tGrassRock, TextureUnit.Texture3, "texGrassRock");
            BindTexture(ref tDirt, TextureUnit.Texture4, "texDirt");
        }

        private void BindTexture(ref int textureId, TextureUnit textureUnit, string UniformName)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.Uniform1(GL.GetUniformLocation(shaderProgramHandle, UniformName), textureUnit - TextureUnit.Texture0);
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
            GL.ClearColor(Color4.DarkBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            uniformCameraMatrixPointer = GL.GetUniformLocation(shaderProgramHandle, "camera_matrix");
            uniformProjectionMatrixPointer = GL.GetUniformLocation(shaderProgramHandle, "projection_matrix");

            camera.SetCameraMatrix();
            camera.SetProjectionMatrix();

            GL.Enable(EnableCap.CullFace);

            GL.DrawElements(PrimitiveType.Triangles, game.terrain.GetTriangleCount() , DrawElementsType.UnsignedShort, 0);

            gameWindow.SwapBuffers();
        }    

        public void resize()
        {
            GL.Viewport(0, 0, gameWindow.Width, gameWindow.Height);
        }

    }
}
