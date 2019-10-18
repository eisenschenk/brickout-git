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
        public Player Player;
        public Ball Ball;
        public Gameboard Gameboard;
        public Textoutput Score = new Textoutput("Score: ", new Vector2(2, 2));
        public Vector2 BallSize = new Vector2(20, 20);
        public float Elapsed;
        public float BallDistance;
        public int Width;
        public int Height;
        DirectInput DirectInput;
        Keyboard Keyboard;
        Bitmap Tileset;
        Color4 BackgroundColor;


        public Game(int width, int height, string level)
        {
            Player = new Player(new Vector2(width / 2f, height - 55));
            GobjectList.Add(Player);
            Ball = CreateBall();
            GobjectList.Add(Ball);
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
            GobjectList.ForEach(r => render.DrawBitmap(Tileset, new RectangleF(r.Position.X, r.Position.Y, r.Size.X, r.Size.Y),
                    1, BitmapInterpolationMode.Linear, r.Sprite));
            foreach (TextCharacter character in Score.Output)
                render.DrawBitmap(Tileset, new RectangleF(character.Position.X, character.Position.Y, character.Size.X, character.Size.Y),
                    1, BitmapInterpolationMode.Linear, character.Sprite);
            render.EndDraw();
        }

        public void Update(float elapsed)
        {
            Elapsed = elapsed;
            KeyboardState keyboard = Keyboard.GetCurrentState();

            if (keyboard.IsPressed(Key.Left) && Player.IsValidMovementLeft(Player, Gameboard))
                Player.Position.X += -Player.Speed;
            if (keyboard.IsPressed(Key.Right) && Player.IsValidMovementRight(Player, Gameboard))
                Player.Position.X += Player.Speed;
            if (keyboard.IsPressed(Key.Space) && Ball.Direction.IsZero && Player.Life >= 0)
                Ball.Direction = new Vector2(0, -1);


            //if (BallExists)
            MoveBall();
        }
        //ToDo
        private void GameOver()
        {

        }

        private void MoveBall()
        {
            Lifelost(Ball.BRPoint);
            BounceAndMove();
        }
        private void Lifelost(Vector2 brPoint)
        {

            if (brPoint.Y >= Height)
            {
                // BallExists = false;
                Player.Life--;
                Ball.BRPoint = new Vector2(Player.Position.X + Player.Size.X / 2, Player.Position.Y - Ball.Size.Y - 5);
                Ball.Direction = new Vector2(0, 0);
                if (Player.Life <= 0)
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
            List<GameObject> isHitList = new List<GameObject>();
            Intersection intersection = default;

            isHitList = GobjectList
                 .Where(g => g.BallIsHitting(ballLine, Ball))
                 .ToList();

            if (isHitList.Any())
            {
                intersection = GameObject.GetIntersection(ballLine, Ball, isHitList, isHitList);
                GameObject[] isHitArray = isHitList.ToArray();
                isHitList.Clear();
                BouncesBall(intersection, ballLine, isHitArray);
            }

            return isHit;
        }
        private void BouncesBall(Intersection intersection, Line ballLine, GameObject[] isHitArray)
        {
            Ball.Direction = Ball.Bounce(intersection, ballLine, Gameboard);
            Ball.Direction.Normalize();
            Ball.Direction *= intersection.LengthAfterIntersection;
            Ball.BRPoint = intersection.IntersectionPoint + Ball.Direction;
            Lifelost(intersection.IntersectionPoint);
            Line newBallLine = new Line(intersection.IntersectionPoint, intersection.IntersectionPoint + Ball.Direction);
            //isHitArray.ToList().ForEach(g => GobjectList.Remove(g));
            GobjectList = GobjectList.Except(isHitArray).ToList();
            if (newBallLine.Start != newBallLine.End)
                BounceAndMove(newBallLine);
            foreach (GameObject gObject in isHitArray)
            {
                GobjectList.Add(gObject);
                ReduceDurability(gObject);
            }
        }
        public void ReduceDurability(GameObject gObject)
        {
            if (gObject is Brick)
            {
                Brick brick = (Brick)gObject;
                brick.Durability--;
                if (brick.Durability <= 0)
                {
                    GobjectList.Remove(gObject);
                    Score.Number += brick.ScorePoints;
                    PowerupHit(brick);
                }
            }
        }
        public void PowerupHit(Brick brick)
        {
            if (brick.BrickID == 9)
            {
                //ToDo
            }
        }
        private Ball CreateBall()
        {
            return new Ball(Player);
        }
        public void Dispose()
        {
            Tileset.Dispose();
            Keyboard.Dispose();
            DirectInput.Dispose();
        }
    }
}
