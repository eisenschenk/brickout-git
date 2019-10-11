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
        public Lines Left;
        public Lines Right;
        public Lines Top;
        public Lines Bottom;
        public Lines LineIsHit;


        public GameObject(Vector2 position, Vector2 size, RawRectangleF sprite)
        {
            Position = position;
            Size = size;
            Sprite = sprite;
           
        }
        public bool BallIsHitting(Lines ballLine)
        {
            Vector2 nullVector = new Vector2(0, 0);

            bool intersectLeft = (nullVector != Left.LineSegmentIntersection(ballLine));
            bool intersectRight = (nullVector != Right.LineSegmentIntersection(ballLine));
            bool intersectTop = (nullVector != Top.LineSegmentIntersection(ballLine));
            bool intersectBottom = (nullVector != Bottom.LineSegmentIntersection(ballLine));


            if (intersectLeft)
                LineIsHit = Left;
            if (intersectRight)
                LineIsHit = Right;
            if (intersectTop)
                LineIsHit = Top;
            if (intersectBottom)
                LineIsHit = Bottom;

            return (intersectLeft || intersectRight || intersectTop || intersectBottom);
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
       
       

        //public bool Hits(GameObject gameObject)
        //{
        //    Vector2 lt = new Vector2(gameObject.Position.X, gameObject.Position.Y);
        //    Vector2 lb = new Vector2(gameObject.Position.X, gameObject.Position.Y + gameObject.Size.Y);
        //    Vector2 rt = new Vector2(gameObject.Position.X + gameObject.Size.X, gameObject.Position.Y);
        //    Vector2 rb = new Vector2(gameObject.Position.X + gameObject.Size.X, gameObject.Position.Y + gameObject.Size.Y);

        //    return (IncludesPoint(lt) || IncludesPoint(lb) || IncludesPoint(rt) || IncludesPoint(rb));
        //}

    }
}
