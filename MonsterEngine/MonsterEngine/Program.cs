using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using MonsterEngine.Engine;

namespace MonsterEngine
{
    class Program
    {
        [STAThread]
        public static void Main()
        {
            Core core = new Core();
            using (var game = new GameWindow(core.WIDTH,core.HEIGHT, new GraphicsMode(32, 24, 0, core.MSAA)))
            {
                game.Load += (sender, e) =>
                {
                    core.load(game);
                };

                game.Resize += (sender, e) =>
                {
                    core.resize();
                };

                game.UpdateFrame += (sender, e) =>
                {
                    core.update();
                };

                game.RenderFrame += (sender, e) =>
                {
                    core.draw();
                };
                // Run the game at 60 updates per second
                game.Run(60.0);
            }
        }
    }
}
