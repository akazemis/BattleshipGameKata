using System;
using System.Collections.Generic;

namespace BattleShip
{
    public class Ship
    {
        public HashSet<Position> Coordinates { get; set; } = new HashSet<Position>();
        public HashSet<Position> HitCoordinates { get; set; } = new HashSet<Position>();

        public override bool Equals(object obj)
        {
            var ship = obj as Ship;
            if(ship == null)
            {
                return false;
            }
            return Equals(ship);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private bool Equals(Ship other)
        {
            return Coordinates.SetEquals(other.Coordinates) &&
                   HitCoordinates.SetEquals(other.HitCoordinates);
        }
    }
}
