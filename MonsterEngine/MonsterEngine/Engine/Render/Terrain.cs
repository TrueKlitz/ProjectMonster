using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Diagnostics;

namespace MonsterEngine.Engine.Render
{
    class Chunk
    {
        private Vector3[] v3Vertices;
        private Vector3[] v3RefVertices;
        private Vector3[,] v3RefNormals;
        private Vector3[] v3Normals;
        private Vector2[] v2TexCoord;
        private float fPositionX, fPositionZ;

        private int iVBO, iIBO, iTBO, iNBO;
        private int iTriangleCount = 0;

        private int iCurrentSize;
        private int iSize;
        private float fScale;
        
        private float LOD;

        public Chunk(ref Vector3[] refVertices, ref Vector3[,] _Normals , float _PositionX, float _PositionZ, int _Size)
        {
            v3RefVertices = refVertices;
            LOD = 1;
            iSize = _Size;

            fPositionX = _PositionX;
            fPositionZ = _PositionZ;

            v3RefNormals = _Normals;

            CreateBuffers();
            BindBuffers();
        }
        public void Update()
        {
            float lastLOD = LOD;
            Vector3 camPos = -Core.game.camera.vPosition;
            float distance = Helper.Vec3Lenght(new Vector3(camPos.X - (fPositionX + (fScale / 2)), camPos.Y, camPos.Z - ( fPositionZ + (fScale / 2))));

            if (distance >= 24 * Core.drawDistance) { LOD = 8; }
            if (distance < 24 * Core.drawDistance) { LOD = 7; }
            if (distance <= 21 * Core.drawDistance) { LOD = 6; }
            if (distance <= 18 * Core.drawDistance) { LOD = 5; }
            if (distance <= 15 * Core.drawDistance) { LOD = 4; }
            if (distance <= 12 * Core.drawDistance) { LOD = 3; }
            if (distance <= 9 * Core.drawDistance) { LOD = 2; } // 2 ist nicht notwendig, da LOD = 2 genau so aussieht wie LOD = 1
            if( distance <= 6 * Core.drawDistance){LOD = 1;}

            if (lastLOD != LOD)
            {
                CreateBuffers();
                BindBuffers();
            }
        }

        private void CreateBuffers()
        {
            iCurrentSize = (int)(iSize / LOD);
            iTriangleCount = ((iCurrentSize - 1) * 6) * (iCurrentSize - 1);
            v3Vertices = new Vector3[iCurrentSize * iCurrentSize];
            v2TexCoord = new Vector2[iCurrentSize * iCurrentSize];
            v3Normals = new Vector3[iCurrentSize * iCurrentSize];

            fScale = 32.0f;

            for (int x = 0; x < iCurrentSize; x++)
            {
                for (int z = 0; z < iCurrentSize; z++)
                {
                    int iX = (int)(((fScale / ((iCurrentSize - 1) * 1.0f)) * x) + fPositionX);
                    int iZ = (int)(((fScale / ((iCurrentSize - 1) * 1.0f)) * z) + fPositionZ);
                    v3Vertices[x * iCurrentSize + z] = v3RefVertices[(int)(iX * Math.Sqrt(v3RefVertices.Length) + iZ)];
                    v2TexCoord[x * iCurrentSize + z] = new Vector2(iX / 5f, iZ / 5f);
                    v3Normals[x * iCurrentSize + z] = v3RefNormals[iX, (int)iZ];

                }
            }

            short[] indices = new short[iCurrentSize * iCurrentSize * 6];
            
            int index = 0;

            for (int x = 0; x < iCurrentSize; x++)
            {
                for (int z = 0; z < iCurrentSize - 1; z++)
                {
                    int offset = x * iCurrentSize + z;
                    indices[index] = (short)(offset + 0);
                    indices[index + 1] = (short)(offset + 1);
                    indices[index + 2] = (short)(offset + iCurrentSize);
                    indices[index + 3] = (short)(offset + 1);
                    indices[index + 4] = (short)(offset + iCurrentSize + 1);
                    indices[index + 5] = (short)(offset + iCurrentSize);
                    index += 6;
                }
            }

            //Generate TexCoordinates
            GL.GenBuffers(1, out iTBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iTBO);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer,
                                   new IntPtr(v2TexCoord.Length * Vector2.SizeInBytes),
                                   v2TexCoord, BufferUsageHint.StaticDraw);

            //Generate Normals
            GL.GenBuffers(1, out iNBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iNBO);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(v3Normals.Length * Vector3.SizeInBytes),
                                   v3Normals, BufferUsageHint.StaticDraw);

            //Generate Vertices
            GL.GenBuffers(1, out iVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, iVBO);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(v3Vertices.Length * Vector3.SizeInBytes),
                                   v3Vertices, BufferUsageHint.StaticDraw);

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
            GL.UseProgram(Core.game.shader.S1_shaderProgramHandle);
            BindBuffers();
            GL.DrawElements(PrimitiveType.Triangles, iTriangleCount, DrawElementsType.UnsignedShort, 0);
        }
    }
    class Terrain
    {
        private int iSize;
        private Chunk[,] chunk;
        private Vector3[] v3Vertices;
        private Vector3[,] normals;
        private Heightmap heightmap;

        public Terrain(Heightmap _heightmap , int _size)
        {
            iSize = _size;
            chunk = new Chunk[_size,_size];
            heightmap = _heightmap;

            int totalSize = _size * 64;
            normals = new Vector3[totalSize,totalSize];
 
            for (int x = 0; x < totalSize; x++)
            {
                for (int z = 0; z < totalSize; z++)
                {
                    if (x >= 1 && z >= 1 && x < totalSize && z < totalSize)
                    {
                        Vector3 u = new Vector3(1.0f, 0.0f, heightmap.getHeight(x+1,z) - heightmap.getHeight(x-1,z));
                        Vector3 v = new Vector3(0.0f, 1.0f, heightmap.getHeight(x, z + 1) - heightmap.getHeight(x, z - 1));
                        normals[x,z] = Vector3.Normalize(Vector3.Cross(u, v));
                    }
                }
            }
            v3Vertices = new Vector3[ totalSize * totalSize ];
            for (int x = 0; x < totalSize; x++)
            {
                for (int z = 0; z < totalSize; z++)
                {
                    v3Vertices[x * totalSize + z] = 
                        new Vector3(x,heightmap.getHeight(x,z),z);
                }
            }
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    chunk[x, y] = new Chunk(ref v3Vertices, ref normals, x * 32.0f, y * 32.0f, 64);
                }
            }
        }

        public void Update()
        {

            for (int x = 0; x < iSize; x++)
            {
                for (int y = 0; y < iSize; y++)
                {
                    chunk[x, y].Update();
                }
            }

        }

        public void Draw()
        {
            for (int x = 0; x < iSize; x++)
            {
                for (int y = 0; y < iSize; y++)
                {
                    chunk[x, y].Draw() ;
                }
            }
        }
    }
}
