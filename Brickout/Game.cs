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
    // player life --> value not shown
    class Game : IDisposable
    {
        List<GameObject> GobjectList = new List<GameObject>();
        List<Powerup> PowerupList = new List<Powerup>();
        List<Ball> BallList = new List<Ball>();
        public Player Player;
        public Ball Ball;
        public Gameboard Gameboard;
        public Random Random = new Random();
        public Vector2 BallSize = new Vector2(20, 20);
        public float Elapsed;
        public int Width;
        public int Height;
        DirectInput DirectInput;
        Keyboard Keyboard;
        Bitmap Tileset;
        Color4 BackgroundColor;
        int Score;

        private IEnumerable<TextCharacter> AllCharacters => TextCharacter.GetOutput($"Score: {Score}", new Vector2(8, 2))
            .Concat(TextCharacter.GetOutput($"Life: {Player.Life}", new Vector2(Width - 120, 2)));

        public Game(int width, int height, string level)
        {
            Width = width;
            Height = height;
            Player = CreatePlayer();
            GobjectList.Add(Player);
            Ball = CreateBall();
            Gameboard = new Gameboard(width, height, level, GobjectList);
            GobjectList.Add(Gameboard);
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
            foreach (TextCharacter character in AllCharacters)
                render.DrawBitmap(Tileset, new RectangleF(character.Position.X, character.Position.Y, character.Size.X, character.Size.Y),
                    1, BitmapInterpolationMode.Linear, character.Sprite);
            render.EndDraw();
        }

        public void Update(float elapsed)
        {
            Elapsed = elapsed;
            KeyboardState keyboard = Keyboard.GetCurrentState();
            if (Ball.Direction.IsZero)
            {
                Ball.Position.X = Player.Position.X + Player.Size.X * 0.5f;
                Ball.Position.Y = Player.Position.Y - Ball.Size.Y - 1;
            }
            if (keyboard.IsPressed(Key.Left) && Player.IsValidMovementLeft(Player, Gameboard))
                Player.Position.X += -Player.Speed;
            if (keyboard.IsPressed(Key.Right) && Player.IsValidMovementRight(Player, Gameboard))
                Player.Position.X += Player.Speed;
            if (keyboard.IsPressed(Key.Space) && Ball.Direction.IsZero && Player.Life >= 0)
                Ball.Direction = new Vector2(0, -1);
            //debug player,ball
            Player playerBase = new Player(new Vector2(0, 0));
            Ball ballBase = new Ball(playerBase);
            //debug playerSize+
            if (keyboard.IsPressed(Key.F1))
            {
                Player.Size.X += playerBase.Size.X / 3;
                if (!Gameboard.IncludesGameObject(Player))
                    Player.Position.X = Gameboard.Width - Player.Size.X;
            }
            //debug playerSize-
            if (keyboard.IsPressed(Key.F2))
                Player.Size.X -= playerBase.Size.X / 3;
            //debug ball imba
            if (keyboard.IsPressed(Key.F7))
            {
                Ball.BallImbalanced = true;
                Ball.BallImbaNow.Start();
            }
            //debug ball split
            if (keyboard.IsPressed(Key.F8))
                GobjectList.Add(new Ball(Ball));

            MoveGameObjetcs();
        }

        //ToDo
        private void GameOver()
        {

        }
        private void MovePowerup()
        {
            foreach (Powerup pU in PowerupList.ToArray())
            {
                pU.Direction.Normalize();
                Vector2 newPuPosition = pU.BRPoint + pU.Direction * Elapsed * pU.Speed;
                Line pULine = new Line(pU.BRPoint, newPuPosition);
                List<GameObject> isHitList = new List<GameObject>();
                isHitList = GobjectList
                               .Where(g => g is Player)
                                .Where(g => g.ObjectIsHitting(pULine, pU))
                                .ToList();
                if (isHitList.Any())
                {
                    pU.UsePowerup(Player, GobjectList, Gameboard, BallList);
                    PowerupList.Remove(pU);
                    GobjectList.Remove(pU);
                    isHitList.Clear();
                }
                else
                    pU.Position += pU.Direction * Elapsed * pU.Speed;
            }

        }
        private void MoveGameObjetcs()
        {
            Lifelost();
            MovePowerup();
            BounceAndMoveBall();
        }
        private void Lifelost()
        {
            foreach (Ball ball in BallList.ToArray())
                if (ball.BRPoint.Y >= Height)
                {
                    GobjectList.Remove(ball);
                    BallList.Remove(ball);
                    if (BallList.Count <= 0)
                    {
                        Player.LifeLost();
                        Ball = CreateBall();

                    }
                }
        }
        private void BounceAndMoveBall()
        {
            foreach (GameObject gObject in GobjectList.ToArray())
            {
                if (gObject is Ball)
                {

                    Ball ball = (Ball)gObject;
                    // BallDistance = Elapsed * ball.Speed;
                    ball.Direction.Normalize();
                    Vector2 newBallPosition = ball.BRPoint + ball.Direction * ball.Speed * Elapsed;
                    Line ballLine = new Line(ball.BRPoint, newBallPosition);
                    BounceAndMove(ballLine, ball);
                }
            }
        }

        private bool BounceAndMove(Line ballLine, Ball ball)
        {
            bool isHit = default;
            List<GameObject> isHitList = new List<GameObject>();
            Intersection intersection = default;

            isHitList = GobjectList
                .Except(PowerupList)
                .Where(b => !(b is Ball))
                 .Where(g => g.ObjectIsHitting(ballLine, ball))
                 .ToList();

            isHitList = BallIsImba(isHitList, ball);
            if (isHitList.Any())
            {
                intersection = GameObject.GetIntersection(ballLine, ball, isHitList, isHitList);
                GameObject[] isHitArray = isHitList.ToArray();
                isHitList.Clear();
                BouncesBall(intersection, ballLine, isHitArray, ball);
            }
            else
            {
                ball.Direction.Normalize();
                ball.Position += ball.Direction * ball.Speed * Elapsed;
            }


            return isHit;
        }
        private void BouncesBall(Intersection intersection, Line ballLine, GameObject[] isHitArray, Ball ball)
        {
            ball.Direction = ball.Bounce(intersection, ballLine, Gameboard);
            ball.BRPoint = intersection.IntersectionPoint;
            Lifelost();
            ball.Direction.Normalize();
            ball.Direction *= intersection.LengthAfterIntersection;
            ball.BRPoint = intersection.IntersectionPoint + ball.Direction;
            Line newBallLine = new Line(intersection.IntersectionPoint, intersection.IntersectionPoint + ball.Direction);
            //isHitArray.ToList().ForEach(g => GobjectList.Remove(g));
            GobjectList = GobjectList.Except(isHitArray).ToList();
            if (newBallLine.Start != newBallLine.End)
                BounceAndMove(newBallLine, ball);
            foreach (GameObject gObject in isHitArray)
            {
                GobjectList.Add(gObject);
                ReduceDurability(gObject);
            }
        }
        private List<GameObject> BallIsImba(List<GameObject> isHitList, Ball ball)
        {
            if (ball.BallImbalanced)
            {
                BallList.ForEach(b => b.Sprite = new RawRectangleF(66, 136, 74, 144));
                GobjectList = GobjectList.Except(isHitList.OfType<Brick>()).ToList();
                Score += isHitList.OfType<Brick>().Select(b => b.ScorePoints).Sum();
                isHitList.OfType<Brick>().Where(b => b.BrickID == 9).ToList().ForEach(p => PowerupBrickHit(p));
                isHitList = isHitList.Where(g => !(g is Brick)).ToList();
                if (ball.BallImbaNow.ElapsedMilliseconds > ball.BallImbaWindow.TotalMilliseconds)
                {
                    ball.BallImbalanced = false;
                    ball.Sprite = new Ball(Player).Sprite;
                }
            }
            return isHitList;
        }
        public void ReduceDurability(GameObject gObject)
        {
            if (gObject is Brick brick)
            {
                brick.Durability--;
                if (brick.Durability <= 0)
                {
                    GobjectList.Remove(gObject);
                    Score += brick.ScorePoints;
                    PowerupBrickHit(brick);
                }
            }
        }
        public void PowerupBrickHit(Brick brick)
        {
            if (brick.BrickID == 9)
            {
                Powerup powerup = new Powerup(
                    new Vector2(brick.Position.X + brick.Size.X / 2, brick.Position.Y + brick.Size.Y + Ball.Size.Y),
                    Random.Next(2), Random);
                PowerupList.Add(powerup);
                GobjectList.Add(powerup);
            }
        }
        public Player CreatePlayer()
        {
            return new Player(new Vector2(Width / 2f, Height - 55));
        }
        private Ball CreateBall()
        {
            Ball ball = new Ball(Player);
            GobjectList.Add(ball);
            BallList.Add(ball);
            return ball;
        }
        public void Dispose()
        {
            Tileset.Dispose();
            Keyboard.Dispose();
            DirectInput.Dispose();
        }
    }
}
