using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RaceGame
{
    class Server : InputListener, GameStateObject
    {
        InputState inputState = new InputState();
        GameState gameState = new GameState();

        const float ACC_MUL = 70;
        const float BREAK_MUL = 300;
        const float STEER_MUL = 40;

        bool running = true;

        List<GameStateListener> listeners = new List<GameStateListener>();

        public void setInputState(InputState state)
        {
            inputState = state;
        }

        void update(TimeSpan time)
        {
            /*if (inputState.steer == 0)
            {
                gameState.steer -= Math.Sign(gameState.steer) * Math.Min(Math.Max((float)time.TotalSeconds * STEER_MUL, (float)time.TotalSeconds), Math.Abs(gameState.steer));
            }*/

            if (inputState.acceleration == 0 && inputState.breakVal == 0)
            {
                gameState.speed -= Math.Sign(gameState.speed) * Math.Min(Math.Max((float)time.TotalSeconds * ACC_MUL, (float)time.TotalSeconds), Math.Abs(gameState.speed));
            }

            gameState.speed += inputState.acceleration * (float)time.TotalSeconds * ACC_MUL;
            gameState.speed -= inputState.breakVal * (float)time.TotalSeconds * BREAK_MUL;
            //gameState.steer += inputState.steer * (float)time.TotalSeconds * STEER_MUL;
            gameState.steer = inputState.steer;

            gameState.update(time);

            foreach (GameStateListener listener in listeners)
            {
                listener.setGameState(gameState);
            }
        }

        public void run()
        {
            while (!Game1.isReady)
            {
                System.Threading.Thread.Sleep(100);
            }
            
            DateTime time = DateTime.Now;
            while (running)
            {
                DateTime newTime = DateTime.Now;
                update(newTime - time);
                time = newTime;
                System.Threading.Thread.Sleep(30);
            }
        }

        public void stop()
        {
            running = false;
        }

        public void addListener(GameStateListener listener)
        {
            listeners.Add(listener);
        }
    }
}
