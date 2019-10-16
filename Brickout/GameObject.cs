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
        public RawRectangleF Sprite;
        public float Speed;
        private bool intersectLeft;
        private bool intersectRight;
        private bool intersectTop;
        private bool intersectBottom;
        private Vector2 topLeft(Ball ball) => new Vector2(Position.X, Position.Y);
        private Vector2 topRight(Ball ball) => new Vector2(Position.X + Size.X + ball.Size.X, Position.Y);
        private Vector2 botLeft(Ball ball) => new Vector2(Position.X, Position.Y + ball.Size.Y + Size.Y);
        private Vector2 botRight(Ball ball) => new Vector2(Position.X + Size.X + ball.Size.X, Position.Y + Size.Y + ball.Size.Y);

        public virtual Lines GetLeftBorder(Ball ball) => new Lines(topLeft(ball), botLeft(ball));
        public virtual Lines GetRightBorder(Ball ball) => new Lines(topRight(ball), botRight(ball));
        public virtual Lines GetTopBorder(Ball ball) => new Lines(topLeft(ball), topRight(ball));
        public virtual Lines GetBottomBorder(Ball ball) => new Lines(botLeft(ball), botRight(ball));

        public GameObject(Vector2 position, Vector2 size, RawRectangleF sprite)
        {
            Position = position;
            Size = size;
            Sprite = sprite;
        }

        public bool BallIsHitting(Lines ballLine, Ball ball)
        {
            Vector2 nullVector = new Vector2(0, 0);
            intersectLeft = (nullVector != GetLeftBorder(ball).LineSegmentIntersection(ballLine));
            intersectRight = (nullVector != GetRightBorder(ball).LineSegmentIntersection(ballLine));
            intersectTop = (nullVector != GetTopBorder(ball).LineSegmentIntersection(ballLine));
            intersectBottom = (nullVector != GetBottomBorder(ball).LineSegmentIntersection(ballLine));

            return (intersectLeft || intersectRight || intersectTop || intersectBottom);
        }
        public Intersection GetIntersection(Lines ballLine, Ball ball, List<GameObject> gObjectList, List<Intersection> intersectionList, List<GameObject> isHitList)
        {
            Intersection closestIntersection = default;

            float shortestDistance = 0;

            foreach (GameObject gObject in gObjectList)
            {
                Vector2 Left = (gObject.GetLeftBorder(ball)).LineSegmentIntersection(ballLine);
                Vector2 Right = (gObject.GetRightBorder(ball)).LineSegmentIntersection(ballLine);
                Vector2 Top = (gObject.GetTopBorder(ball)).LineSegmentIntersection(ballLine);
                Vector2 Bottom = (gObject.GetBottomBorder(ball)).LineSegmentIntersection(ballLine);

                if (!Left.IsZero)
                    intersectionList.Add(new Intersection(gObject, gObject.GetLeftBorder(ball), ballLine, gObjectList.IndexOf(gObject)));
                if (!Right.IsZero)
                    intersectionList.Add(new Intersection(gObject, gObject.GetRightBorder(ball), ballLine, gObjectList.IndexOf(gObject)));
                if (!Top.IsZero)
                    intersectionList.Add(new Intersection(gObject, gObject.GetTopBorder(ball), ballLine, gObjectList.IndexOf(gObject)));
                if (!Bottom.IsZero)
                    intersectionList.Add(new Intersection(gObject, gObject.GetBottomBorder(ball), ballLine, gObjectList.IndexOf(gObject)));
            }
            foreach (Intersection intersection in intersectionList)
            {
                if (shortestDistance == 0 || shortestDistance >= intersection.IntersectionLength)
                {
                    shortestDistance = intersection.DistanceToIntersection;
                    closestIntersection = intersection;
                }
            }
            if (intersectionList.Count > 0)
            {
                intersectionList.Clear();
                return closestIntersection;
            }

            return null;
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
