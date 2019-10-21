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
    //powerup + size--> player freezes@right gameborder
    //texoutput--> change text/numb string 
    // imba ball --> no disable
    // double ball not working
    // lifelost--> not working with multiple balls
    // player life --> value not shown
    class Game : IDisposable
    {
        List<GameObject> GobjectList = new List<GameObject>();
        List<Powerup> PowerupList = new List<Powerup>();
        List<Ball> BallList = new List<Ball>();
        public Player Player;
        public Ball Ball;
        public Gameboard Gameboard;
        public Textoutput Score = new Textoutput("Score: ", new Vector2(2, 2));
        public Random Random = new Random();
        public Vector2 BallSize = new Vector2(20, 20);
        public float Elapsed;
        //public float BallDistance;
        public int Width;
        public int Height;
        DirectInput DirectInput;
        Keyboard Keyboard;
        Bitmap Tileset;
        Color4 BackgroundColor;


        public Game(int width, int height, string level)
        {
            Width = width;
            Height = height;
            Player = CreatePlayer();
            GobjectList.Add(Player);
            Ball = CreateBall();
            GobjectList.Add(Ball);
            BallList.Add(Ball);
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
            if (keyboard.IsPressed(Key.Space) && Ball.Direction.IsZero && Ball.Direction.IsZero && Player.Life >= 0)
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
                Ball.BallImbalanced = true;
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
                    pU.UsePowerup(Player, Ball, GobjectList, Gameboard, BallList);
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
                    Player.Life--;
                    GobjectList.Remove(ball);
                    BallList.Remove(ball);
                    if (BallList.Count <= 0)
                    {
                        Ball = CreateBall();
                        GobjectList.Add(Ball);
                        GobjectList.Remove(Player);
                        Player = CreatePlayer();
                        GobjectList.Add(Player);
                        if (Player.Life <= 0)
                            GameOver();
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

            isHitList = BallKillsAll(isHitList, ball);
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
        private List<GameObject> BallKillsAll(List<GameObject> isHitList, Ball ball)
        {
            if (ball.BallImbalanced)
            {
                GobjectList = GobjectList.Except(isHitList.OfType<Brick>()).ToList();
                Score.Number += isHitList.OfType<Brick>().Select(b => b.ScorePoints).Sum();
                isHitList = isHitList.Where(g => !(g is Brick)).ToList();
            }
            return isHitList;
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
