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
        public int MarginTop;

        public override Line GetLeftBorder(GameObject ball) => new Line(new Vector2(ball.Size.X, ball.Size.Y), new Vector2(ball.Size.X, Height + ball.Size.Y));
        public override Line GetTopBorder(GameObject ball) => new Line(new Vector2(ball.Size.X, ball.Size.Y), new Vector2(Width, ball.Size.Y));
        public override Line GetRightBorder(GameObject ball) => new Line(new Vector2(Width, ball.Size.Y), new Vector2(Width, Height + ball.Size.Y));
        public override Line GetBottomBorder(GameObject ball) => new Line(new Vector2(ball.Size.X, Height + ball.Size.Y), new Vector2(Width, Height + ball.Size.Y));
        public Gameboard(int width, int height, string level, List<GameObject> gObList)
            : base(new Vector2(0, 0), new Vector2(width, height), new RawRectangleF())
        {
            Width = width;
            Height = height;
            GameBoard = JsonConvert.DeserializeObject<int[][]>(File.ReadAllText(level));
            MarginTop = 30;
            FillList(width, height, gObList);
        }
        public void FillList(int width, int height, List<GameObject> gObList)
        {
            Player player = new Player(new Vector2(0, 0));
            Ball ball = new Ball(player);
            //schleife für gleichmäßige verteilung der bricks
            for (int outerIndex = 0; outerIndex < GameBoard.Length; outerIndex++)
                for (int innerIndex = 0; innerIndex < GameBoard[outerIndex].Length; innerIndex++)
                {
                    int brickX = width / GameBoard[outerIndex].Length * innerIndex + 1;
                    int brickY = height / 2 / GameBoard.Length * outerIndex + 1;
                    gObList.Add(new Brick(new Vector2(brickX, brickY + MarginTop), GameBoard[outerIndex][innerIndex], ball));
                }
        }
    }
}
