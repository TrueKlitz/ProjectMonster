using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Diagnostics;

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
        private float PositionX, PositionY;

        public Stopwatch sw = new Stopwatch();

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

            CreateBuffers();
            BindBuffers();
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

        private void BindBuffers()
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
            sw.Restart();
            GL.UseProgram(Core.game.shader.S1_shaderProgramHandle);
            BindBuffers();
            Core.game.shader.SetAttributesShaderOne(tGrass, tRock, tSand, tGrassRock, tDirt);  
            GL.DrawElements(PrimitiveType.Triangles, iTriangleCount, DrawElementsType.UnsignedShort, 0);
            sw.Stop();
        }
    }
}
