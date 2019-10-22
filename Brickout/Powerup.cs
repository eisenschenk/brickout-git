using SharpDX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brickout
{
    class Powerup : GameObject
    {
        public int PowerupID;
        public int Effect;
        public Random Random;
        public Vector2 BRPoint
        {
            get => Position + Size;
            set => Position = value - Size;
        }
        public Powerup(Vector2 position, int powerupID, Random random) : base(position, new Vector2(20, 20), ReturnRectangle(powerupID))
        {
            PowerupID = powerupID;
            Random = random;
            Effect = GetEffect(powerupID);
            Direction = new Vector2(0, 1);
            Speed = 50;
            if (powerupID != 1)
                PowerupID = -1;

        }
        private int GetEffect(int powerupID)
        {
            return Random.Next(5);
        }

        public void UsePowerup(Player player, List<GameObject> gObjectList, Gameboard gameboard, List<Ball> BallList)
        {
            Player playerBase = new Player(new Vector2(0, 0));
            Ball ballBase = new Ball(playerBase);


            //playerSize abh. von PowerupId(entweder 1 oder -1)
            if (Effect == 0 && player.Size.X < playerBase.Size.X * 2 && player.Size.X > playerBase.Size.X * 0.5f)
            {
                player.Size.X += PowerupID * playerBase.Size.X / 3;
                if (!gameboard.IncludesGameObject(player))
                    player.Position.X = gameboard.Width - player.Size.X;
            }
            foreach (Ball ball in BallList.ToArray())
                switch (Effect)
                {
                    case 1://ballSpeed abh. von PowerupId(entweder 1 oder -1)
                        if (ball.Speed < ballBase.Speed * 1.5f && ball.Speed > ballBase.Speed * 0.2f)
                            ball.Speed -= PowerupID * ballBase.Speed / 5;
                        break;
                    case 2://ballSize abh. von PowerupID(entweder 1 oder -1)
                        if (ball.Size.X < ballBase.Size.X * 1.5f && ball.Size.X > ballBase.Size.X * 0.5f)
                            ball.Size += PowerupID * ballBase.Size / 3;
                        break;
                    case 3:// ball kills all in its wake
                        if (PowerupID == 1)
                        {
                            ball.BallImbalanced = true;
                            ball.BallImbaNow.Start();
                        }
                        else
                        {
                            Effect = Random.Next(3);
                            UsePowerup(player, gObjectList, gameboard, BallList);
                        }
                        break;
                    case 4://double ball
                        if (PowerupID == 1)
                        {
                            Ball ballNew = new Ball(ball);
                            gObjectList.Add(ballNew);
                            BallList.Add(ballNew);
                        }
                        else
                        {
                            Effect = Random.Next(3);
                            UsePowerup(player, gObjectList, gameboard, BallList);
                        }
                        break;
                    default: return;
                }
        }
        private static RawRectangleF ReturnRectangle(int powerupID)
        {
            if (powerupID == 1)
                return new RawRectangleF(238, 176, 251, 191);
            else
                return new RawRectangleF(328, 176, 341, 191);


        }

    }
}
