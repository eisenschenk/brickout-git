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
        public Brick(Vector2 position, int durability, Ball ball) : base(position, new Vector2(70, 30), ReturnBrick(durability))
        {
            Durability = durability;
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
    }
}

