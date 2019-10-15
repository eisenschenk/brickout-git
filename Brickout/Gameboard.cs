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
            return (ball.Position.X + ball.Size.X / 2 <= 0 || ball.Position.X + ball.Size.X / 2 >= Width);
        }
        public bool BallHitsTop(Ball ball)
        {
            return (ball.Position.Y + ball.Size.Y / 2 <= 0);
        }


    }
}
