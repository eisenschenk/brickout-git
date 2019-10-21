using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brickout
{
    class Textoutput
    {
        public string Text;
        public int Number;
        public Vector2 Position;

        public TextCharacter[] Output => GetOutput($"{Text} {Number}");



        public Textoutput(string text, Vector2 position)
        {
            Text = text;
            Position = position;
        }

        public TextCharacter[] GetOutput(string text)
        {
            return text.ToUpper().Select((t, index) =>  new TextCharacter(t, index, Position)).ToArray();
        }
    }
}
