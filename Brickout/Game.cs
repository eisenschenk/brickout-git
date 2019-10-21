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
    // lifelost--> not working with multimple balls
    // player life --> value not shown
    class Game : IDisposable
    {
        List<GameObject> GobjectList = new List<GameObject>();
        List<Powerup> PowerupList = new List<Powerup>();
        public Player Player;
        public Ball Ball;
        public Gameboard Gameboard;
        public Textoutput Score = new Textoutput("Score: ", new Vector2(2, 2));
        public Random Random = new Random();
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
            Width = width;
            Height = height;
            Player = CreatePlayer();
            GobjectList.Add(Player);
            Ball = CreateBall();
            GobjectList.Add(Ball);
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
            if (keyboard.IsPressed(Key.Space) && Ball.Direction.IsZero && Player.Life >= 0)
                Ball.Direction = new Vector2(0, -1);
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
                    pU.UsePowerup(Player, Ball, GobjectList);
                    PowerupList.Remove(pU);
                    GobjectList.Remove(pU);
                    isHitList.Clear();
                }
                pU.Position += pU.Direction * Elapsed * pU.Speed;
            }

        }
        private void MoveGameObjetcs()
        {
            Lifelost(Ball.BRPoint);
            MovePowerup();
            BounceAndMoveBall();
        }
        private void Lifelost(Vector2 brPoint)
        {

            if (brPoint.Y >= Height)
            {
                Player.Life--;
                GobjectList.Remove(Ball);
                Ball = CreateBall();
                GobjectList.Add(Ball);
                GobjectList.Remove(Player);
                Player = CreatePlayer();
                GobjectList.Add(Player);
                if (Player.Life <= 0)
                    GameOver();
            }
        }
        private void BounceAndMoveBall()
        {
            BallDistance = Elapsed * Ball.Speed;
            Ball.Direction.Normalize();
            Vector2 newBallPosition = Ball.BRPoint + Ball.Direction * BallDistance;
            Line ballLine = new Line(Ball.BRPoint, newBallPosition);

            BounceAndMove(ballLine);
        }

        private bool BounceAndMove(Line ballLine)
        {
            bool isHit = default;
            List<GameObject> isHitList = new List<GameObject>();
            Intersection intersection = default;

            isHitList = GobjectList
                .Except(PowerupList)
                .Where(b => !(b is Ball))
                 .Where(g => g.ObjectIsHitting(ballLine, Ball))
                 .ToList();

            isHitList = BallKillsAll(isHitList);
            if (isHitList.Any())
            {
                intersection = GameObject.GetIntersection(ballLine, Ball, isHitList, isHitList);
                GameObject[] isHitArray = isHitList.ToArray();
                isHitList.Clear();
                BouncesBall(intersection, ballLine, isHitArray);
            }
            else
            {
                Ball.Direction.Normalize();
                Ball.Position += Ball.Direction * BallDistance;

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
        private List<GameObject> BallKillsAll(List<GameObject> isHitList)
        {
            if (Ball.BallImbalanced)
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
