using Newtonsoft.Json;
using SharpDX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Brickout
{
    class Gameboard : GameObject
    {
        public int Width;
        public int Height;
        public int[][] GameBoard;

        public override Lines Left => new Lines(new Vector2(BallSize.X, BallSize.Y), new Vector2(BallSize.X, Height));
        public override Lines Top => new Lines(new Vector2(BallSize.X, BallSize.Y), new Vector2(Width - BallSize.X, BallSize.Y));
        public override Lines Right => new Lines(new Vector2(Width, BallSize.Y), new Vector2(Width, Height));
        public override Lines Bottom => new Lines(new Vector2(0, Height + BallSize.Y*5), new Vector2(Width, Height + BallSize.Y*5));
        public Gameboard(int width, int height, string level, List<Brick> brickList)
            : base(new Vector2(0, 0), new Vector2(width, height), new RawRectangleF())
        {
            Width = width;
            Height = height;
            GameBoard = JsonConvert.DeserializeObject<int[][]>(File.ReadAllText(level));
            FillList(width, height, brickList);
        }
        public void FillList(int width, int height, List<Brick> brickList)
        {
            Ball ball = new Ball();
            for (int outerIndex = 0; outerIndex < GameBoard.Length; outerIndex++)
                for (int innerIndex = 0; innerIndex < GameBoard[outerIndex].Length; innerIndex++)
                {
                    int brickX = width / GameBoard[outerIndex].Length * innerIndex + 1;
                    int brickY = height / 2 / GameBoard.Length * outerIndex + 1;
                    brickList.Add(new Brick(new Vector2(brickX, brickY), GameBoard[outerIndex][innerIndex], ball));
                }
        }
        public bool BallHitsWall(Ball ball)
        {
            return (ball.Position.X + ball.Size.X <= 0 || ball.Position.X + ball.Size.X >= Width);
        }
        public bool BallHitsTop(Ball ball)
        {
            return (ball.Position.Y + ball.Size.Y <= 0);
        }


    }
}
