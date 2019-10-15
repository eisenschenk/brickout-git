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
            Speed = 100;
            Direction = new Vector2(10, -10);
        }
        public Vector2 BRPoint => Position + Size;
        public Vector2 BounceBrick(Lines line, Lines ballLine)
        {
            line.Vector.Normalize();
            return Vector2.Reflect(Direction, line.Vector) * -1;
        }

        public Vector2 BouncePlayer(Lines line, Lines ballLine, Player player)
        {
            line.Vector.Normalize();
            Vector2 bounceVektor = Vector2.Reflect(Direction, line.Vector) * -1; ;
            bounceVektor.Y = -1.2f + Math.Abs(bounceVektor.X);
            //statt  ballLine.end --> schnittpunkt mit player
            //von -1/0 zu 0/-1 zu 1,0 mit y = -1 + abs(x)
            if (ballLine.End.X <= player.Position.X + player.Size.X / 2)
                bounceVektor.X = -1 * Math.Abs(bounceVektor.X) * (player.Position.X + player.Size.X / 2) / ballLine.End.X;
            else
                bounceVektor.X = Math.Abs(bounceVektor.X) * (player.Position.X + player.Size.X) / ballLine.End.X;

            bounceVektor.Normalize();
            return bounceVektor;
        }
    }
}
