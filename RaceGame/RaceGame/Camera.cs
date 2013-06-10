using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RaceGame
{
    class Camera
    {
        List<Car> cars;
        GraphicsDeviceManager graphics;
        GraphicsDevice graphicsDevice;
        Vector2 center = Vector2.Zero;
        float zoom = 1f;
        public Rectangle rectangle;
        
        const float visibleCircle = 100;
        const float velocityMul = 1.5f;

        public Camera(List<Car> cars, GraphicsDeviceManager graphics, GraphicsDevice GraphicsDevice, Rectangle visibleRect)
        {
            this.cars = cars;
            this.graphics = graphics;
            rectangle = visibleRect;
            graphicsDevice = GraphicsDevice;
        }

        public void Update(TimeSpan time)
        {
            Vector2 visibleCircle = new Vector2(Camera.visibleCircle);

            Vector2 min = new Vector2(float.PositiveInfinity);
            Vector2 max = new Vector2(float.NegativeInfinity);
            center = Vector2.Zero;
            foreach (Car car in cars)
            {
                updateMin(ref min, car.Position - visibleCircle);
                updateMax(ref max, car.Position + visibleCircle);

                Car.SpeedInfo info = car.getSpeed();
                if (Math.Abs(info.speed.X) > Math.Abs(info.speed.Y))
                    if (info.speed.X > 0)
                        updateMinMax(ref min, ref max, car.Position + velocityMul * info.speed.Length() * Vector2.UnitX);
                    else
                        updateMinMax(ref min, ref max, car.Position + velocityMul * -info.speed.Length() * Vector2.UnitX);
                else
                    if (info.speed.Y > 0)
                        updateMinMax(ref min, ref max, car.Position + velocityMul * info.speed.Length() * Vector2.UnitY);
                    else
                        updateMinMax(ref min, ref max, car.Position + velocityMul * -info.speed.Length() * Vector2.UnitY);
                
                center += car.Position / cars.Count;
            }

            if ((max.X - min.X) > (max.Y - min.Y))
            {
                zoom = Math.Min(rectangle.Width, rectangle.Height) / (max.X - min.X);
            }
            else
            {
                zoom = Math.Min(rectangle.Width, rectangle.Height) / (max.Y - min.Y);
            }

            zoom = Math.Min(zoom, 1);
        }

        public void Draw(SpriteBatch spriteBatch, List<DrawableObject> objList)
        {
            graphicsDevice.ScissorRectangle = rectangle;

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, graphicsDevice.DepthStencilState, graphicsDevice.RasterizerState);

            spriteBatch.Draw(Game1.lineTexture, new Vector2(rectangle.X, rectangle.Y), null, Color.White, 0, Vector2.Zero, new Vector2(rectangle.Width, 1), SpriteEffects.None, 0.0f);
            spriteBatch.Draw(Game1.lineTexture, new Vector2(rectangle.X, rectangle.Y + rectangle.Height - 1), null, Color.White, 0, Vector2.Zero, new Vector2(rectangle.Width, 1), SpriteEffects.None, 0.0f);
            spriteBatch.Draw(Game1.lineTexture, new Vector2(rectangle.X, rectangle.Y), null, Color.White, 0, Vector2.Zero, new Vector2(1, rectangle.Height), SpriteEffects.None, 0.0f);
            spriteBatch.Draw(Game1.lineTexture, new Vector2(rectangle.X + rectangle.Width - 1, rectangle.Y), null, Color.White, 0, Vector2.Zero, new Vector2(1, rectangle.Height), SpriteEffects.None, 0.0f);
            
            foreach (DrawableObject obj in objList)
            {
                spriteBatch.Draw(obj.texture, zoom * ((obj.Position - center)) + new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2), null, Color.White, obj.Rotation, obj.origin, obj.scale * zoom, SpriteEffects.None, obj.level);
            }
            spriteBatch.End();
        }

        private void updateMin(ref Vector2 min, Vector2 val)
        {
            if (min.X > val.X)
                min.X = val.X;
            if (min.Y > val.Y)
                min.Y = val.Y;
        }

        private void updateMax(ref Vector2 max, Vector2 val)
        {
            if (max.X < val.X)
                max.X = val.X;
            if (max.Y < val.Y)
                max.Y = val.Y;
        }

        private void updateMinMax(ref Vector2 min, ref Vector2 max, Vector2 val)
        {
            if (min.X > val.X)
                min.X = val.X;
            if (min.Y > val.Y)
                min.Y = val.Y;
            if (max.X < val.X)
                max.X = val.X;
            if (max.Y < val.Y)
                max.Y = val.Y;
        }
    }
}
