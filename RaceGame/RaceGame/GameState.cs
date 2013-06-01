using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Common.Decomposition;

namespace RaceGame
{
    public class GameObject
    {
        public Body body { get; set; }
        Vector2 origin;
        float scale = 1f;

        public GameObject(Texture2D texture)
        {
            uint[] data = new uint[texture.Width * texture.Height];
            texture.GetData(data);
            Vertices textureVertices = PolygonTools.CreatePolygon(data, texture.Width, false);
            Vector2 centroid = -textureVertices.GetCentroid();
            textureVertices.Translate(ref centroid);

            origin = -centroid;

            textureVertices = SimplifyTools.ReduceByDistance(textureVertices, 4f);

            List<Vertices> list = BayazitDecomposer.ConvexPartition(textureVertices);

            /*Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(1)) * scale;
            foreach (Vertices vertices in list)
            {
                vertices.Scale(ref vertScale);
            }

            //Create a single body with multiple fixtures
            _compound = BodyFactory.CreateCompoundPolygon(World, list, 1f, BodyType.Dynamic);
            _compound.BodyType = BodyType.Dynamic;*/
        }
    }

    public class GameState
    {
        List<GameObject> objects = new List<GameObject>();
        List<Tuple<Int32, InputState>> players = new List<Tuple<int, InputState>>();

        public Vector2 position;
        public float rotation;
        public float steer;
        public float speed;

        public GameState()
        {
            position = new Vector2();
            rotation = 0;
            speed = 0;
        }

        public GameState(GameState state)
        {
            position = new Vector2(state.position.X, state.position.Y);
            rotation = state.rotation;
            speed = state.speed;
            steer = state.steer;
        }

        public void update(TimeSpan time)
        {
            position.X += (float)(Math.Sin(-rotation) * time.TotalSeconds * speed);
            position.Y += (float)(Math.Cos(-rotation) * time.TotalSeconds * speed);

            rotation += (float)(steer * time.TotalMilliseconds * speed * 0.000001);
        }

        public void addObject(GameObject obj)
        {
            objects.Add(obj);
        }

        public void addPlayer(InputState playerState, GameObject obj)
        {
            int n = 0;
            lock (objects)
            {
                n = objects.Count;
                objects.Add(obj);
            }
            players.Add(new Tuple<int,InputState>(n, playerState));
        }
    }

    interface GameStateListener
    {
        void setGameState(GameState state);
    }

    interface GameStateObject
    {
        void addListener(GameStateListener listener);
    }

    class GameStateSender : GameStateListener, GameStateObject
    {
        const int DELAY = 100;

        List<GameStateListener> listeners = new List<GameStateListener>();

        public GameStateSender()
        {
        }

        void send(GameState state)
        {
            GameState newState = new GameState(state);

            int r = new Random().Next();
            System.Threading.Thread.Sleep(DELAY);

            foreach (GameStateListener listener in listeners)
            {
                listener.setGameState(newState);
            }
        }

        public void setGameState(GameState state)
        {
            Action a = new Action(() => send(state));
            a.BeginInvoke(null, null);
        }

        public void addListener(GameStateListener listener)
        {
            listeners.Add(listener);
        }
    }
}
