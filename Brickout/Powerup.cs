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
        public PowerUpEffect Effect;
        public Random Random;

        public Vector2 BRPoint
        {
            get => Position + Size;
            set => Position = value - Size;
        }
        public Powerup(Vector2 position, PowerUpType powerUpType, Random random)
            : base(position, new Vector2(20, 20), GetSprite(powerUpType))
        {
            Random = random;
            Effect = GetEffect(powerUpType);
            Direction = new Vector2(0, 1);
            Speed = 50;
        }
        private PowerUpEffect GetEffect(PowerUpType powerUpType)
        {
            int positivePowerUpCount = 5;
            int negativePowerUpCount = 3;
            if (powerUpType == PowerUpType.Positive)
                return (PowerUpEffect)Random.Next(positivePowerUpCount);
            else
                return (PowerUpEffect)(Random.Next(negativePowerUpCount) + positivePowerUpCount);
        }

        public void UsePowerup(Player player, List<GameObject> gObjectList, Gameboard gameboard, List<Ball> BallList)
        {
            Player playerBase = new Player(new Vector2(0, 0));
            Ball ballBase = new Ball(playerBase, BallList);
            foreach (Ball ball in BallList.ToArray())
                switch (Effect)
                {
                    case PowerUpEffect.PlayerSizeLarger: ChangePlayerSize(player, playerBase, gameboard, 1); break;
                    case PowerUpEffect.PlayerSizeSmaller: ChangePlayerSize(player, playerBase, gameboard, -1); break;
                    case PowerUpEffect.BallSpeedFaster: ChangeBallSpeed(ball, ballBase, 1); break;
                    case PowerUpEffect.BallSpeedSlower: ChangeBallSpeed(ball, ballBase, -1); break;
                    case PowerUpEffect.BallSizeLarger: ChangeBallSize(ball, ballBase, 1); break;
                    case PowerUpEffect.BallSizeSmaller: ChangeBallSize(ball, ballBase, -1); break;
                    case PowerUpEffect.BallImba: MakeBallImba(ball); break;
                    case PowerUpEffect.BallSplit: SplitBall(gObjectList, BallList, ball); break;
                    default: throw new NotImplementedException();
                }
        }
        private static RawRectangleF GetSprite(PowerUpType powerUpType)
        {
            if (powerUpType == PowerUpType.Positive)
                return new RawRectangleF(238, 176, 251, 191);
            else
                return new RawRectangleF(328, 176, 341, 191);
        }
        public void ChangePlayerSize(Player player, Player playerBase, Gameboard gameboard, int modifier)
        {
            if (player.Size.X < playerBase.Size.X * 2 && player.Size.X > playerBase.Size.X * 0.5f)
                player.Size.X += modifier * playerBase.Size.X / 3;
            if (!gameboard.IncludesGameObject(player))
                player.Position.X = gameboard.Width - player.Size.X;
        }
        public void ChangeBallSpeed(Ball ball, Ball ballBase, int modifier)
        {
            if (ball.Speed < ballBase.Speed * 1.5f && ball.Speed > ballBase.Speed * 0.2f)
                ball.Speed += modifier * ballBase.Speed / 5;
        }
        public void ChangeBallSize(Ball ball, Ball ballBase, int modifier)
        {
            if (ball.Size.X < ballBase.Size.X * 1.5f && ball.Size.X > ballBase.Size.X * 0.5f)
                ball.Size += modifier * ballBase.Size / 3;
        }
        private void MakeBallImba(Ball ball)
        {
            ball.BallImbalanced = true;
            ball.BallImbaNow.Start();
        }
        private void SplitBall(List<GameObject> gObjectList, List<Ball> BallList, Ball ball)
        {
            Ball ballNew = ball.Split();
            gObjectList.Add(ballNew);
            BallList.Add(ballNew);
        }
    }
    enum PowerUpEffect
    {
        PlayerSizeLarger,
        BallSpeedSlower,
        BallSizeLarger,
        BallImba,
        BallSplit,
        PlayerSizeSmaller,
        BallSpeedFaster,
        BallSizeSmaller,
    }

    enum PowerUpType
    {
        Positive,
        Negative,
    }
}
