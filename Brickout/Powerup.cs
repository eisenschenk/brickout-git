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
        public int PowerupModifier;
        public PowerUpEffect Effect;
        public Random Random;
        public Vector2 BRPoint
        {
            get => Position + Size;
            set => Position = value - Size;
        }
        public Powerup(Vector2 position, int powerupModifier, Random random) : base(position, new Vector2(20, 20), ReturnRectangle(powerupModifier))
        {
            PowerupModifier = powerupModifier;
            Random = random;
            Effect = GetEffect(powerupModifier);
            Direction = new Vector2(0, 1);
            Speed = 50;
            if (powerupModifier != 1)
                PowerupModifier = -1;

        }
        private PowerUpEffect GetEffect(int powerupID)
        {
            if (powerupID == 1)
                return (PowerUpEffect)Random.Next(5);
            else
                return (PowerUpEffect)Random.Next(3);
        }

        public void UsePowerup(Player player, List<GameObject> gObjectList, Gameboard gameboard, List<Ball> BallList)
        {
            Player playerBase = new Player(new Vector2(0, 0));
            Ball ballBase = new Ball(playerBase, BallList);
            foreach (Ball ball in BallList.ToArray())
                switch (Effect)
                {
                    case PowerUpEffect.PlayerSize: PlayerSize(player, playerBase, gameboard); break;
                    case PowerUpEffect.BallSpeed: BallSpeed(ball, ballBase); break;
                    case PowerUpEffect.BallSize: BallSize(ball, ballBase); break;
                    case PowerUpEffect.BallImba: BallImba(ball); break;
                    case PowerUpEffect.BallSplit: BallSplit(gObjectList, BallList, ball); break;
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
        public void PlayerSize(Player player, Player playerBase, Gameboard gameboard)
        {
            if (player.Size.X < playerBase.Size.X * 2 && player.Size.X > playerBase.Size.X * 0.5f)
                player.Size.X += PowerupModifier * playerBase.Size.X / 3;
            if (!gameboard.IncludesGameObject(player))
                player.Position.X = gameboard.Width - player.Size.X;
        }
        public void BallSpeed(Ball ball, Ball ballBase)
        {
            if (ball.Speed < ballBase.Speed * 1.5f && ball.Speed > ballBase.Speed * 0.2f)
                ball.Speed -= PowerupModifier * ballBase.Speed / 5;
        }
        public void BallSize(Ball ball, Ball ballBase)
        {
            if (ball.Size.X < ballBase.Size.X * 1.5f && ball.Size.X > ballBase.Size.X * 0.5f)
                ball.Size += PowerupModifier * ballBase.Size / 3;
        }
        private void BallImba(Ball ball)
        {
            ball.BallImbalanced = true;
            ball.BallImbaNow.Start();
        }
        private void BallSplit(List<GameObject> gObjectList, List<Ball> BallList, Ball ball)
        {
            Ball ballNew = ball.Split();
            gObjectList.Add(ballNew);
            BallList.Add(ballNew);
        }
    }
    enum PowerUpEffect
    {
        PlayerSize,
        BallSpeed,
        BallSize,
        BallImba,
        BallSplit,
    }
}
