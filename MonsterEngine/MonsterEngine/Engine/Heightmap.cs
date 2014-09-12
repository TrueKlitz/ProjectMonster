using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonsterEngine.Engine
{
    class Heightmap
    {
        public float[,] heightmap;
        private int SIZE;
        public Heightmap(int size)
        {
            SIZE = size;
            heightmap = new float[size,size];
        }

        public float getHeight(int x, int y){
            if (x < SIZE && y < SIZE && x >= 0 && y >= 0)
            {
                return heightmap[x,y];
            }
            else
            {
                return 0;
            }
        }

        public void setHeight(int x, int y, float height)
        {
            if (x < SIZE && y < SIZE)
            {
                if (height <= 0)
                {
                    heightmap[x, y] = 0;
                }
                heightmap[x,y] = height;
            }
        }
        public float[,] getHeight2d()
        {
            float[,] result = new float[SIZE, SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    result[i, j] = heightmap[i,j];
                }
            }
            return result;
        }
        public float[,] getHeight2d(int x , int y , int width, int height)
        {
            float[,] result = new float[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[i, j] = heightmap[(i+x),(j+y)];
                }
            }
            return result;
        }

    }
}
