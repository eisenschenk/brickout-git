using SharpDX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brickout
{
    class Brick : GameObject
    {
        public int Durability;
      
       


        public Brick(Vector2 position, int durability, Ball ball) : base(position, new Vector2(60, 30), ReturnBrick(durability))
        {
            Durability = durability;
            Left = new Lines(new Vector2(Position.X, Position.Y), new Vector2(Position.X, Position.Y + ball.Size.Y / 2 + Size.Y));
            Right = new Lines(new Vector2(Position.X + Size.X + ball.Size.X / 2, Position.Y), new Vector2(Position.X + Size.X + ball.Size.X / 2, Position.Y + Size.Y + ball.Size.Y / 2));
            Top = new Lines(new Vector2(Position.X, Position.Y), new Vector2(Position.X + Size.X + ball.Size.X / 2, Position.Y));
            Bottom = new Lines(new Vector2(Position.X, Position.Y + Size.Y + ball.Size.Y / 2), new Vector2(Position.X + Size.X + ball.Size.X / 2, Position.Y + Size.Y + ball.Size.Y / 2));

        }

        private static RawRectangleF ReturnBrick(int durability)
        {
            switch (durability)
            {
                case 1: return new RawRectangleF(47, 48, 80, 63);   //silver
                case 2: return new RawRectangleF(83, 48, 116, 63);  //gold
                case 3: return new RawRectangleF(119, 48, 152, 63); //red
                default: return new RawRectangleF(7, 48, 39, 63);
            }
        }

       
        //public Ball ChangeBallDirection(Ball ball)
        //{
        //    bool left = ball.Position.X <= Position.X - 1;
        //    bool right = ball.Position.X >= Position.X + 1 + Size.X - 1;
        //    bool top = ball.Position.Y <= Position.Y - 1;
        //    bool bottom = ball.Position.Y >= Position.Y + 1 + Size.Y - 1;

        //    if (top || bottom && left && right)
        //        ball.Direction.Y *= -1;
        //    if (left || right && top && bottom)
        //        ball.Direction.X *= -1;


        //    return ball;
        //}


    }
}

