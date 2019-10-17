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

        private static readonly Dictionary<int, int> DurabiltyToSpriteID = new Dictionary<int, int>
        {
            [1] = 14,
            [2] = 15,
            [3] = 16,
        };

        private static RawRectangleF ReturnBrick(int durability)
        {
            if (!DurabiltyToSpriteID.TryGetValue(durability, out int spriteID))
                spriteID = 1;

            Vector2 start = new Vector2(48, 8);
            int row = spriteID / 7;
            int cul = spriteID % 7;
            int width = 32;
            int height = 16;
            int distance = 4;
            //left - top - right - bottom
            return new RawRectangleF(
                start.X + (distance + width) * cul,
                start.Y + (distance + height) * row,
                start.X + width + (distance + width) * cul,
                start.Y + height + (distance + height) * row);


        }

        //private static RawRectangleF ReturnBrick(int durability)
        //{


        //    switch (durability)
        //    {
        //        case 1: return new RawRectangleF(47, 48, 80, 63);   //silver
        //        case 2: return new RawRectangleF(83, 48, 116, 63);  //gold
        //        case 3: return new RawRectangleF(119, 48, 152, 63); //red
        //        default: return new RawRectangleF(7, 48, 39, 63);
        //    }
        ////}
    }
}

