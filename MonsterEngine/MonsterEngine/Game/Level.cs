using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using MonsterEngine.Engine.Render;
using System.Diagnostics;

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
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Starting loading level");
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
           float rampLenght = 10;
            //Gernerate Ramps
            for (int x = 10; x < iSize-10 ; x++)
            {
                for (int y = 10; y < iSize-10 ; y++)
                {
                    if (random.Next(4) == 0)
                    {
                        // right
                        float height = faHeightmap[x, y];
                        if (height == faHeightmap[x + 1, y] && height == faHeightmap[x - 1, y] && height == 2)
                        {
                            if (height != faHeightmap[x, y + 1] && height != faHeightmap[x - 1, y + 1] && height != faHeightmap[x + 1, y + 1] && faHeightmap[x, y + 1] == height - 1)
                            {
                                int correction = 0;
                                for (int xx = -5; xx < 5; xx++)
                                {
                                    for (int yy = -5; yy < 5; yy++)
                                    {
                                        if (faHeightmap[x + xx, y + yy] == height | faHeightmap[x + xx, y + yy] == height - 1)
                                        {
                                            correction++;
                                        }
                                    }
                                }
                                if (correction >= 100)
                                {
                                    for (int i = 0; i < rampLenght; i++)
                                    {
                                        faHeightmap[x - 3, y + i] = height - (i / rampLenght);
                                        faHeightmap[x - 2, y + i] = height - (i / rampLenght);
                                        faHeightmap[x - 1, y + i] = height - (i / rampLenght);
                                        faHeightmap[x, y + i] = height - (i / rampLenght);
                                        faHeightmap[x + 1, y + i] = height - (i / rampLenght);
                                        faHeightmap[x + 2, y + i] = height - (i / rampLenght);
                                        faHeightmap[x + 3, y + i] = height - (i / rampLenght);
                                    }
                                }
                            }
                        }

                        //left
                        if (height == faHeightmap[x + 1, y] && height == faHeightmap[x - 1, y] && height == 2)
                        {
                            if (height != faHeightmap[x, y - 1] && height != faHeightmap[x - 1, y - 1] && height != faHeightmap[x + 1, y - 1] && faHeightmap[x, y - 1] == height - 1)
                            {
                                int correction = 0;
                                for (int xx = -5; xx < 5; xx++)
                                {
                                    for (int yy = -5; yy < 5; yy++)
                                    {
                                        if (faHeightmap[x + xx, y + yy] == height | faHeightmap[x + xx, y + yy] == height - 1)
                                        {
                                            correction++;
                                        }
                                    }
                                }
                                if (correction >= 100)
                                {
                                    for (int i = 0; i < rampLenght; i++)
                                    {
                                        faHeightmap[x - 3, y - i] = height - (i / rampLenght);
                                        faHeightmap[x - 2, y - i] = height - (i / rampLenght);
                                        faHeightmap[x - 1, y - i] = height - (i / rampLenght);
                                        faHeightmap[x, y - i] = height - (i / rampLenght);
                                        faHeightmap[x + 1, y - i] = height - (i / rampLenght);
                                        faHeightmap[x + 2, y - i] = height - (i / rampLenght);
                                        faHeightmap[x + 3, y - i] = height - (i / rampLenght);
                                    }
                                }
                            }
                        }

                        //up
                        if (height == faHeightmap[x, y + 1] && height == faHeightmap[x, y - 1] && height == 2)
                        {
                            if (height != faHeightmap[x - 1, y] && height != faHeightmap[x - 1, y - 1] && height != faHeightmap[x - 1, y + 1] && faHeightmap[x - 1, y] == height - 1)
                            {
                                int correction = 0;
                                for (int xx = -5; xx < 5; xx++)
                                {
                                    for (int yy = -5; yy < 5; yy++)
                                    {
                                        if (faHeightmap[x + xx, y + yy] == height | faHeightmap[x + xx, y + yy] == height - 1)
                                        {
                                            correction++;
                                        }
                                    }
                                }
                                if (correction >= 100)
                                {
                                    for (int i = 0; i < rampLenght; i++)
                                    {
                                        faHeightmap[x - i, y - 3] = height - (i / rampLenght);
                                        faHeightmap[x - i, y - 2] = height - (i / rampLenght);
                                        faHeightmap[x - i, y - 1] = height - (i / rampLenght);
                                        faHeightmap[x - i, y] = height - (i / rampLenght);
                                        faHeightmap[x - i, y + 1] = height - (i / rampLenght);
                                        faHeightmap[x - i, y + 2] = height - (i / rampLenght);
                                        faHeightmap[x - i, y + 3] = height - (i / rampLenght);
                                    }
                                }
                            }
                        }
                        //down
                        if (height == faHeightmap[x, y + 1] && height == faHeightmap[x, y - 1] && height == 2)
                        {
                            if (height != faHeightmap[x + 1, y] && height != faHeightmap[x + 1, y - 1] && height != faHeightmap[x + 1, y + 1] && faHeightmap[x + 1, y] == height - 1)
                            {
                                int correction = 0;
                                for (int xx = -5; xx < 5; xx++)
                                {
                                    for (int yy = -5; yy < 5; yy++)
                                    {
                                        if (faHeightmap[x + xx, y + yy] == height | faHeightmap[x + xx, y + yy] == height - 1)
                                        {
                                            correction++;
                                        }
                                    }
                                }
                                if (correction >= 100)
                                {
                                    for (int i = 0; i < rampLenght; i++)
                                    {
                                        faHeightmap[x + i, y - 3] = height - (i / rampLenght);
                                        faHeightmap[x + i, y - 2] = height - (i / rampLenght);
                                        faHeightmap[x + i, y - 1] = height - (i / rampLenght);
                                        faHeightmap[x + i, y] = height - (i / rampLenght);
                                        faHeightmap[x + i, y + 1] = height - (i / rampLenght);
                                        faHeightmap[x + i, y + 2] = height - (i / rampLenght);
                                        faHeightmap[x + i, y + 3] = height - (i / rampLenght);
                                    }
                                }
                            }
                        }
                    }
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
                    faHeightmap[x, y] = (faHeightmap[x, y]*6 + faHeightMapNormalGen[x, y]*4) / 10.0f;
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

            sw.Stop();
            Console.WriteLine("Level loaded in "  + sw.ElapsedMilliseconds + "ms");

        }

    }
}
