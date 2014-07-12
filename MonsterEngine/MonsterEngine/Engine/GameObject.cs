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
        private Vector3[] normals;
        private Vector2[] texCoord;
        private Vector3[] tangent;

        private int tGroundTexture, tNormal;

        private int iVBO, iTBO, iNBO, iTanBO;
        private int vertex_count = 0;
        private int point_count = 0;
        private int texCoord_count = 0;
        private int normal_count = 0;
        private int uniformModelViewMatrixPointer = 0;

        public GameObject(String _fileLocation)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            fileLocation = ".../.../Game/GameObjects/" + _fileLocation + "/object.obj";
            tGroundTexture = Texture.LoadTexture(".../.../Game/GameObjects/"+_fileLocation+"/texture.png");
            tNormal = Texture.LoadTexture(".../.../Game/GameObjects/" + _fileLocation + "/normal.png");
            LoadFile();
            LoadBuffers();
            uniformModelViewMatrixPointer = GL.GetUniformLocation(Core.game.shader.S2_shaderProgramHandle, "modelview_matrix");
            sw.Stop();
            Console.WriteLine("Object has being loaded " + fileLocation + " in " + sw.ElapsedMilliseconds + "ms");
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
                if (content_line[line].IndexOf("vt ", 0) == 0)
                {
                    texCoord_count++;
                }
                if (content_line[line].IndexOf("vn ", 0) == 0)
                {
                    normal_count++;
                }
                if (content_line[line].IndexOf("f ", 0) == 0)
                {
                    point_count++;
                }
            }       

            Vector3[] vertices_temp = new Vector3[vertex_count];
            Vector3[] normals_temp = new Vector3[normal_count];
            Vector2[] texCoord_temp = new Vector2[texCoord_count];

            vertex_count = 0;
            texCoord_count = 0;
            normal_count = 0;

            for (int line = 0; line < content_line.Length; line++)
            {
                String[] line_split = content_line[line].Split(' ');
                if (content_line[line].IndexOf("v ", 0) == 0)
                {
                    vertices_temp[vertex_count] = new Vector3(float.Parse(line_split[1], CultureInfo.InvariantCulture), float.Parse(line_split[2], CultureInfo.InvariantCulture), float.Parse(line_split[3], CultureInfo.InvariantCulture));
                    vertex_count++;
                }
                if (content_line[line].IndexOf("vt ", 0) == 0)
                {
                    texCoord_temp[texCoord_count] = new Vector2(float.Parse(line_split[1], CultureInfo.InvariantCulture), float.Parse(line_split[2], CultureInfo.InvariantCulture));
                    texCoord_count++;
                }
                if (content_line[line].IndexOf("vn ", 0) == 0)
                {
                    normals_temp[normal_count] = new Vector3(float.Parse(line_split[1], CultureInfo.InvariantCulture), float.Parse(line_split[2], CultureInfo.InvariantCulture), float.Parse(line_split[3], CultureInfo.InvariantCulture));
                    normal_count++;
                }
                line_split = null;
            }


            vertices = new Vector3[point_count * 3];
            texCoord = new Vector2[point_count * 3];
            normals = new Vector3[point_count * 3];
            tangent = new Vector3[point_count * 3];

            point_count = 0;

            for (int line = 0; line < content_line.Length; line++)
            {
                String[] line_split = content_line[line].Split(' ');
                if (content_line[line].IndexOf("f ", 0) == 0)
                {

                    vertices[point_count + 0] = vertices_temp[int.Parse(line_split[1].Split('/')[0]) - 1];
                    vertices[point_count + 1] = vertices_temp[int.Parse(line_split[2].Split('/')[0]) - 1];
                    vertices[point_count + 2] = vertices_temp[int.Parse(line_split[3].Split('/')[0]) - 1];
                    texCoord[point_count + 0] = texCoord_temp[int.Parse(line_split[1].Split('/')[1]) - 1];
                    texCoord[point_count + 1] = texCoord_temp[int.Parse(line_split[2].Split('/')[1]) - 1];
                    texCoord[point_count + 2] = texCoord_temp[int.Parse(line_split[3].Split('/')[1]) - 1];
                    normals[point_count + 0]  = normals_temp [int.Parse(line_split[1].Split('/')[2]) - 1];
                    normals[point_count + 1]  = normals_temp [int.Parse(line_split[2].Split('/')[2]) - 1];
                    normals[point_count + 2]  = normals_temp [int.Parse(line_split[3].Split('/')[2]) - 1];
                    point_count += 3 ;
                }
                line_split = null;
            }

            for (int i = 0; i < normals.Length; i++)
            {
                Vector3 c1 = Vector3.Cross(normals[i],new Vector3(0.0f, 0.0f, 1.0f));
                Vector3 c2 = Vector3.Cross(normals[i],new Vector3(0.0f, 1.0f, 0.0f));

                if (Helper.Vec3Lenght(c1) > Helper.Vec3Lenght(c2))
                {
                    tangent[i] = c1;
                }
                else
                {
                    tangent[i] = c2;
                }
                tangent[i] = Vector3.Normalize(tangent[i]);
            }

            content_line = null;
            content = null;

        }
        
        private void LoadBuffers()
        {
            //Generate TexCoordinates
            GL.GenBuffers(1, out iTanBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iTanBO);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(tangent.Length * Vector3.SizeInBytes),
                                   tangent, BufferUsageHint.StaticDraw);

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

        }      
        
        public void BindBuffers()
        {
            //Aktiviert Das 1. Vertexattribut und bindet es zum VBO
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iVBO);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
            //Aktiviert Das 2. Vertexattribut und bindet es zum NBO
            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iNBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            //Aktiviert Das 3. Vertexattribut und bindet es zum TBO
            GL.EnableVertexAttribArray(2);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iTBO);
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, 0, 0);
            //Aktiviert Das 4. Vertexattribut und bindet es zum TanBO
            GL.EnableVertexAttribArray(3);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iTanBO);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);      

        }

        public void BindDraw()
        {
            GL.UseProgram(Core.game.shader.S2_shaderProgramHandle);
            BindBuffers();
            Core.game.shader.SetAttributesShaderTwo(tGroundTexture, tNormal);
        }

        public void SetModelViewMatrix(Matrix4 mPosScaleRot)
        {
            GL.UniformMatrix4(uniformModelViewMatrixPointer, false, ref mPosScaleRot);
        }

        public void Draw()
        {
            GL.DrawArrays(PrimitiveType.Triangles,0,point_count);
        }
    
    }
}
