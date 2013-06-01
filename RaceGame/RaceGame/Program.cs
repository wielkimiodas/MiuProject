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
            System.Threading.ThreadPool.SetMinThreads(128, 128);

            using (Game1 game = new Game1())
            {
                InputHandler handler = new InputHandler(game);
                InputSender inputSender = new InputSender();
                GameStateSender gameStateSender = new GameStateSender();
                Server server = new Server();
                inputSender.addListener(server);
                server.addListener(gameStateSender);
                gameStateSender.addListener(game);
                handler.addListener(inputSender);
                Action a = new Action(() => handler.run());
                a.BeginInvoke(null, null);
                a = new Action(() => server.run());
                a.BeginInvoke(null, null);
                game.Run();
                handler.stop();
                server.stop();
            }
        }
    }
#endif
}

