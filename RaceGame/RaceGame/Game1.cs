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
using FarseerPhysics.Dynamics;

namespace RaceGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        World world;
        Car car;
        DrawableObj[] elements;

        Texture2D carTexture, lineTexture, elementsTexture;

        SpriteFont Font1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            carTexture = Content.Load<Texture2D>("car");
            lineTexture = Content.Load<Texture2D>("line");
            elementsTexture = Content.Load<Texture2D>("image");

            world = new World(Vector2.Zero);
            car = new Car(world, carTexture, new Vector2(0.1f, 0.1f), 1);
            car.Restitution = 0.1f;

            elements = new DrawableObj[10];
            for (int i = 1; i <= elements.Length; i++)
            {
                elements[i - 1] = new DrawableObj(world, elementsTexture, new Vector2(0.51f, 0.51f), 1);
                elements[i - 1].Position = new Vector2(75 * i, 0);
                elements[i - 1].Restitution = 0.1f;
            }

            /*for (int i = 0; i < 10; i++)
            {
                Body body = FarseerPhysics.Factories.BodyFactory.CreateCircle(world, 1, 1);
                body.Position = new Vector2(50*(i+1), 0);
                body.BodyType = BodyType.Dynamic;
            }*/

            Font1 = Content.Load<SpriteFont>("Font1");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            carTexture.Dispose();
            lineTexture.Dispose();
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
            car.inputState.acceleration = keyboard.IsKeyDown(Keys.Up) ? 1 : 0;
            car.inputState.breakVal = keyboard.IsKeyDown(Keys.Down) ? 1 : 0;
            car.inputState.steer = keyboard.IsKeyDown(Keys.Left) ? -1 : 0;
            car.inputState.steer += keyboard.IsKeyDown(Keys.Right) ? 1 : 0;
            if (keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            car.update(gameTime.ElapsedGameTime);
            //Console.WriteLine("1:\t" + elements[0].Position + "\t" + car.Position);
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            //Console.WriteLine("2:\t" + elements[0].Position + "\t" + car.Position);

            //Console.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", car.inputState.acceleration, car.inputState.breakVal, car.inputState.steer, car.body.Position, car.body.LinearVelocity, car.body.AngularVelocity));

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            const int N = 5;
            /*for (int i = 0; i <= N; i++)
            {
                int size = graphics.PreferredBackBufferWidth / N;
                spriteBatch.Draw(lineTexture, new Vector2(i * size - (int)car.Position.X % size, 0), null, Color.White, MathHelper.PiOver2, Vector2.Zero, new Vector2(graphics.PreferredBackBufferWidth, 1), SpriteEffects.None, 0.1f);
                spriteBatch.Draw(lineTexture, new Vector2(0, i * size - (int)car.Position.Y % size), null, Color.White, 0, Vector2.Zero, new Vector2(graphics.PreferredBackBufferWidth, 1), SpriteEffects.None, 0.1f);
            }*/

            spriteBatch.Draw(car.texture, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, car.Rotation, car.origin, car.scale, SpriteEffects.None, 0.0f);

            for (int i = 0; i < elements.Length; i++)
            {
                DrawableObj el = elements[i];
                spriteBatch.Draw(el.texture, (el.Position - car.Position) + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, el.Rotation, el.origin, el.scale, SpriteEffects.None, 0.0f);
            }

            spriteBatch.DrawString(Font1, car.LinearVelocity.Length().ToString("0.00"), new Vector2(0, 0), Color.White);
            //spriteBatch.DrawString(Font1, car.lastSideSpeed.ToString("0.00"), new Vector2(100, 0), Color.White);
            spriteBatch.DrawString(Font1, (car.Position).X.ToString("0.00"), new Vector2(0, 50), Color.White);
            spriteBatch.DrawString(Font1, (car.Position).Y.ToString("0.00"), new Vector2(100, 50), Color.White);

            /*car.position.X = graphics.PreferredBackBufferWidth / 2;
            car.position.Y = graphics.PreferredBackBufferHeight / 2;
            car.rotation = gameState.rotation;
            car.draw(spriteBatch);*/
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
