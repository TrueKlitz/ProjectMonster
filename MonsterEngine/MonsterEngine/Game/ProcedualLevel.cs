using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using MonsterEngine.Engine.Render;
using MonsterEngine.Engine;
using System.Diagnostics;

namespace MonsterEngine.Game
{
    class ProcedualLevel
    {
        String sSeed;
        Random random;

        private int iSize;
        private Heightmap heightmap;
        private Terrain t;

        public int MapSize()
        {
            return iSize;
        }

        public ProcedualLevel()
        {
            Load();
        }

        public void disposeData()
        {
        }

        public void Load()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Starting loading level");

            iSize = 16;
            sSeed = "32";
            random = new Random(sSeed.GetHashCode());
            heightmap = new Heightmap(iSize * 64); // * 64 da jeder chunk 64 einheiten besitz 

            for (int i = 0; i < iSize*64; i++)
            {
                for (int j = 0; j < iSize*64; j++)
                {
                    heightmap.setHeight(i, j, (float) ((Noise.Generate(i / 64.0f, j / 64.0f) * 10f + 4) + (Noise.Generate(i / 32.0f, j / 32.0f) * 10f + 4)) / 2.0f );
                }
            }

            t = new Terrain(heightmap, iSize);

            sw.Stop();
            Console.WriteLine("Level loaded in "  + sw.ElapsedMilliseconds + "ms");

        }

        public void Draw()
        {
            t.Update();
            t.Draw();
        }

    }
}
