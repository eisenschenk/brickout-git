using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brickout
{
    class Lines
    {
        public Vector2 Start;
        public Vector2 End;
        public Vector2 Vector;



        public Lines(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
            Vector = new Vector2(End.X - Start.X, End.Y - Start.Y);


        }

        public float SlopeM(Vector2 start, Vector2 end)
        {
            return (end.Y - start.Y) / (end.X - start.X);
        }
        public float YInterceptB(Vector2 start, Vector2 end)
        {
            return (start.Y / (SlopeM(start, end) * start.X));
        }
        public float LineIntersectionX(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            return (YInterceptB(start2, end2) - YInterceptB(start1, end1)) / (SlopeM(start1, end1) - SlopeM(start2, end2));
        }

        public float LineIntersectionY(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            return (LineIntersectionX(start1, end1, start2, end2) * SlopeM(start1, end1) + YInterceptB(start1, end1));
        }
        public Vector2 GetIntersection(Lines line)
        {
            return (new Vector2(LineIntersectionX(Start, End, line.Start, line.End), LineIntersectionY(Start, End, line.Start, line.End)));
        }
        public bool AreIntersecting(Lines line)
        {
            float x = 0;
            float y = 0;
            x = LineIntersectionX(Start, End, line.Start, line.End);
            y = LineIntersectionY(Start, End, line.Start, line.End);
            return (x != 0 && y != 0);
        }
        

        //http://www.cs.swan.ac.uk/~cssimon/line_intersection.html
        public Vector2 LineSegmentIntersection(Lines line)
        {
            float tA = -1;
            float tB = -1;
            float tALineTop = (line.Start.Y - line.End.Y) * (Start.X - line.Start.X) + (line.End.X - line.Start.X) * (Start.Y - line.Start.Y);
            float tALineBottom = (line.End.X - line.Start.X) * (Start.Y - End.Y) - (Start.X - End.X) * (line.End.Y - line.Start.Y);
            float tBLineTop = (Start.Y - End.Y) * (Start.X - line.Start.X) + (End.X - Start.X) * (Start.Y - line.Start.Y);
            float tBLineBottom = (line.End.X - line.Start.X) * (Start.Y - End.Y) - (Start.X - End.X) * (line.End.Y - line.Start.Y);

            if (tALineBottom != 0 && tBLineBottom != 0)
            {
                tA = tALineTop / tALineBottom;
                tB = tBLineTop / tBLineBottom;
            }

            if (0 <= tA && tA <= 1 && 0 <= tB && tB <= 1)
            {
                float x = Start.X + tA * (End.X - Start.X);
                float y = Start.Y + tA * (End.Y - Start.Y);
                return (new Vector2(x, y));
            }
            else
                return new Vector2(0, 0);
        }
        public float Getlength()
        {
            float x = Math.Abs(Start.X - End.X);
            float y = Math.Abs(Start.Y - End.Y);
           return (float)Math.Sqrt((float)Math.Pow(x, 2) + (float)Math.Pow(y, 2));
        }



    }
}
