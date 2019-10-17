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

        public List<TextCharacter> Output => GetOutput(Text + Number.ToString());



        public Textoutput(string text, Vector2 position)
        {
            Text = text;
            Position = position;
        }

        public List<TextCharacter> GetOutput(string text)
        {
            List<TextCharacter> list = new List<TextCharacter>();
            int index = 0;
            foreach (char character in text.ToUpper())
            {
                index++;
                list.Add(new TextCharacter(character, index, Position));
            }
            return list;
        }



    }
}
