using System.Collections.Generic;

namespace BattleShip
{
    public class BoardState
    {
        public (int Width, int Height) Dimensions { get; set; }
        public List<Ship> Ships { get; set; } = new List<Ship>();
    }
}
