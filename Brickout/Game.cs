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
        List<Brick> BrickList = new List<Brick>();
        public Player player;
        public Ball Ball;
        public Gameboard gameboard;
        public Vector2 BallSize = new Vector2(20, 20);
        public float Elapsed;
        public float BallDistance;
        public int Width;
        public int Height;
        public bool hasBounced = default;
        bool ballExists = default;
        DirectInput DirectInput;
        Keyboard Keyboard;
        Bitmap Tileset;
        Color4 BackgroundColor;


        public Game(int width, int height, string level)
        {
            player = new Player(new Vector2(width / 2f, height - 55));
            gameboard = new Gameboard(width, height, level, BrickList);
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
            foreach (Brick brick in BrickList)
                render.DrawBitmap(Tileset, new RectangleF(brick.Position.X, brick.Position.Y, brick.Size.X, brick.Size.Y),
                    1, BitmapInterpolationMode.Linear, brick.Sprite);
            render.DrawBitmap(Tileset, new RectangleF(player.Position.X, player.Position.Y, player.Size.X, player.Size.Y),
                1, BitmapInterpolationMode.Linear, player.Sprite);
            if (!ballExists)
                Ball = CreateBall();
            render.DrawBitmap(Tileset, new RectangleF(Ball.Position.X, Ball.Position.Y, Ball.Size.X, Ball.Size.Y),
                1, BitmapInterpolationMode.Linear, Ball.Sprite);
            render.EndDraw();
        }

        public void Update(float elapsed)
        {
            Elapsed = elapsed;
            KeyboardState keyboard = Keyboard.GetCurrentState();

            if (keyboard.IsPressed(Key.Left) && player.IsValidMovementLeft(player, gameboard))
                player.Position.X += -player.Speed;
            if (keyboard.IsPressed(Key.Right) && player.IsValidMovementRight(player, gameboard))
                player.Position.X += player.Speed;

            if (ballExists)
                MoveBall();
        }
        //ToDo
        public void GameOver()
        {

        }

        public void MoveBall()
        {
            BounceAndMove();
            if (Ball.Position.Y > Height)
            {
                ballExists = false;
                player.Life--;
                if (player.Life <= 0)
                    GameOver();
            }
        }
        public void BounceAndMove()
        {
            BallDistance = Elapsed * Ball.Speed;
            Ball.Direction.Normalize();
            Vector2 newBallPosition = Ball.BRPoint + Ball.Direction * BallDistance;
            Lines ballLine = new Lines(Ball.BRPoint, newBallPosition);

            GameobjectGotHit(ballLine);
            if (!hasBounced)
            {
                Ball.Direction.Normalize();
                Ball.Position += Ball.Direction * BallDistance;
            }
            hasBounced = false;
        }

        public void GameobjectGotHit(Lines ballLine)
        {
            foreach (Brick brick in BrickList.ToArray())
                if (brick.BallIsHitting(ballLine))
                {
                    BouncesBall(brick, ballLine);
                    brick.Durability--;
                    if (brick.Durability == 0)
                        BrickList.Remove(brick);
                    hasBounced = true;
                    return;
                }
            if (gameboard.BallIsHitting(ballLine))
            {
                BouncesBall(gameboard, ballLine);
                hasBounced = true;
                return;
            }
            if (player.BallIsHitting(ballLine))
            {
                Ball.Direction = Ball.BouncePlayer(player.GetIntersectionLine(ballLine), ballLine, player);
                hasBounced = true;
                return;
            }
        }
        public void BouncesBall(GameObject gObject, Lines ballLine)
        {
            Lines lineHit = gObject.GetIntersectionLine(ballLine);
            Vector2 intersection = lineHit.LineSegmentIntersection(ballLine);
            Lines distanceToIntersection = new Lines(ballLine.Start, intersection);
            float lengthAfterIntersection = ballLine.Getlength() - distanceToIntersection.Getlength();
            Ball.Direction = Ball.Bounce(lineHit, ballLine);
            Ball.Direction.Normalize();
            Ball.Direction *= lengthAfterIntersection;
            Ball.Position += Ball.Direction;
            hasBounced = true;
        }



        public Ball CreateBall()
        {
            ballExists = true;
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
