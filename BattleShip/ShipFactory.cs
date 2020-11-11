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
            
            var position = startPosition;
            var offsetInEachStep = GetPositionOffset(oneDimensionShip.Orientation);
            for (int i = 0; i < oneDimensionShip.Length; i++)
            {
                ship.Coordinates.Add(position);
                position = position + offsetInEachStep;
            }
            return ship;
        }

        private Position GetPositionOffset(ShipOrientation shipOrientation)
        {
            var rowOffset = shipOrientation == ShipOrientation.Vertical
                            ? 1
                            : 0;
            var columnOffset = shipOrientation == ShipOrientation.Horizontal
                               ? 1
                               : 0;
            return new Position(columnOffset, rowOffset);
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
