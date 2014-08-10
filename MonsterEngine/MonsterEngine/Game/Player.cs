using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonsterEngine.Engine.Render;
using OpenTK;

namespace MonsterEngine.Game
{
    class Player
    {
        GameModel playerModel;
        Vector3 vPosition;
        int team;

        public Player(ref GameModel player, Vector3 position, int team)
        {
            this.playerModel = player;
            vPosition = position;
            this.team = team;
        }
        public void Update()
        {

        }
        public void Draw()
        {

        }
    }
}
