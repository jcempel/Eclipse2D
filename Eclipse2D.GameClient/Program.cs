using System;
using Eclipse2D;

namespace Eclipse2D.GameClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (EclipseGame Game = new EclipseGame("Eclipse Game", 1024, 768))
            {
                Game.Run();
            }
        }
    }
}
