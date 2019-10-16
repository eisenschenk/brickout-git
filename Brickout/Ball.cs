using SharpDX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brickout
{
    class Ball : GameObject
    {

        public Ball() : base(new Vector2(600, 600), new Vector2(20, 20), new RawRectangleF(48, 136, 56, 144))
        {
            Speed = 200;
            Direction = new Vector2(10, -10);
        }
        public Vector2 BRPoint
        {
            get => Position + Size;
            set => Position = value - Size;
        }
        public Vector2 Bounce(Intersection intersection, Line ballLine, Gameboard gameboard)
        {
            Vector2 Bounce()
            {
                if (intersection.IntersectionLine == gameboard.GetBottomBorder(this))
                    return new Vector2(0, 0);

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
}
