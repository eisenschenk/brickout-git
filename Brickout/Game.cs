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
        public int Width;
        public int Height;
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
            foreach (Brick item in BrickList)
                render.DrawBitmap(Tileset, new RectangleF(item.Position.X, item.Position.Y, item.Size.X, item.Size.Y),
                    1, BitmapInterpolationMode.Linear, item.Sprite);
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
            {
                BrickGetsHit();
                MoveBall(Elapsed);
            }
        }
        //ToDo
        public void GameOver()
        {

        }

        public void MoveBall(float elapsed)
        {
            Ball.Direction.Normalize();
            Vector2 newBallPosition = new Vector2(Ball.Position.X + Ball.Size.X, Ball.Position.Y + Ball.Size.Y) + Ball.Speed * Elapsed * Ball.Direction;
            Lines ballLine = new Lines(Ball.Position, newBallPosition);

            if (BallMovementIsValid(elapsed))
            {
                if (player.IsHit(ballLine))
                    Ball.Direction = Ball.BouncePlayer(player.LineIsHit, ballLine, player);
                else if (gameboard.BallHitsWall(Ball))
                    Ball.Direction.X *= -1;
                else if (gameboard.BallHitsTop(Ball))
                    Ball.Direction.Y *= -1;

                Ball.Direction.Normalize();
                Ball.Position += Ball.Direction * Ball.Speed * elapsed;

            }
            else
            {
                player.Life--;
                ballExists = false;
                if (player.Life <= 0)
                    GameOver();
            }
        }
        public void BrickGetsHit()
        {
            // Vector2 ballPlusSize = new Vector2(Ball.Position.X + Ball.Size.X / 2, Ball.Position.Y + Ball.Size.Y / 2);

            Ball.Direction.Normalize();
            Vector2 newBallPosition = new Vector2(Ball.Position.X + Ball.Size.X, Ball.Position.Y + Ball.Size.Y) + Ball.Speed * Elapsed * Ball.Direction;
            Lines ballLine = new Lines(Ball.Position, newBallPosition);
            foreach (Brick brick in BrickList.ToArray())
            {

                if (brick.BallIsHitting(ballLine))
                {

                    Ball.Direction = Ball.BounceBrick(brick.LineIsHit, ballLine) * -1;
                    brick.Durability--;
                    if (brick.Durability == 0)
                        BrickList.Remove(brick);
                }
            }
        }
        public bool BallMovementIsValid(float elapsed)
        {
            Ball tryMoveBall = Ball;
            tryMoveBall.Position += tryMoveBall.Direction * tryMoveBall.Speed * elapsed;
            return (tryMoveBall.Position.Y <= Height);
        }

        public Ball CreateBall()
        {
            ballExists = true;
            return new Ball(player, Elapsed);
        }
        public void Dispose()
        {
            Tileset.Dispose();
            Keyboard.Dispose();
            DirectInput.Dispose();
        }
    }
}
