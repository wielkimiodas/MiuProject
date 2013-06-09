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
        public Car car1;
        public Car car2;
        PhysicObj[] cones;
        PhysicObj[] tires;

        List<DrawableObject> objects = new List<DrawableObject>();

        Texture2D carTexture1, carTexture2, coneTexture, tireTexture, blockTexture;
        public static Texture2D lineTexture;

        Texture2D trackTex;

        SpriteFont Font1;

        Camera camera1, camera2;

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
            GraphicsDevice.RasterizerState = new RasterizerState()
            {
                ScissorTestEnable = true
            };

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

            carTexture1 = Content.Load<Texture2D>("car1");
            carTexture2 = Content.Load<Texture2D>("car2");
            lineTexture = Content.Load<Texture2D>("line");
            coneTexture = Content.Load<Texture2D>("cone");
            tireTexture = Content.Load<Texture2D>("tire");
            blockTexture = Content.Load<Texture2D>("block");
            trackTex = Content.Load<Texture2D>("track");

            world = new World(Vector2.Zero);
            
            car1 = new Car(world, carTexture1, new Vector2(0.1f, 0.1f), 1);
            car1.Restitution = 0.1f;
            car1.Rotation = (float)Math.PI / 2;
            car1.level = 0.1f;
            objects.Add(car1);

            car2 = new Car(world, carTexture2, new Vector2(0.1f, 0.1f), 1);
            car2.Restitution = 0.1f;
            car2.Rotation = (float)Math.PI / 2;
            car2.Position = new Vector2(-100, 0);
            car2.level = 0.1f;
            objects.Add(car2);

            cones = new PhysicObj[10];
            for (int i = 1; i <= cones.Length; i++)
            {
                cones[i - 1] = new PhysicObj(world, coneTexture, new Vector2(0.2f, 0.2f), 0.8f);
                cones[i - 1].Position = new Vector2(75 * i, 0);
                cones[i - 1].Restitution = 0.1f;
                cones[i - 1].body.LinearDamping = 5.0f;
                objects.Add(cones[i - 1]);
            }

            tires = new PhysicObj[10];
            for (int i = 1; i <= tires.Length; i++)
            {
                tires[i - 1] = new PhysicObj(world, tireTexture, new Vector2(0.5f, 0.5f), 1);
                tires[i - 1].Position = new Vector2(75 * i, 100);
                tires[i - 1].Restitution = 0.8f;
                tires[i - 1].body.LinearDamping = 8.0f;
                objects.Add(tires[i - 1]);
            }

            for (int i = 0; i < 100; i++)
            {
                PhysicObj obj = new PhysicObj(world, blockTexture, new Vector2(10, 10), 1);
                obj.Position = new Vector2(-100 - i * 100, -100);
                obj.body.BodyType = BodyType.Static;
                obj.body.Restitution = 0.0f;
                objects.Add(obj);
            }

            DrawableObject o = new DrawableObject(trackTex, Vector2.One, Vector2.Zero, 0);
            objects.Add(o);

            /*for (int i = 0; i < 10; i++)
            {
                Body body = FarseerPhysics.Factories.BodyFactory.CreateCircle(world, 1, 1);
                body.Position = new Vector2(50*(i+1), 0);
                body.BodyType = BodyType.Dynamic;
            }*/

            List<Car> cars = new List<Car>();
            cars.Add(car1);
            camera1 = new Camera(cars, graphics, GraphicsDevice, new Rectangle(0, 0, graphics.PreferredBackBufferWidth/2,graphics.PreferredBackBufferHeight));

            cars = new List<Car>();
            cars.Add(car2);
            camera2 = new Camera(cars, graphics, GraphicsDevice, new Rectangle(graphics.PreferredBackBufferWidth/2, 0, graphics.PreferredBackBufferWidth/2,graphics.PreferredBackBufferHeight));

            Font1 = Content.Load<SpriteFont>("Font1");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            carTexture1.Dispose();
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
            car2.inputState.acceleration = keyboard.IsKeyDown(Keys.Up) ? 1 : 0;
            car2.inputState.breakVal = keyboard.IsKeyDown(Keys.Down) ? 1 : 0;
            car2.inputState.steer = keyboard.IsKeyDown(Keys.Left) ? -1 : 0;
            car2.inputState.steer += keyboard.IsKeyDown(Keys.Right) ? 1 : 0;
            if (keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            car1.update(gameTime.ElapsedGameTime);
            car2.update(gameTime.ElapsedGameTime);
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            camera1.Update(gameTime.ElapsedGameTime);
            camera2.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //DrawableObj target = car2;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            //spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            const int N = 5;
            /*for (int i = 0; i <= N; i++)
            {
                int size = graphics.PreferredBackBufferWidth / N;
                spriteBatch.Draw(lineTexture, new Vector2(i * size - (int)car.Position.X % size, 0), null, Color.White, MathHelper.PiOver2, Vector2.Zero, new Vector2(graphics.PreferredBackBufferWidth, 1), SpriteEffects.None, 0.1f);
                spriteBatch.Draw(lineTexture, new Vector2(0, i * size - (int)car.Position.Y % size), null, Color.White, 0, Vector2.Zero, new Vector2(graphics.PreferredBackBufferWidth, 1), SpriteEffects.None, 0.1f);
            }*/

            /*spriteBatch.Draw(car1.texture, (car1.Position - target.Position) + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, car1.Rotation, car1.origin, car1.scale, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(car2.texture, (car2.Position - target.Position) + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, car2.Rotation, car2.origin, car2.scale, SpriteEffects.None, 0.0f);


            for (int i = 0; i < cones.Length; i++)
            {
                DrawableObj el = cones[i];
                spriteBatch.Draw(el.texture, (el.Position - target.Position) + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, el.Rotation, el.origin, el.scale, SpriteEffects.None, 0.0f);
            }*/

            /*foreach(DrawableObj obj in objects)
                spriteBatch.Draw(obj.texture, (obj.Position - target.Position) + new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, obj.Rotation, obj.origin, obj.scale, SpriteEffects.None, obj.level);*/

            camera1.Draw(spriteBatch, objects);
            camera2.Draw(spriteBatch, objects);

            /*spriteBatch.DrawString(Font1, target.LinearVelocity.Length().ToString("0.00"), new Vector2(0, 0), Color.White);
            //spriteBatch.DrawString(Font1, car.lastSideSpeed.ToString("0.00"), new Vector2(100, 0), Color.White);
            spriteBatch.DrawString(Font1, (target.Position).X.ToString("0.00"), new Vector2(0, 50), Color.White);
            spriteBatch.DrawString(Font1, (target.Position).Y.ToString("0.00"), new Vector2(100, 50), Color.White);*/

            /*car.position.X = graphics.PreferredBackBufferWidth / 2;
            car.position.Y = graphics.PreferredBackBufferHeight / 2;
            car.rotation = gameState.rotation;
            car.draw(spriteBatch);*/
            //spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
