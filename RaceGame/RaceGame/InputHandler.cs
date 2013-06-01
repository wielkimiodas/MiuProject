using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RaceGame
{
    class InputHandler : InputObject
    {
        List<InputListener> listeners = new List<InputListener>();

        InputState state = new InputState();

        Game1 game;
        bool running;

        public InputHandler(Game1 game)
        {
            this.game = game;
            running = true;
        }

        public void addListener(InputListener listener)
        {
            listeners.Add(listener);
        }

        public void update(TimeSpan time)
        {
            if (game.up)
            {
                //state.acceleration += accSpeed * (float)time.TotalSeconds;
                state.acceleration += 1;
            }
            else
            {
                state.acceleration = 0;
            }

            if (game.down)
            {
                state.breakVal = 1;
            }
            else
            {
                state.breakVal = 0;
            }

            if (game.left)
            {
                state.steer -= 1;
            }
            else if (game.right)
            {
                state.steer += 1;
            }
            else
            {
                state.steer = 0;
            }


            state.acceleration = MathHelper.Clamp(state.acceleration, 0, 1);
            state.breakVal = MathHelper.Clamp(state.breakVal, 0, 1);
            state.steer = MathHelper.Clamp(state.steer, -1, 1);

            foreach (InputListener listener in listeners)
            {
                listener.setInputState(state);
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
    }
}
