using SharpDX;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brickout
{
    class Player : GameObject
    {
        public int Life;
        private Vector2 BasePosition;
        private Vector2 BaseSize;
        public Player(Vector2 position) : base(position, new Vector2(160, 30), new RawRectangleF(115, 72, 180, 86))
        {
            Life = 5;
            Speed = 0.5f;
            BasePosition = position;
            BaseSize = Size;
        }
        public bool IsValidMovementLeft(Player player, Gameboard gameboard)
        {
            Player playerL = new Player(new Vector2(player.Position.X - 1, player.Position.Y));
            playerL.Size = player.Size;
            return (gameboard.IncludesGameObject(playerL));
        }
        public bool IsValidMovementRight(Player player, Gameboard gameboard)
        {
            Player playerR = new Player(new Vector2(player.Position.X + 1, player.Position.Y));
            playerR.Size = player.Size;
            return (gameboard.IncludesGameObject(playerR));
        }
       public void LifeLost()
        {
            Life--;
            Position = BasePosition;
            Size = BaseSize;
        }
     
    }
}
