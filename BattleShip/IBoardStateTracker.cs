namespace BattleShip
{
    public interface IBoardStateTracker
    {
        void AddShip(Ship ship);
        BoardStatus GetBoardStatus();
        AttackResult TakeAttack(Position position);
    }
}
