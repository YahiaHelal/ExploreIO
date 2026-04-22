namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository {get; }
        IMessageRepository MessageRepository {get; }
        IFollowingsRepository FollowingsRepository {get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}