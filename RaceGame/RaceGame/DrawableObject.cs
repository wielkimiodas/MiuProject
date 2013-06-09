using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RaceGame
{
    public class DrawableObject
    {
        public Vector2 origin { get; protected set; }
        public Texture2D texture { get; protected set; }
        public Vector2 scale { get; protected set; }
        public float level = 1.0f;

        public virtual Vector2 Position { get; set; }

        public virtual float Rotation { get; set; }

        public DrawableObject(Texture2D texture)
            : this(texture, Vector2.One, Vector2.Zero, 0) { }

        public DrawableObject(Texture2D texture, Vector2 scale, Vector2 position, float rotation)
        {
            this.texture = texture;
            this.scale = scale;
            this.Position = position;
            this.Rotation = rotation;

            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }
    }
}
