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
        public virtual RawRectangleF Sprite { get; }
        public float Speed;
        private bool intersectLeft;
        private bool intersectRight;
        private bool intersectTop;
        private bool intersectBottom;
        private Vector2 topLeft(GameObject ball) => new Vector2(Position.X, Position.Y);
        private Vector2 topRight(GameObject ball) => new Vector2(Position.X + Size.X + ball.Size.X, Position.Y);
        private Vector2 botLeft(GameObject ball) => new Vector2(Position.X, Position.Y + ball.Size.Y + Size.Y);
        private Vector2 botRight(GameObject ball) => new Vector2(Position.X + Size.X + ball.Size.X, Position.Y + Size.Y + ball.Size.Y);

        public virtual LineSegment GetLeftBorder(GameObject ball) => new LineSegment(topLeft(ball), botLeft(ball));
        public virtual LineSegment GetRightBorder(GameObject ball) => new LineSegment(topRight(ball), botRight(ball));
        public virtual LineSegment GetTopBorder(GameObject ball) => new LineSegment(topLeft(ball), topRight(ball));
        public virtual LineSegment GetBottomBorder(GameObject ball) => new LineSegment(botLeft(ball), botRight(ball));


        public GameObject(Vector2 position, Vector2 size, RawRectangleF sprite)
        {
            Position = position;
            Size = size;
            Sprite = sprite;
        }
        public void Move(float elapsed)
        {
            Direction.Normalize();
            Position += Direction * Speed * elapsed;
        }
        public bool ObjectIsHitting(LineSegment ballLine, GameObject ball)
        {
            var nullVector = new Vector2(0, 0);
            intersectLeft = (nullVector != GetLeftBorder(ball).LineSegmentIntersection(ballLine));
            intersectRight = (nullVector != GetRightBorder(ball).LineSegmentIntersection(ballLine));
            intersectTop = (nullVector != GetTopBorder(ball).LineSegmentIntersection(ballLine));
            intersectBottom = (nullVector != GetBottomBorder(ball).LineSegmentIntersection(ballLine));

            return (intersectLeft || intersectRight || intersectTop || intersectBottom);
        }
        public static Intersection GetIntersection(LineSegment ballLine, GameObject ball, List<GameObject> gObjectList, List<GameObject> isHitList)
        {
            List<Intersection> intersectionList = new List<Intersection>();
            void AddBorderToList(LineSegment line, GameObject gObject)
            {
                if (!line.LineSegmentIntersection(ballLine).IsZero)
                    intersectionList.Add(new Intersection(gObject, line, ballLine, gObjectList.IndexOf(gObject)));
            }

            foreach (GameObject gObject in gObjectList)
            {
                AddBorderToList(gObject.GetLeftBorder(ball), gObject);
                AddBorderToList(gObject.GetRightBorder(ball), gObject);
                AddBorderToList(gObject.GetTopBorder(ball), gObject);
                AddBorderToList(gObject.GetBottomBorder(ball), gObject);
            }
            intersectionList = intersectionList
                .GroupBy(i => i.DistanceToIntersection)
                .OrderBy(g => g.Key)
                .First() //1st grp
                .ToList();
            if (intersectionList.Count > 0)
            {
                isHitList.Clear();
                isHitList.AddRange(intersectionList.Select(i => i.IntersectingObject));
                return intersectionList.First();
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
