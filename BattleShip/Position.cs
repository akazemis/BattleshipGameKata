namespace BattleShip
{
    /// <summary>
    /// Represents a position with X,Y (Column, Row) dimensions
    /// </summary>
    public struct Position
    {
        private (int Column, int Row) _position;

        public Position(int column, int row)
        {
            _position = (column, row);
        }

        public int Column { get { return _position.Column; } }

        public int Row { get { return _position.Row; } }

        public static Position operator +(Position a, Position b) => new Position(a.Column + b.Column, a.Row + b.Row);

        public static Position operator -(Position a, Position b) => new Position(a.Column - b.Column, a.Row - b.Row);
    }
}
