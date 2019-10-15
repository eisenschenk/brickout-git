using SharpDX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brickout
{
    class Player : GameObject
    {
        public int Life;


        public Player(Vector2 position) : base(position, new Vector2(160, 30), new RawRectangleF(115, 72, 180, 86))
        {
            Life = 3;
            Speed = 0.5f;
            //Top = new Lines(new Vector2(Position.X, Position.Y), new Vector2(Position.X + Size.X + BallSize.X, Position.Y));
        }
        public bool IsValidMovementLeft(Player player, Gameboard gameboard)
        {
            Player playerL = new Player(new Vector2(player.Position.X - 1, player.Position.Y));
            return (gameboard.IncludesGameObject(playerL));
        }
        public bool IsValidMovementRight(Player player, Gameboard gameboard)
        {
            Player playerR = new Player(new Vector2(player.Position.X + 1, player.Position.Y));
            return (gameboard.IncludesGameObject(playerR));
        }
        //public bool IsHit(Lines ballLine)
        //{
        //    float halfBallSize = 10;
        //    Top = new Lines(new Vector2(Position.X, Position.Y), new Vector2(Position.X + Size.X + halfBallSize, Position.Y));
        //    if (Top.LineSegmentIntersection(ballLine) != new Vector2(0, 0))
        //    {
        //        LineIsHit = Top;
        //        return true;
        //    }
        //    return false;
        //}
    }
}
