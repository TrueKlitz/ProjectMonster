using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using MonsterEngine.Engine.Render;

namespace MonsterEngine.Game
{
    class Level
    {
        String sSeed;
        Random random;

        private int iSize;
        public float[,] faHeightmap;
        public float[,] faHeightMapNormalGen;// = new float[iSize, iSize];

        public int MapSize()
        {
            return iSize;
        }

        public Level(int size_ ,String seed_)
        {
            iSize = size_;
            sSeed = seed_;
            faHeightmap = new float[iSize,iSize];
            faHeightMapNormalGen = new float[iSize, iSize];
            random = new Random(sSeed.GetHashCode());
        }

        public void disposeData()
        {
            faHeightMapNormalGen = null;
            faHeightmap = null;
        }

        public void Load()
        {

            float lSeed = (float)random.NextDouble();
            float mapInnerSize = 50.0f;

            for (int x = 0; x < iSize; x++)
            {
                for (int y = 0; y < iSize; y++)
                {
                    faHeightmap[x, y] += (float)Math.Floor(Noise.Generate(x / mapInnerSize + lSeed, y / mapInnerSize + lSeed ));
                }
            }

            mapInnerSize = 45.0f;
            lSeed = (float)random.NextDouble();
            for (int x = 0; x < iSize; x++)
            {
                for (int y = 0; y < iSize; y++)
                {
                    faHeightmap[x, y] = Math.Max(faHeightmap[x, y], (float)Math.Floor(Noise.Generate(x / mapInnerSize - lSeed, y / mapInnerSize - lSeed, (x+y)/mapInnerSize)));
                }   
            }

            mapInnerSize = 100.0f;
            lSeed =(float) random.NextDouble();
            for (int x = 0; x < iSize; x++)
            {
                for (int y = 0; y < iSize; y++)
                {
                    faHeightmap[x, y] += (float)Math.Floor(Noise.Generate(x / mapInnerSize - lSeed, y / mapInnerSize - lSeed, (x + y) / mapInnerSize));
                }
            }

            mapInnerSize = 90.0f;
            lSeed = (float)random.NextDouble();
            for (int x = 0; x < iSize; x++)
            {
                for (int y = 0; y < iSize; y++)
                {
                    faHeightmap[x, y] += (float)Math.Floor(Noise.Generate(x / mapInnerSize + lSeed, y / mapInnerSize + lSeed, (x + y) / mapInnerSize));
                }
            }

            // 0 , -1 , -2 sind die Werte, die bis jetzt generiert wurden.

            int iMergeRange = 8;

            for (int i = 0; i < 1; i++)
            {
                for (int x = 0; x < iSize; x++)
                {
                    for (int y = 01; y < iSize; y++)
                    {
                        int iNull = 0;
                        int iOne = 0;
                        int iTwo = 0;
                        int iThree = 0;

                        for (int xx = -iMergeRange; xx <= iMergeRange; xx++)
                        {
                            for (int yy = -iMergeRange; yy <= iMergeRange; yy++)
                            {
                                if (x + xx >= 0 && y + yy >= 0 && x + xx < iSize && y + yy < iSize)
                                {
                                    switch ((int)faHeightmap[x + xx, y + yy])
                                    {
                                        case 0:
                                            iNull++;
                                            break;
                                        case -1:
                                            iOne++;
                                            break;
                                        case -2:
                                            iTwo++;
                                            break;
                                        case -3:
                                            iThree++;
                                            break;
                                    }
                                }
                            }
                        }

                        faHeightMapNormalGen[x, y] = 1f;
                        if (iNull >= iOne && iNull >= iTwo && iNull >= iThree) faHeightMapNormalGen[x, y] = 0f;
                        if (iOne >= iNull && iOne >= iTwo && iOne >= iThree) faHeightMapNormalGen[x, y] = 1f;
                        if (iTwo >= iNull && iTwo >= iOne && iTwo >= iThree) faHeightMapNormalGen[x, y] = 2f;
                        if (iThree >= iNull && iThree >= iOne && iThree >= iTwo) faHeightMapNormalGen[x, y] = 3f;
                    }
                }

            }

            
            for (int x = 1; x < iSize - 1; x++)
            {
                for (int y = 1; y < iSize - 1; y++)
                {
                    faHeightmap[x, y] = faHeightMapNormalGen[x, y];
                }
            }
            for (int x = 1; x < iSize - 1; x++)
            {
                for (int y = 1; y < iSize - 1; y++)
                {
                    faHeightMapNormalGen[x, y] += (float)(random.NextDouble() * ( random.Next(3) * random.NextDouble()) );
                }
            }
            
            for (int i = 0; i < 3; i++)
            {
                for (int x = 1; x < iSize - 1; x++)
                {
                    for (int y = 1; y < iSize - 1; y++)
                    {
                        faHeightMapNormalGen[x, y] =
                           (faHeightMapNormalGen[x + 1, y] + faHeightMapNormalGen[x - 1, y] + faHeightMapNormalGen[x, y + 1] + faHeightMapNormalGen[x, y - 1] +
                            faHeightMapNormalGen[x + 1, y + 1] + faHeightMapNormalGen[x - 1, y - 1] + faHeightMapNormalGen[x - 1, y + 1] + faHeightMapNormalGen[x + 1, y - 1]) / 8.0f;
                    }
                }
            }

            for (int x = 1; x < iSize - 1; x++)
            {
                for (int y = 1; y < iSize - 1; y++)
                {
                    faHeightmap[x, y] = (faHeightmap[x, y]*4 + faHeightMapNormalGen[x, y]*10) / 14.0f;
                }
            }


            for (int x = 0; x < iSize; x++)
            {
                for (int y = 0; y < iSize; y++)
                {
                    int l_ms = iSize - 1;
                    faHeightmap[x,y] *= 1.0f;
                    faHeightmap[x, y] += 0.5f;
                    if (x == 0 | x == 1 | y == 0 | y == 1 | x == l_ms | x == l_ms - 1 | y == l_ms | y == l_ms - 1) faHeightmap[x, y] = 0.0f;
                }
            }

        }
    }
}
