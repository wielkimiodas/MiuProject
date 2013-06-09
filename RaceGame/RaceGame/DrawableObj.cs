using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;

namespace RaceGame
{
    public class DrawableObj
    {
        public Vector2 origin { get; protected set; }
        public Body body { get; set; }
        public Texture2D texture { get; protected set; }
        public Vector2 scale { get; protected set; }

        private const int MULTIPLER = 100;

        public Vector2 LinearVelocity
        {
            get
            {
                return body.LinearVelocity * MULTIPLER;
            }
            set
            {
                body.LinearVelocity = value / MULTIPLER;
            }
        }

        public Vector2 Position
        {
            get
            {
                return body.Position * MULTIPLER;
            }
            set
            {
                body.Position = value / MULTIPLER;
            }
        }

        public float Rotation
        {
            get
            {
                return body.Rotation;
            }
            set
            {
                body.Rotation = value;
            }
        }

        public float AngularVelocity
        {
            get
            {
                return body.AngularVelocity;
            }
            set
            {
                body.AngularVelocity = value;
            }
        }

        public float Restitution
        {
            get
            {
                return body.Restitution;
            }
            set
            {
                body.Restitution = value;
            }
        }

        public DrawableObj(World world, Texture2D texture)
            : this(world, texture, Vector2.One) { }

        public DrawableObj(World world, Texture2D texture, Vector2 scale)
            : this(world, texture, scale, 1f) { }

        public DrawableObj(World world, Texture2D texture, Vector2 scale, float density)
        {
            this.texture = texture;
            this.scale = scale;
            scale /= MULTIPLER;

            uint[] data = new uint[texture.Width * texture.Height];

            texture.GetData(data);

            Vertices textureVertices = PolygonTools.CreatePolygon(data, texture.Width, false);

            Vector2 centroid = -textureVertices.GetCentroid();
            textureVertices.Translate(ref centroid);

            origin = -centroid;

            textureVertices = SimplifyTools.ReduceByDistance(textureVertices, 4f);

            List<Vertices> list = BayazitDecomposer.ConvexPartition(textureVertices);

            float minX = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;

            foreach (Vertices vertices in list)
            {
                vertices.Scale(ref scale);
                foreach (Vector2 vec in vertices)
                {
                    if (vec.X < minX)
                        minX = vec.X;
                    if (maxX < vec.X)
                        maxX = vec.X;
                }
            }

            Console.WriteLine(minX + "\t" + maxX);

            body = BodyFactory.CreateCompoundPolygon(world, list, density, BodyType.Dynamic);
            body.BodyType = BodyType.Dynamic;
        }
    }
}
