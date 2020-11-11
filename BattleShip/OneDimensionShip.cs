namespace BattleShip
{
    /// <summary>
    /// It's a model that represents a ship and its position assuming it's a 1D ship with a specific
    /// length and orientation (Horizontal or Vertical)
    /// </summary>
    public class OneDimensionShip
    {
        public ShipOrientation Orientation { get; set; }
        public Position StartPosition { get; set; }
        public int Length { get; set; }
    }
}
