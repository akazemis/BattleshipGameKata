namespace BattleShip
{
    public struct Position
    {
        private (int Column, int Row) _position;

        public Position(int column, int row)
        {
            _position = (column, row);
        }
        public int Column { get { return _position.Column; } }
        public int Row { get { return _position.Row; } }
    }
}
