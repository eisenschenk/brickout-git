using SharpDX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Brickout
{
    class GameObject
    {
        public Vector2 Position;
        public Vector2 Size;
        public Vector2 Direction;
        public Vector2 BallSize;
        public RawRectangleF Sprite;
        public float Speed;
        private bool intersectLeft;
        private bool intersectRight;
        private bool intersectTop;
        private bool intersectBottom;
        private Vector2 topLeft => new Vector2(Position.X, Position.Y);
        private Vector2 topRight => new Vector2(Position.X + Size.X + BallSize.X, Position.Y);
        private Vector2 botLeft => new Vector2(Position.X, Position.Y + BallSize.Y + Size.Y);
        private Vector2 botRight => new Vector2(Position.X + Size.X + BallSize.X, Position.Y + Size.Y + BallSize.Y);

        public virtual Lines Left => new Lines(topLeft, botLeft);
        public virtual Lines Right => new Lines(topRight, botRight);
        public virtual Lines Top => new Lines(topLeft, topRight);
        public virtual Lines Bottom => new Lines(botLeft, botRight);

        public GameObject(Vector2 position, Vector2 size, RawRectangleF sprite)
        {
            Position = position;
            Size = size;
            Sprite = sprite;
            BallSize = new Vector2(20, 20);
        }

        public bool BallIsHitting(Lines ballLine)
        {
            Vector2 nullVector = new Vector2(0, 0);
            intersectLeft = (nullVector != Left.LineSegmentIntersection(ballLine));
            intersectRight = (nullVector != Right.LineSegmentIntersection(ballLine));
            intersectTop = (nullVector != Top.LineSegmentIntersection(ballLine));
            intersectBottom = (nullVector != Bottom.LineSegmentIntersection(ballLine));

            return (intersectLeft || intersectRight || intersectTop || intersectBottom);
        }
        public Lines GetIntersectionLine(Lines ballLine)
        {
            Vector2 nullVector = new Vector2(0, 0);
            if (nullVector != Left.LineSegmentIntersection(ballLine))
                return Left;
            if (nullVector != Right.LineSegmentIntersection(ballLine))
                return Right;
            if (nullVector != Top.LineSegmentIntersection(ballLine))
                return Top;
            if (nullVector != Bottom.LineSegmentIntersection(ballLine))
                return Bottom;
            throw new Exception("no Intersection");
        }
        public bool IncludesPoint(Vector2 position)
        {
            return (position.X >= Position.X && position.X <= Position.X + Size.X && position.Y >= Position.Y && position.Y <= Position.Y + Size.Y);
        }
        public bool IncludesGameObject(GameObject gameObject)
        {
            Vector2 positionPlusSize = new Vector2(gameObject.Position.X + gameObject.Size.X, gameObject.Position.Y + gameObject.Size.Y);
            return (IncludesPoint(gameObject.Position) && IncludesPoint(positionPlusSize));
        }
    }
}
