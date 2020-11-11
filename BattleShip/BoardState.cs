using System.Collections.Generic;

namespace BattleShip
{
    /// <summary>
    /// The state object that holds a single user's battleship board state
    /// </summary>
    public class BoardState
    {
        public (int Width, int Height) Dimensions { get; set; }
        public List<Ship> Ships { get; set; } = new List<Ship>();
    }
}
