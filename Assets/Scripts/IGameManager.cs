public interface IGameManager
{
    public T GetSystem<T>() where T : class;
}
