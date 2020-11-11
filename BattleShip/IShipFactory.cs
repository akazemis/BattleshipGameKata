using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShip
{
    public interface IShipFactory
    {
        Ship CreateShip(OneDimensionShip oneDimensionShip);
    }
}
