using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddGroup(Group g);
        void removeConn(Connection conn);
        Task<Connection> GetConnectionAsync(string connId);
        Task<Group> GetMessageGroupAsync(string gName);
        Task<Group> GetGroupForConnectionAsync(string connId);
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
        Task<bool> SaveAllAsync();
    }
}