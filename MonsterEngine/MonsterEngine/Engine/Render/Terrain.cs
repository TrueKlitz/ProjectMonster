using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace MonsterEngine.Engine.Render
{
    class Terrain
    {

        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] texCoord;

        private int iVBO, iIBO, iTBO, iNBO;
        private int terrainSize;// 256 is the maximum
        private int iTriangleCount = 0;
        private static int tGrass, tRock, tSand, tGrassRock, tDirt;

        private float[,] faHeightMap;
        private float[,] faNormalMap;
        public int vertexHandle, fragmentHandle, shaderProgramHandle;
        private float PositionX, PositionY;

        public Terrain(float[,] _HeightMap, float[,] _NormalMap, float _PositionX, float _PositionY)
        {
            faHeightMap = _HeightMap;
            faNormalMap = _NormalMap;
            terrainSize = faHeightMap.GetLength(0);

            tGrass = Texture.LoadTexture(".../.../Textures/Grass.png");
            tRock = Texture.LoadTexture(".../.../Textures/Rock.png");
            tSand = Texture.LoadTexture(".../.../Textures/Sand.png");
            tGrassRock = Texture.LoadTexture(".../.../Textures/GrassRock.png");
            tDirt = Texture.LoadTexture(".../.../Textures/Dirt.png");

            PositionX = _PositionX;
            PositionY = _PositionY;

            LoadShader();
            CreateBuffers();
            BindBuffers();
        }

        private void LoadShader()
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
                gl_Position = projection_matrix * camera_matrix * vec4(vertex_position.xyz, 1 );
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
            Console.WriteLine("\n" + programInfoLog);

            GL.BindAttribLocation(shaderProgramHandle, 0, "vertex_position");
            GL.BindAttribLocation(shaderProgramHandle, 1, "vertex_normal");
            GL.BindAttribLocation(shaderProgramHandle, 2, "vertex_texCoord");

        }

        private void BindTextures()
        {
            Texture.BindTexture(ref tGrass, TextureUnit.Texture0, "texGrass", shaderProgramHandle);
            Texture.BindTexture(ref tRock, TextureUnit.Texture1, "texRock", shaderProgramHandle);
            Texture.BindTexture(ref tSand, TextureUnit.Texture2, "texSand", shaderProgramHandle);
            Texture.BindTexture(ref tGrassRock, TextureUnit.Texture3, "texGrassRock", shaderProgramHandle);
            Texture.BindTexture(ref tDirt, TextureUnit.Texture4, "texDirt", shaderProgramHandle);
        }

        private void CreateBuffers()
        {
            terrainSize -= 2;
            iTriangleCount = ((terrainSize - 1) * 6) * (terrainSize - 1);
            vertices = new Vector3[terrainSize * terrainSize];
            texCoord = new Vector2[terrainSize * terrainSize];
            normals = new Vector3[terrainSize * terrainSize];

            float MapGenSize = 6;
            Random rnd = new Random();
            for (int x = 0; x < terrainSize; x++)
            {
                for (int z = 0; z < terrainSize; z++)
                {
                    vertices[x * terrainSize + z] = new Vector3(x / MapGenSize + PositionX, faHeightMap[x + 1, z + 1], z / MapGenSize + PositionY);

                    texCoord[x * terrainSize + z] = new Vector2(x / 13f, z / 13f);

                    //texCoord[x * iMapSize + z].Scale(new Vector2(1.6f,1.6f));

                    if (x == 0) vertices[x * terrainSize + z] = new Vector3((x + 1) / MapGenSize + PositionX, faHeightMap[x + 1, z + 1], z / MapGenSize + PositionY);
                    if (z == 0) vertices[x * terrainSize + z] = new Vector3((x) / MapGenSize + PositionX, faHeightMap[x + 1, z + 1], (z + 1) / MapGenSize + PositionY);
                    if (x == terrainSize - 1) vertices[x * terrainSize + z] = new Vector3((x - 1) / MapGenSize + PositionX, faHeightMap[x + 1, z + 1], z / MapGenSize + PositionY);
                    if (z == terrainSize - 1) vertices[x * terrainSize + z] = new Vector3((x) / MapGenSize + PositionX, faHeightMap[x + 1, z + 1], (z - 1) / MapGenSize + PositionY);

                    if (x == 0 && z == 0) vertices[x * terrainSize + z] = new Vector3((x + 1) / MapGenSize + PositionX, faHeightMap[x + 1, z + 1], (z + 1) / MapGenSize + PositionY);
                    if (x == terrainSize - 1 && z == 0) vertices[x * terrainSize + z] = new Vector3((x - 1) / MapGenSize + PositionX, faHeightMap[x + 1, z + 1], (z + 1) / MapGenSize + PositionY);
                    if (x == terrainSize - 1 && z == terrainSize - 1) vertices[x * terrainSize + z] = new Vector3((x - 1) / MapGenSize + PositionX, faHeightMap[x + 1, z + 1], (z - 1) / MapGenSize + PositionY);
                    if (x == 0 && z == terrainSize - 1) vertices[x * terrainSize + z] = new Vector3((x + 1) / MapGenSize + PositionX, faHeightMap[x + 1, z + 1], (z - 1) / MapGenSize + PositionY);
                }
            }

            for (int x = 0; x < terrainSize; x++)
            {
                for (int z = 0; z < terrainSize; z++)
                {
                    if (x > 1 && z > 1)
                    {
                        Vector3 u = new Vector3(1.0f, 0.0f, faNormalMap[x + 1, z] - faNormalMap[x - 1, z] );
                        Vector3 v = new Vector3(0.0f, 1.0f, faNormalMap[x, z + 1] - faNormalMap[x, z - 1] );
                        normals[x * terrainSize + z] = Vector3.Normalize(Vector3.Cross(u, v));
                    }
                    else
                        normals[x * terrainSize + z] = new Vector3(0.0f, 0.0f, 0.0f);
                }
            }

            faNormalMap = null;

            short[] indices = new short[terrainSize * terrainSize * 6];
            int index = 0;

            for (int x = 0; x < terrainSize; x++)
            {
                for (int z = 0; z < terrainSize - 1; z++)
                {
                    int offset = x * terrainSize + z;
                    indices[index] = (short)(offset + 0);
                    indices[index + 1] = (short)(offset + 1);
                    indices[index + 2] = (short)(offset + terrainSize);
                    indices[index + 3] = (short)(offset + 1);
                    indices[index + 4] = (short)(offset + terrainSize + 1);
                    indices[index + 5] = (short)(offset + terrainSize);
                    index += 6;
                }
            }

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
                                   new IntPtr(indices.Length * sizeof(short)),
                                   indices, BufferUsageHint.StaticDraw);
        }

        public void BindBuffers()
        {
            //Generate Indices  
            GL.EnableVertexAttribArray(3);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, iIBO);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);

            //Aktiviert Das 3. Vertexattribut und bindet es zum TBO
            GL.EnableVertexAttribArray(2);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iTBO);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);

            //Aktiviert Das 2. Vertexattribut und bindet es zum NBO
            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iNBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            //Aktiviert Das 1. Vertexattribut und bindet es zum VBO
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iVBO);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
        }

        public void Draw()
        {
            GL.UseProgram(shaderProgramHandle);
            BindBuffers();
            BindTextures();
            GL.DrawElements(PrimitiveType.Triangles, iTriangleCount, DrawElementsType.UnsignedShort, 0);
        }
    }
}
