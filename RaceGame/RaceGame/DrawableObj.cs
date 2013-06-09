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
    public class PhysicObj : DrawableObject
    {
        public Body body { get; set; }

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

        public override Vector2 Position
        {
            get
            {
                return body.Position * MULTIPLER;
            }
            set
            {
                if (body != null)
                    body.Position = value / MULTIPLER;
            }
        }

        public override float Rotation
        {
            get
            {
                return body.Rotation;
            }
            set
            {
                if (body != null)
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

        public PhysicObj(World world, Texture2D texture)
            : this(world, texture, Vector2.One) { }

        public PhysicObj(World world, Texture2D texture, Vector2 scale)
            : this(world, texture, scale, 1f) { }

        public PhysicObj(World world, Texture2D texture, Vector2 scale, float density)
            : base(texture, scale, Vector2.Zero, 0)
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

            body = BodyFactory.CreateCompoundPolygon(world, list, density, BodyType.Dynamic);
            body.BodyType = BodyType.Dynamic;
        }
    }
}
