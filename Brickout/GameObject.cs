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
        public Lines GetIntersectingLine(Lines ballLine, Ball ball, List<GameObject> gObjectList)
        {
            Vector2 Left = (GetLeftBorder(ball)).LineSegmentIntersection(ballLine);
            Vector2 Right = (GetRightBorder(ball)).LineSegmentIntersection(ballLine);
            Vector2 Top = (GetTopBorder(ball)).LineSegmentIntersection(ballLine);
            Vector2 Bottom = (GetBottomBorder(ball)).LineSegmentIntersection(ballLine);

            List<Vector2> lineList = new List<Vector2>();

            foreach (GameObject gObject in gObjectList)
            {
                if (!Left.IsZero)
                    lineList.Add(Left);
                if (!Right.IsZero)
                    lineList.Add(Right);
                if (!Top.IsZero)
                    lineList.Add(Top);
                if (!Bottom.IsZero)
                    lineList.Add(Bottom);
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
