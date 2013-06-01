using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaceGame
{
    public class InputState
    {
        public float acceleration;
        public float breakVal;
        public float steer;

        public InputState()
        {
            acceleration = 0;
            breakVal = 0;
            steer = 0;
        }

        public InputState(InputState state)
        {
            acceleration = state.acceleration;
            this.breakVal = state.breakVal;
            this.steer = state.steer;
        }
    }

    interface InputListener
    {
        void setInputState(InputState state);
    }

    interface InputObject
    {
        void addListener(InputListener listener);
    }

    class InputSender : InputListener, InputObject
    {
        const int DELAY = 100;

        List<InputListener> listeners = new List<InputListener>();

        public InputSender()
        {
        }

        void send(InputState state)
        {
            InputState newState = new InputState(state);

            int r = new Random().Next();
            System.Threading.Thread.Sleep(DELAY);

            foreach (InputListener listener in listeners)
            {
                listener.setInputState(newState);
            }
        }

        public void setInputState(InputState state)
        {
            Action a = new Action(() => send(state));
            a.BeginInvoke(null, null);
        }

        public void addListener(InputListener listener)
        {
            listeners.Add(listener);
        }
    }
}
