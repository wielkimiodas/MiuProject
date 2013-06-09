using System;

namespace RaceGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                Server.GameServer s= new Server.GameServer();
                string r = s.LaunchServerInstance();
                Console.WriteLine(r);
                game.Run();
            }
        }
    }
#endif
}

