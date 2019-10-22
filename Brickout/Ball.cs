using SharpDX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brickout
{
    class Ball : GameObject
    {
        public bool BallImbalanced;
        public Stopwatch BallImbaNow = new Stopwatch();
        public TimeSpan BallImbaWindow = TimeSpan.FromSeconds(10);
        public Ball(Player player) : base(new Vector2(player.Position.X + player.Size.X / 2, player.Position.Y - 25), new Vector2(20, 20), new RawRectangleF(48, 136, 56, 144))
        {
            Speed = 200;
            Direction = new Vector2(0, 0);
        }
        private Ball(Vector2 position, Vector2 size, RawRectangleF sprite, float speed, Vector2 direction) : base(position, size, sprite)
        {
            Speed = speed;
            Direction = direction;
        }
        public Ball Split()
        {
            return new Ball(Position, Size, Sprite, Speed, Direction*new Vector2(-1,1));
        }
        public Vector2 BRPoint
        {
            get => Position + Size;
            set => Position = value - Size;
        }
        public void Color(BallColor color)
        {
            switch (color)
            {
                case BallColor.Green: Sprite = new RawRectangleF(57, 136, 65, 144); break;
                case BallColor.Red: Sprite = new RawRectangleF(66, 136, 74, 144); break;
                case BallColor.Purple: Sprite = new RawRectangleF(75, 136, 83, 144); break;
                case BallColor.Blue:
                case BallColor.Unset: Sprite = new RawRectangleF(48, 136, 56, 144); break;
            }
        }
        public Vector2 Bounce(Intersection intersection, LineSegment ballLine, Gameboard gameboard)
        {
            Vector2 Bounce()
            {
                var reflector = intersection.IntersectionLine.Vector;
                reflector.Normalize();
                return Vector2.Reflect(Direction, reflector) * -1;
            }
            Vector2 BouncePlayer()
            {
                float distanceFromCenter = (intersection.IntersectionLine.Start.X + intersection.IntersectionLine.Getlength() / 2) - intersection.IntersectionLine.LineSegmentIntersection(ballLine).X;
                Vector2 bounceVector = new Vector2(distanceFromCenter * -2 / intersection.IntersectionLine.Getlength(), -1);
                bounceVector.Normalize();
                return bounceVector;
            }

            if (intersection.IntersectingObject is Player)
                return BouncePlayer();
            else
                return Bounce();
        }

    }
    enum BallColor
    {
        Unset,
        Blue,
        Green,
        Red,
        Purple,
    }
}
