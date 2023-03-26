using ScratchProjectDatingApp.DTOs;
using ScratchProjectDatingApp.Entity;
using ScratchProjectDatingApp.Helper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Text.RegularExpressions;

namespace ScratchProjectDatingApp.Interfaces
{
    public interface IMessageRepository
    {

        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
      
        void AddGroup(Entity.Group group);
        void RemoveConnection(Entity.Connection connection);
        Task<Entity.Connection> GetConnection(string connectionId);
        Task<Entity.Group> GetMessageGroup(string groupName);
        Task<Entity.Group> GetGroupForConnection(string connectionId);
    }
}
