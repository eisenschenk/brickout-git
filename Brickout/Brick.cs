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
        public int ScorePoints;
        public int BrickID;
        public Brick(Vector2 position, int brickID, Ball ball) : base(position, new Vector2(70, 30), GetSprite(brickID))
        {
            if (brickID <= 8)
                Durability = brickID;
            else
                Durability = 1;
            ScorePoints = brickID * 10;
            BrickID = brickID;
        }
        private static readonly Dictionary<int, int> BrickIDToSpriteID = new Dictionary<int, int>
        {
            [1] = 14,
            [2] = 15,
            [3] = 16,
            [9] = 17,
        };

        private static RawRectangleF GetSprite(int brickID)
        {
            if (!BrickIDToSpriteID.TryGetValue(brickID, out int spriteID))
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
    }
}

