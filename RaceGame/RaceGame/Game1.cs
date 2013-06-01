using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace RaceGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game, GameStateListener
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Sprite car;
        Sprite line;
        
        public static bool isReady = false;
        public bool up, down, left, right;

        GameState gameState = new GameState();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            //graphics.ToggleFullScreen();
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            car = new Sprite(Content.Load<Texture2D>("car"), new Vector2(0.2f, 0.2f), 0);
            car.position = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            line = new Sprite(Content.Load<Texture2D>("line"), new Vector2(graphics.PreferredBackBufferWidth, 1), 0.1f);

            isReady = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState keyboard = Keyboard.GetState();
            up = keyboard.IsKeyDown(Keys.Up);
            down = keyboard.IsKeyDown(Keys.Down);
            left = keyboard.IsKeyDown(Keys.Left);
            right = keyboard.IsKeyDown(Keys.Right);
            if (keyboard.IsKeyDown(Keys.Escape))
                this.Exit();
            
            gameState.update(new TimeSpan((long)gameTime.ElapsedGameTime.TotalMilliseconds));
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            const int N = 5;
            for (int i = 0; i <= N; i++)
            {
                int size = graphics.PreferredBackBufferWidth / N;
                line.rotation = 0;
                line.position.X = i * size - (int)gameState.position.X % size;
                line.position.Y = 0;
                line.draw(spriteBatch);
                line.rotation = -MathHelper.PiOver2;
                line.position.X = 0;
                line.position.Y = i * size - (int)gameState.position.Y % size;
                line.draw(spriteBatch);

            }

            car.position.X = graphics.PreferredBackBufferWidth / 2;
            car.position.Y = graphics.PreferredBackBufferHeight / 2;
            car.rotation = gameState.rotation;
            car.draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public KeyboardState getKeyboardState()
        {
            return Keyboard.GetState();
        }

        public void setGameState(GameState state)
        {
            /*inputState.acceleration = state.acceleration;
            inputState.breakVal = state.breakVal;
            inputState.steer = state.steer;*/
            gameState = state;
        }
    }

    public class Sprite
    {
        public Vector2 position;
        public Vector2 scale;
        public float rotation;
        public Texture2D texture;
        float height;

        public Sprite(Texture2D tex, Vector2 scale, float height)
        {
            position = new Vector2();
            this.scale = scale;
            rotation = 0;
            texture = tex;
            this.height = height;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            spriteBatch.Draw(texture, position, null, Color.White, rotation+MathHelper.PiOver2, origin, scale, SpriteEffects.None, height);
            //spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, 100, 200), null, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);
        }
    }
}
