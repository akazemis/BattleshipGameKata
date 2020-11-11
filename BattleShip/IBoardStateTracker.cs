namespace BattleShip
{
    public interface IBoardStateTracker
    {
        void AddShip(Ship ship);
        BoardStatus GetBoardState();
        AttackResult TakeAttack(Position position);
    }
}
