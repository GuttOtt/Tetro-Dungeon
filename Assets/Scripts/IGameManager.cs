public interface IGameManager
{
    public T GetSystem<T>() where T : class;
    public void PlayerWin();
    public void PlayerLose();
}
