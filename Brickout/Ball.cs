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
            Speed = 50;
            Direction = new Vector2(10, -10);
        }
        public Vector2 BRPoint
        {
            get => Position + Size;
            set => Position = value - Size;
        }
        public Vector2 Bounce(GameObject gObject, Lines line, Lines ballLine)
        {
            if (gObject is Player)
                return BouncePlayer(line, ballLine);
            else
                return Bounce(line, ballLine);
        }
        public Vector2 Bounce(Lines line, Lines ballLine)
        {
            var reflector = line.Vector;
            reflector.Normalize();
            return Vector2.Reflect(Direction, reflector) * -1;
        }

        public Vector2 BouncePlayer(Lines line, Lines ballLine)
        {
            float distanceFromCenter = (line.Start.X + line.Getlength() / 2) - line.LineSegmentIntersection(ballLine).X;
            Vector2 bounceVector = new Vector2( distanceFromCenter * -2 / line.Getlength(), -1);
            bounceVector.Normalize();
            return bounceVector;
        }
    }
}
