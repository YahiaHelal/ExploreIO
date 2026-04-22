using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddGroup(Group g)
        {
            _context.Groups.Add(g);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnectionAsync(string connId)
        {
            return await _context.Connections.FindAsync(connId);
        }

        public async Task<Group> GetGroupForConnectionAsync(string connId)
        {
            return await _context.Groups
                .Include(c => c.Connections)
                .Where(c => c.Connections.Any(x => x.ConnectionId == connId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(u => u.Sender)
                .Include(u => u.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Group> GetMessageGroupAsync(string gName)
        {
            return await _context.Groups
                .Include(g => g.Connections)
                .FirstOrDefaultAsync(g => g.Name == gName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();
            
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username && !u.RecipientDeleted), // messages you received
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username && !u.SenderDeleted), // messages you sent
                _ => query.Where(u => u.Recipient.UserName == messageParams.Username && !u.RecipientDeleted && u.DateRead == null) // Unread messages
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                .Include(m => m.Sender).ThenInclude(u => u.Photos)
                .Include(m => m.Recipient).ThenInclude(u => u.Photos)
                .Where(
                    m => (m.RecipientUsername  == currentUsername && m.RecipientDeleted == false
                    && m.Sender.UserName == recipientUsername)
                    || (m.Recipient.UserName == recipientUsername
                    && m.Sender.UserName == currentUsername && m.SenderDeleted == false)
                )
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername).ToList();

            if(unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    // System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
                    msg.DateRead = DateTime.UtcNow;
                }
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public void removeConn(Connection conn)
        {
            _context.Connections.Remove(conn);  
        }

    }
}