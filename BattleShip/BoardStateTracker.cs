using BattleShip.Exceptions;
using System;
using System.Linq;

namespace BattleShip
{
    public class BoardStateTracker : IBoardStateTracker
    {
        private readonly BoardState _boardState;
        public (int Width, int Height) Dimensions { get { return _boardState.Dimensions; } }

        public BoardStateTracker(int width, int height)
        {
            if(width<=0 || height<= 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            _boardState = new BoardState();
            _boardState.Dimensions = (width, height);
        }

        public void AddShip(Ship ship)
        {
            ValidateAddShip(ship);
            _boardState.Ships.Add(ship);
        }

        public BoardStatus GetBoardState()
        {
            if (_boardState == null)
            {
                return BoardStatus.NotInitiated;
            }
            if (_boardState.Ships == null || !_boardState.Ships.Any())
            {
                return BoardStatus.Empty;
            }
            else
            {
                return
                    _boardState.Ships.All(ship => ship.Coordinates.Count == ship.HitCoordinates.Count)
                    ? BoardStatus.AllShipsDestroyed
                    : BoardStatus.ShipsAvailable;
            }
        }

        /// <summary>
        /// Takes an attack on a given coordinates and returns a Hit or Miss result
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public AttackResult TakeAttack(Position position)
        {
            foreach (var ship in _boardState.Ships)
            {
                if (CoordinateHits(ship, position))
                {
                    return AttackResult.Hit;
                }
            }
            return AttackResult.Miss;
        }

        /// <summary>
        /// Checks if the position hits the ship
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="position"></param>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        private bool CoordinateHits(Ship ship, Position position)
        {
            return ship.Coordinates.Contains(position);
        }

        private void ValidateAddShip(Ship ship)
        {
            if (ship == null)
            {
                throw new ArgumentNullException(nameof(ship));
            }
            if (ship.Coordinates == null || !ship.Coordinates.Any())
            {
                throw new ArgumentException("Ship coordinates cannot be null or empty.", nameof(ship));
            }
            foreach (var position in ship.Coordinates)
            {
                if (IsOutOfBoundaries(position))
                {
                    throw new ShipOutOfBoundariesException();
                }
                else if (IsOverlappingWithExistingShips(position))
                {
                    throw new ShipOverlapException();
                }
            }
        }

        private bool IsOverlappingWithExistingShips(Position position)
        {
            return _boardState.Ships.Any(ship => ship.Coordinates.Contains(position));
        }

        private bool IsOutOfBoundaries(Position position)
        {
            return position.Column < 0 ||
                   position.Row < 0 ||
                   position.Column >= Dimensions.Height ||
                   position.Row >= Dimensions.Width;
        }
    }
}
