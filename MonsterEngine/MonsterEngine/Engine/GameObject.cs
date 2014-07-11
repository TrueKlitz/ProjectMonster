using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Platform;
using System.Globalization;
using System.Diagnostics;
using MonsterEngine.Engine.Render;

namespace MonsterEngine.Engine
{
    class GameObject
    {
        String fileLocation;
        String content;

        private Vector3[] vertices;
        //private Vector3[] normals;
        private Vector2[] texCoord;
        private int tGroundTexture;
        private short[] indices;

        private int iVBO, iIBO, iTBO, iNBO;
        private int vertex_count = 0;
        private int face_count = 0;
        private int texCoord_count = 0;
        private int iTriangleCount = 0;

        public int vertexHandle, fragmentHandle, shaderProgramHandle;

        public GameObject(String _fileLocation)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            fileLocation = ".../.../Game/GameObjects/" + _fileLocation + "/object.obj";
            tGroundTexture = Texture.LoadTexture(".../.../Game/GameObjects/"+_fileLocation+"/texture.png");
            LoadFile();
            LoadBuffers();
            LoadShader();
            iTriangleCount = indices.Length / 3;  
            sw.Stop();
            Console.WriteLine("Object has being loaded " + fileLocation + " in " + sw.ElapsedMilliseconds + "ms");
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

            void main(void)
            {
              //not a proper transformation if modelview_matrix involves non-uniform scaling
              normal = (vec4( vertex_normal, 0 ) ).xyz;
              TexCoord0 = vertex_texCoord;
              // transforming the incoming vertex position
                gl_Position = projection_matrix * camera_matrix * vec4(vertex_position.xyz, 0.2 );
            }";
            string fragmentShaderSource = @"
            #version 140
 
            precision highp float;
 
            const vec3 ambient = vec3( 0.1, 0.1, 0.1 );
            const vec3 lightVecNormalized = normalize( vec3( 0.3, 0.25, 0.5 ) );
            const vec3 lightColor = vec3( 0.6, 0.6, 0.6 );
 
            uniform sampler2D texGround;

            in vec3 normal;
            in vec2 TexCoord0;

            out vec4 out_frag_color;

            void main(void)
            {
              vec4 texture = vec4(1,1,1,1);
              texture = texture2D(texGround, TexCoord0.xy ) ;
              
              //float diffuse = clamp( dot( lightVecNormalized, normalize( normal ) ), 0.0, 1.0 );
              out_frag_color = texture;// * vec4( ambient + diffuse * lightColor, 1.0 );
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
            Texture.BindTexture(ref tGroundTexture , TextureUnit.Texture0, "texGround", shaderProgramHandle);
        }
        private void LoadFile()
        {
            
            if (String.IsNullOrEmpty(fileLocation))
            {
                throw new ArgumentException(fileLocation);
            }
            else
            {
                if (File.Exists(fileLocation))
                {
                    content = File.ReadAllText(fileLocation);
                    Console.WriteLine("Object is being loaded " + fileLocation + " ");
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            String[] content_line = content.Split('\n');

            for (int line = 0; line < content_line.Length; line++)
            {
                if (content_line[line].IndexOf("v ", 0) == 0)
                {
                    vertex_count++;
                }
                if (content_line[line].IndexOf("f ", 0) == 0)
                {
                    face_count++;
                }
                if (content_line[line].IndexOf("vt ", 0) == 0)
                {
                    texCoord_count++;
                }
            }

            vertices = new Vector3[vertex_count];
            indices = new short[face_count * 3];
            texCoord = new Vector2[texCoord_count];

            vertex_count = 0;
            face_count = 0;
            texCoord_count = 0;

            for (int line = 0; line < content_line.Length; line++)
            {
                String[] line_split = content_line[line].Split(' ');
                if (content_line[line].IndexOf("v ", 0) == 0)
                {  
                    vertices[vertex_count] = new Vector3(float.Parse(line_split[1], CultureInfo.InvariantCulture), float.Parse(line_split[2], CultureInfo.InvariantCulture), float.Parse(line_split[3], CultureInfo.InvariantCulture));
                    vertex_count++;
                }
                if (content_line[line].IndexOf("f ", 0) == 0)
                {
                    indices[face_count] = short.Parse(line_split[1].Split('/')[0]);
                    indices[face_count + 1] = short.Parse(line_split[2].Split('/')[0]);
                    indices[face_count + 2] = short.Parse(line_split[3].Split('/')[0]);
                    face_count += 3;
                }
                if (content_line[line].IndexOf("vt ", 0) == 0)
                {
                    texCoord[texCoord_count] = new Vector2(float.Parse(line_split[1], CultureInfo.InvariantCulture), float.Parse(line_split[2], CultureInfo.InvariantCulture));
                    texCoord_count++;
                }
                line_split = null;
            }

            content_line = null;
            content = null;

        }

        private void LoadBuffers()
        {
            //Generate TexCoordinates
            GL.GenBuffers(1, out iTBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iTBO);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer,
                                   new IntPtr(texCoord.Length * Vector2.SizeInBytes),
                                   texCoord, BufferUsageHint.StaticDraw);

            //Generate Normals
            /*GL.GenBuffers(1, out iNBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iNBO);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(normals.Length * Vector3.SizeInBytes),
                                   normals, BufferUsageHint.StaticDraw);*/

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
            /*GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iNBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);*/

            //Aktiviert Das 1. Vertexattribut und bindet es zum VBO
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iVBO);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
        }

        public void BindDraw()
        {
            GL.UseProgram(shaderProgramHandle);
            BindBuffers();
            BindTextures();
        }
        public void Draw()
        {
            GL.DrawElements(PrimitiveType.Triangles, iTriangleCount, DrawElementsType.UnsignedShort, 0);
        }
    }
}
