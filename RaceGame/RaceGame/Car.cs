using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
//using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RaceGame
{
    public class Car : DrawableObj
    {
        struct SpeedInfo
        {
            public Vector2 speed;
            public Vector2 sideSpeed;
            public Vector2 direction;
            public float angleCos;
        }

        float maxSteer = (float)(Math.PI / 8);
        float d;
        public InputState inputState;
        
        // minimalna predkosc boczna, kazda mniejsza bedzie obcinana
        const float minSideVelocity = 0.1f;
        // maksymalna predkosc do przodu
        const float maxForwardSpeed = 500;
        // maksymalna predkosc do tylu
        const float maxBackwardSpeed = 200;
        // przyspieszenie samochodu
        const float acceleration = 100f;
        // hamowanie samochodu
        const float breaking = 200f;
        // hamowanie na luzie
        const float dragBreaking = 1f;

        public Car(World world, Texture2D texture, Vector2 scale, float density)
            : base(world, texture, scale, density)
        {
            d = texture.Height * scale.Y;
            //d = 1;
            inputState = new InputState();
        }

        public void update(TimeSpan time)
        {
            if (time.TotalSeconds <= 0)
                return;

            SpeedInfo speedInfo = getSpeed(LinearVelocity, Rotation);

            // rzeczywiste odchylenie kol od srodka
            float realSteer = inputState.steer * maxSteer;
            realSteer *= -0.9f * speedInfo.speed.Length() / maxForwardSpeed + 1;

            if (inputState.acceleration > 0 || inputState.breakVal > 0)
            {
                // jedzie do przodu
                if (speedInfo.angleCos >= 0)
                {
                    float mul = (float)time.TotalSeconds * acceleration * inputState.acceleration;
                    mul -= (float)time.TotalSeconds * breaking * inputState.breakVal;
                    speedInfo.speed += speedInfo.direction * mul;
                    if (speedInfo.speed.Length() > maxForwardSpeed)
                    {
                        speedInfo.speed = speedInfo.direction * maxForwardSpeed;
                    }
                }
                // jedzie do tylu
                else if (speedInfo.angleCos < 0)
                {
                    float mul = (float)time.TotalSeconds * acceleration * inputState.acceleration;
                    mul -= (float)time.TotalSeconds * breaking * inputState.breakVal;
                    speedInfo.speed += speedInfo.direction * mul;
                    if (speedInfo.speed.Length() > maxBackwardSpeed)
                    {
                        speedInfo.speed *= maxBackwardSpeed / speedInfo.speed.Length();
                    }
                }
            }
            else
            {
                speedInfo.speed -= speedInfo.direction * (float)time.TotalSeconds * dragBreaking;
            }

            int linearVelocitySign = Math.Sign(speedInfo.speed.X) == Math.Sign(speedInfo.direction.X)
                    && Math.Sign(speedInfo.speed.Y) == Math.Sign(speedInfo.direction.Y) ? 1 : -1;

            // odleglosc, jaka przebedzie samochod
            Vector2 distance = speedInfo.speed * (float)time.TotalSeconds;

            // droga przebyta przez samochod w linii prostej
            float linearDistance = distance.Length();
            // obrot samochodu
            float angularVelocity = 0;

            if (realSteer != 0)
            {
                // odleglosc srodka tylnej osi od srodka okregu
                float r1 = d / (float)Math.Tan(realSteer);

                // odleglosc srodka samochodu od srodka okregu
                float r = (float)Math.Sqrt(r1 * r1 + d * d / 4);

                // kat okregu, ktory przejedzie samochod
                // linearVelocity == distance.Length()
                angularVelocity = linearVelocitySign * Math.Sign(realSteer) * (linearDistance / r);

                linearDistance = r * r * (2 - (float)Math.Cos(angularVelocity));
            }

            if (speedInfo.sideSpeed.Length() > minSideVelocity)
                speedInfo.sideSpeed *= Math.Max(0, 1 - (float)time.TotalSeconds);
            else
                speedInfo.sideSpeed = Vector2.Zero;

            AngularVelocity = angularVelocity / (float)time.TotalSeconds;
            LinearVelocity = linearVelocitySign * getVelocityVect(speedInfo.speed.Length(), Rotation + angularVelocity) + speedInfo.sideSpeed;
        }

        SpeedInfo getSpeed(Vector2 v, float rotation)
        {
            Vector2 direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            float angleCos = 0;

            if (v.Length() != 0 && direction.Length() != 0)
                angleCos = getAngleCos(v, direction);

            Vector2 speed = direction * angleCos * v.Length();

            return new SpeedInfo()
            {
                speed = speed,
                sideSpeed = v - speed,
                direction = direction,
                angleCos = angleCos
            };
        }

        public static Vector2 getVelocityVect(float linearVelocity, float rotation)
        {
            return new Vector2(linearVelocity * (float)Math.Cos(rotation), linearVelocity * (float)Math.Sin(rotation));
        }

        public static float getAngleCos(Vector2 v1, Vector2 v2)
        {
            return (v1.X * v2.X + v1.Y * v2.Y) / (v1.Length() * v2.Length());
        }
    }
}
