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
        public Vector2 scale { get; set; }
        public float level = 1.0f;

        public virtual Vector2 Position { get; set; }

        public virtual float Rotation { get; set; }

        public DrawableObject(Texture2D texture)
            : this(texture, 0, Vector2.One, Vector2.Zero) { }

        public DrawableObject(Texture2D texture, float rotation)
            : this(texture, rotation, Vector2.One, Vector2.Zero) { }

        public DrawableObject(Texture2D texture, float rotation, Vector2 scale, Vector2 position)
        {
            this.texture = texture;
            this.scale = scale;
            this.Position = position;
            this.Rotation = rotation;

            origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }
    }
}
