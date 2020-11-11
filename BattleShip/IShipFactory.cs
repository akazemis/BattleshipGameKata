namespace BattleShip
{
    public interface IShipFactory
    {
        Ship CreateShip(OneDimensionShip oneDimensionShip);
    }
}
