using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectInput;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brickout
{
    class Game : IDisposable
    {
        List<GameObject> GobjectList = new List<GameObject>();
        public Player player;
        public Ball Ball;
        public Gameboard Gameboard;
        public Vector2 BallSize = new Vector2(20, 20);
        public float Elapsed;
        public float BallDistance;
        public int Width;
        public int Height;
        bool BallExists;
        DirectInput DirectInput;
        Keyboard Keyboard;
        Bitmap Tileset;
        Color4 BackgroundColor;


        public Game(int width, int height, string level)
        {
            player = new Player(new Vector2(width / 2f, height - 55));
            GobjectList.Add(player);
            Gameboard = new Gameboard(width, height, level, GobjectList);
            GobjectList.Add(Gameboard);
            Width = width;
            Height = height;
        }

        public void LoadResources(RenderTarget render)
        {
            BackgroundColor = Color.CornflowerBlue;
            Tileset = Resources.LoadImageFromFile(render, "breakout_pieces.png");

            DirectInput = new DirectInput();
            Keyboard = new Keyboard(DirectInput);
            Keyboard.Acquire();
        }

        public void DrawScene(RenderTarget render)
        {
            render.BeginDraw();
            render.Clear(BackgroundColor);
            foreach (GameObject gObject in GobjectList)
                if (gObject is Brick)
                {
                    Brick brick = (Brick)gObject;
                    render.DrawBitmap(Tileset, new RectangleF(brick.Position.X, brick.Position.Y, brick.Size.X, brick.Size.Y),
                        1, BitmapInterpolationMode.Linear, brick.Sprite);
                }
            render.DrawBitmap(Tileset, new RectangleF(player.Position.X, player.Position.Y, player.Size.X, player.Size.Y),
                1, BitmapInterpolationMode.Linear, player.Sprite);
            if (!BallExists)
                Ball = CreateBall();
            render.DrawBitmap(Tileset, new RectangleF(Ball.Position.X, Ball.Position.Y, Ball.Size.X, Ball.Size.Y),
                1, BitmapInterpolationMode.Linear, Ball.Sprite);
            render.EndDraw();
        }

        public void Update(float elapsed)
        {
            Elapsed = elapsed;
            KeyboardState keyboard = Keyboard.GetCurrentState();

            if (keyboard.IsPressed(Key.Left) && player.IsValidMovementLeft(player, Gameboard))
                player.Position.X += -player.Speed;
            if (keyboard.IsPressed(Key.Right) && player.IsValidMovementRight(player, Gameboard))
                player.Position.X += player.Speed;

            if (BallExists)
                MoveBall();
        }
        //ToDo
        private void GameOver()
        {

        }

        private void MoveBall()
        {
            BounceAndMove();
            Lifelost();
        }
        private void Lifelost()
        {

            if (Ball.Position.Y > Height)
            {
                BallExists = false;
                player.Life--;
                if (player.Life <= 0)
                    GameOver();
            }
        }
        private void BounceAndMove()
        {
            BallDistance = Elapsed * Ball.Speed;
            Ball.Direction.Normalize();
            Vector2 newBallPosition = Ball.BRPoint + Ball.Direction * BallDistance;
            Line ballLine = new Line(Ball.BRPoint, newBallPosition);

            if (!BounceAndMove(ballLine))
            {
                Ball.Direction.Normalize();
                Ball.Position += Ball.Direction * BallDistance;
            }
        }

        private bool BounceAndMove(Line ballLine)
        {
            bool isHit = default;
            List<GameObject> IsHitList = new List<GameObject>();
            Intersection intersection = default;
            foreach (GameObject gObject in GobjectList.ToArray())
                if (gObject.BallIsHitting(ballLine, Ball))
                {
                    isHit = true;
                    IsHitList.Add(gObject);
                }
            if (isHit)
            {
                intersection = GameObject.GetIntersection(ballLine, Ball, IsHitList, IsHitList);
                GameObject[] isHitArray = IsHitList.ToArray();
                IsHitList.Clear();
                BouncesBall(intersection, ballLine, isHitArray);
            }

            return isHit;
        }
        private void BouncesBall(Intersection intersection, Line ballLine, GameObject[] isHitArray)
        {
            Ball.Direction = Ball.Bounce(intersection, ballLine, Gameboard);
            if (Ball.Direction.IsZero)
                return;
            Ball.Direction.Normalize();
            Ball.Direction *= intersection.LengthAfterIntersection;
            Ball.BRPoint = intersection.IntersectionPoint + Ball.Direction;
            foreach (GameObject gObject in isHitArray)
                GobjectList.Remove(gObject);
            Line newBallLine = new Line(intersection.IntersectionPoint, intersection.IntersectionPoint + Ball.Direction);
            if (newBallLine.Start != newBallLine.End)
                BounceAndMove(newBallLine);


            foreach (GameObject gObject in isHitArray)
            {
                GobjectList.Add(gObject);
                if (gObject is Brick)
                {
                    Brick brick = (Brick)gObject;
                    brick.Durability--;
                    if (brick.Durability <= 0)
                        GobjectList.Remove(gObject);
                }
            }


        }



        private Ball CreateBall()
        {
            BallExists = true;
            return new Ball();
        }
        public void Dispose()
        {
            Tileset.Dispose();
            Keyboard.Dispose();
            DirectInput.Dispose();
        }
    }
}
