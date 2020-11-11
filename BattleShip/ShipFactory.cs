using System;
namespace BattleShip
{
    /// <summary>
    /// A simple factory class that creates objects of type <seealso cref="Ship"/> based on different input ship models
    /// </summary>
    public class ShipFactory : IShipFactory
    {
        public Ship CreateShip(OneDimensionShip oneDimensionShip)
        {
            ValidateOneDimensionShip(oneDimensionShip);
            var ship = new Ship();
            var startPosition = oneDimensionShip.StartPosition;
            var rowOffset = oneDimensionShip.Orientation == ShipOrientation.Vertical
                            ? 1
                            : 0;
            var columnOffset = oneDimensionShip.Orientation == ShipOrientation.Horizontal
                               ? 1
                               : 0;
            var row = startPosition.Row;
            var column = startPosition.Column;
            for (int i = 0; i < oneDimensionShip.Length; i++)
            {
                ship.Coordinates.Add(new Position(column, row));
                row += rowOffset;
                column += columnOffset;
            }
            return ship;
        }

        private void ValidateOneDimensionShip(OneDimensionShip oneDimensionShip)
        {
            if(oneDimensionShip == null)
            {
                throw new ArgumentNullException(nameof(oneDimensionShip));
            }
            if(oneDimensionShip.Length <= 0)
            {
                throw new ArgumentException($"The {nameof(OneDimensionShip.Length)} of {nameof(OneDimensionShip)} argument is zero or negative.");
            }
            if(oneDimensionShip.StartPosition.Row < 0 || oneDimensionShip.StartPosition.Column < 0)
            {
                throw new ArgumentException($"{nameof(OneDimensionShip.StartPosition)} of {nameof(OneDimensionShip)} argument cannot have a negative coordinate.");
            }
        }
    }
}
