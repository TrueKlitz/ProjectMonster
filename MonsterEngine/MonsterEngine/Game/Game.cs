using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonsterEngine.Engine;
using MonsterEngine.Engine.Render;

namespace MonsterEngine.Game
{
    class Game
    {
        private Level level;
        private Core EngineCore;

        public Terrain terrain;

        public Game(Core _core)
        {
            EngineCore = _core;
            level = new Level(256, "Test1");
            Load();
        }

        public void Load()
        {
            level.Load();
            terrain = new Terrain(level.faHeightmap , level.faHeightMapNormalGen, 0.0f,0.0f);
            level.disposeData();
        }

        public void Draw()
        {
            terrain.Draw();
        }

    }
}
