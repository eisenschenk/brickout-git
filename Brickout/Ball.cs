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
        public Ball(Player player, float elapsed) : base(new Vector2(400, 350), new Vector2(20, 20), new RawRectangleF(48, 136, 56, 144))
        {
            Speed = 50;
            Direction = new Vector2(10, -10);

        }
        //Bounce wird ausgeführt wenn die ballLine eine der Seiten des Bricks etc schneidet 
        //FIX--> Bounce erst wenn der Ball den Schnittpunkt erreicht.
        public Vector2 BounceBrick(Lines line, Lines ballLine)
        {

            line.Vector.Normalize();
            return Vector2.Reflect(Direction, line.Vector);
        }

        public Vector2 BouncePlayer(Lines line, Lines ballLine, Player player)
        {
            line.Vector.Normalize();
            Vector2 bounceVektor = Vector2.Reflect(Direction, line.Vector) * -1; ;
            bounceVektor.Y = -0.75f;
            if (ballLine.End.X <= player.Position.X + player.Size.X / 2)
                bounceVektor.X = -1 * Math.Abs(bounceVektor.X) * (player.Position.X + player.Size.X / 2) / ballLine.End.X;
            else
                bounceVektor.X = Math.Abs(bounceVektor.X) * (player.Position.X + player.Size.X) / ballLine.End.X;

            bounceVektor.Normalize();
            return bounceVektor;
        }
    }
}
