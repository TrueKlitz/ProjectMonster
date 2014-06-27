using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonsterEngine
{
    class Level
    {
        String sSeed;

        private int iSize;
        private int iRndNumGen = 1;
        private int iOldRndNum = 1;
        public float[,] faHeightmap;

        public int MapSize()
        {
            return iSize;
        }

        public Level(int size_ ,String seed_)
        {
            iSize = size_;
            sSeed = seed_;
            faHeightmap = new float[iSize,iSize];
            GenerateHeighmap();
        }

        private void GenerateHeighmap()
        {
            float lSeed = (RandomNum(2147483646) + 1) / 2147483647.0f; 
            float mapInnerSize = 200.0f;
            for (int x = 0; x < iSize; x++)
            {
                for (int y = 0; y < iSize; y++)
                {
                    faHeightmap[x, y] = (float)Math.Floor(Noise.Generate(( x+lSeed ) / mapInnerSize, ( y+lSeed ) / mapInnerSize));
                }
            }
            mapInnerSize = mapInnerSize / (RandomNum(3) * 1.0f + 1.0f);
            for (int x = 0; x < iSize; x++)
            {
                for (int y = 0; y < iSize; y++)
                {      
                    faHeightmap[x, y] = faHeightmap[x, y] + ( (float)Math.Floor(Noise.Generate(( x+lSeed + iSize) / mapInnerSize, ( y+lSeed + iSize) / mapInnerSize)));
                }   
            }


            for (int i = 0; i < 15; i++)
            {
                for (int x = 1; x < iSize - 1; x++)
                {
                    for (int y = 1; y < iSize - 1; y++)
                    {
                        faHeightmap[x, y] =
                            (faHeightmap[x + 1, y] + faHeightmap[x - 1, y] + faHeightmap[x, y + 1] + faHeightmap[x, y - 1] +
                              faHeightmap[x + 1, y + 1] + faHeightmap[x - 1, y - 1] + faHeightmap[x - 1, y + 1] + faHeightmap[x + 1, y - 1]) / 8.0f;
                    }
                }
            }

            for (int x = 0; x < iSize; x++)
            {
                for (int y = 0; y < iSize; y++)
                {
                    faHeightmap[x, y] = faHeightmap[x, y] * (RandomNum(1000) / 900.0f + 0.5f);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                for (int x = 1; x < iSize - 1; x++)
                {
                    for (int y = 1; y < iSize - 1; y++)
                    {
                        faHeightmap[x, y] =
                            (faHeightmap[x + 1, y] + faHeightmap[x - 1, y] + faHeightmap[x, y + 1] + faHeightmap[x, y - 1] +
                              faHeightmap[x + 1, y + 1] + faHeightmap[x - 1, y - 1] + faHeightmap[x - 1, y + 1] + faHeightmap[x + 1, y - 1]) / 8.0f ;
                    }
                }
            }

            


            for (int x = 0; x < iSize; x++)
            {
                for (int y = 0; y < iSize; y++)
                {
                    if (x == 0) faHeightmap[x, y] = faHeightmap[x + 1, y];
                    if (y == 0) faHeightmap[x, y] = faHeightmap[x , y +1];
                    if (y == iSize - 1) faHeightmap[x, y] = faHeightmap[x, y - 1];
                    if (x == iSize - 1) faHeightmap[x, y] = faHeightmap[x - 1, y];
                }
            }
        }

        private int RandomNum(int range)
        {
            int rndNum = (int)(iOldRndNum + iRndNumGen * sSeed.GetHashCode() / 50123) % range;
            if (rndNum <= 0) rndNum = rndNum * -1;
            iRndNumGen++;
            iOldRndNum = rndNum;
            return rndNum;
        }
    }
}
