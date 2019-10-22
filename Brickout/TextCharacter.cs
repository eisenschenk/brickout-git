using SharpDX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brickout
{
    class TextCharacter
    {
        public RawRectangleF Sprite;
        public Vector2 Position;
        public Vector2 Size => new Vector2(10, 20);

        public TextCharacter(char character, int index, Vector2 position)
        {
            Sprite = GetCharacter(character);
            Position.Y = position.Y;
            Position.X = position.X + index * (Size.X + 1);
        }


        public RawRectangleF GetCharacter(char character)
        {
            int spriteID;
            int width = 5;
            int height = 8;
            Vector2 start;
            if (character == 32)
                return new RawRectangleF(304, 0, 307, 7);
            if (character >= 'A')
            {
                start = new Vector2(304, 8);
                spriteID = character - 'A';
            }
            else
            {
                start = new Vector2(304, 48);
                spriteID = character - '0';
            }

            int distance = 1;
            int row = spriteID / 7;
            int cul = spriteID % 7;

            //left - top - right - bottom
            return new RawRectangleF(
                start.X + (distance + width) * cul,
                start.Y + (distance + height) * row,
                start.X + width + (distance + width) * cul,
                start.Y + height + (distance + height) * row);
        }

        public static TextCharacter[] GetOutput(string text, Vector2 position)
        {
            return text.ToUpper().Select((t, index) => new TextCharacter(t, index, position)).ToArray();
        }
    }
}
