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
    public class Car : PhysicObj
    {
        public struct SpeedInfo
        {
            public Vector2 speed;
            public Vector2 sideSpeed;
            public Vector2 direction;
            public float angleCos;
        }

        float maxSteer = (float)(Math.PI / 8);
        float d;
        public InputState inputState;

        int currentSector = 3;

        DateTime? lapTime = null;
        TimeSpan? bestLapTime = null;
        TimeSpan? lastLapTime = null;
        Rectangle[] rects;
        Stack<int> toFinish = new Stack<int>();

        // minimalna predkosc boczna, kazda mniejsza bedzie obcinana
        const float minSideVelocity = 0.1f;
        // zwalnianie predkosci bocznej
        const float sideBreaking = 120f;
        // maksymalna predkosc do przodu
        const float maxForwardSpeed = 500;
        // maksymalna predkosc do tylu
        const float maxBackwardSpeed = 200;
        // przyspieszenie samochodu
        const float acceleration = 100f;
        // hamowanie samochodu
        const float breaking = 200f;
        // hamowanie na luzie
        const float dragBreaking = 50f;
        // minimalna predkosc do przodu / tylu
        const float minVelocity = 1f;

        public Car(World world, Texture2D texture, Vector2 scale, float density)
            : base(world, texture, scale, density)
        {
            d = texture.Height * scale.Y;
            inputState = new InputState();

            rects = new Rectangle[] {
                new Rectangle(250, 1500, 1050, 1250),
                new Rectangle(1300, 1500, 1050, 1250),
                new Rectangle(1300, 250, 1050, 1250),
                new Rectangle(250, 250, 1050, 1250),
            };

            toFinish.Push(0);
            toFinish.Push(3);
        }

        public void update(TimeSpan time)
        {
            if (time.TotalSeconds <= 0)
                return;

            int index = -1;
            for (int i = 0; i < rects.Length; i++)
                if (rects[i].Contains(new Point((int)Position.X, (int)Position.Y)))
                {
                    index = i;
                    break;
                }

            if (index != currentSector)
            {
                int val = toFinish.Pop();
                if (currentSector == val)
                {
                    if (toFinish.Count == 1)
                    {
                        int val2 = toFinish.Pop();
                        if (val2 == index)
                        {
                            if (lapTime != null)
                            {
                                lastLapTime = DateTime.Now - lapTime;
                                if (bestLapTime == null || lastLapTime < bestLapTime)
                                    bestLapTime = lastLapTime;
                            }

                            lapTime = DateTime.Now;

                            toFinish.Push(0);
                            toFinish.Push(3);
                            toFinish.Push(2);
                            toFinish.Push(1);
                            toFinish.Push(0);
                        }
                        else
                            toFinish.Push(val2);
                    }
                }
                else
                {
                    toFinish.Push(val);
                    toFinish.Push(index);
                }

                currentSector = index;
            }

            SpeedInfo speedInfo = getSpeed();

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
                    float mul = (float)time.TotalSeconds * breaking * inputState.acceleration;
                    mul -= (float)time.TotalSeconds * acceleration * inputState.breakVal;
                    speedInfo.speed += speedInfo.direction * mul;
                    if (speedInfo.speed.Length() > maxBackwardSpeed)
                    {
                        speedInfo.speed *= maxBackwardSpeed / speedInfo.speed.Length();
                    }
                }
            }
            else if (speedInfo.speed.Length() > minVelocity)
            {
                int sign = getLinearVelocitySign(speedInfo.speed, speedInfo.direction);
                speedInfo.speed -= sign * speedInfo.direction * (float)time.TotalSeconds * dragBreaking;
                if (getLinearVelocitySign(speedInfo.speed, speedInfo.direction) != sign)
                    speedInfo.speed = Vector2.Zero;
            }
            else
                speedInfo.speed = Vector2.Zero;

            int linearVelocitySign = getLinearVelocitySign(speedInfo.speed, speedInfo.direction);

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
            {
                Vector2 sideDirection = speedInfo.sideSpeed;
                sideDirection.Normalize();
                speedInfo.sideSpeed -= sideDirection * (float)time.TotalSeconds * sideBreaking;
            }
            else
                speedInfo.sideSpeed = Vector2.Zero;

            AngularVelocity = angularVelocity / (float)time.TotalSeconds;
            LinearVelocity = linearVelocitySign * getVelocityVect(speedInfo.speed.Length(), Rotation + angularVelocity) + speedInfo.sideSpeed;
        }

        public SpeedInfo getSpeed()
        {
            Vector2 direction = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            float angleCos = 0;

            if (LinearVelocity.Length() != 0 && direction.Length() != 0)
                angleCos = getAngleCos(LinearVelocity, direction);

            Vector2 speed = direction * angleCos * LinearVelocity.Length();

            return new SpeedInfo()
            {
                speed = speed,
                sideSpeed = LinearVelocity - speed,
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

        static int getLinearVelocitySign(Vector2 speed, Vector2 direction)
        {
            return Math.Sign(speed.X) == Math.Sign(direction.X)
                    && Math.Sign(speed.Y) == Math.Sign(direction.Y) ? 1 : -1;
        }

        public String getLapTime()
        {
            if (lapTime == null)
                return "-";
            return (DateTime.Now - lapTime).ToString().Substring(0, 12).Substring(3);
        }

        public String getBestLapTime()
        {
            if (bestLapTime == null)
                return "-";
            return bestLapTime.ToString().Substring(0, 12).Substring(3);
        }

        public String getLastLapTime()
        {
            if (lastLapTime == null)
                return "-";
            return lastLapTime.ToString().Substring(0, 12).Substring(3);
        }
    }
}
