using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub: Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _tracker;
        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, 
            IMapper mapper, IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        {
            _tracker = tracker;
            _presenceHub = presenceHub;
            _userRepository = userRepository;
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync() 
        {
            var httpCtx = Context.GetHttpContext();
            var otherUser = httpCtx.Request.Query["user"].ToString();
            
            var gName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, gName); 
            var g = await AddToGroup(gName);
            await Clients.Group(gName).SendAsync("UpdatedGroup", g);

            var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages); // what if user already have the message thread ? better way ?
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var g = await RemoveFromMsgGroup();
            await Clients.Group(g.Name).SendAsync("UpdatedGroup", g);
            await base.OnDisconnectedAsync(ex);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();
            if(username == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("You cannot send message to yourself");
            
            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if(recipient == null) throw new HubException("User not found");

            var msg = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };
            
            var gName = GetGroupName(sender.UserName, recipient.UserName);
            var g = await _messageRepository.GetMessageGroupAsync(gName);

            if(g.Connections.Any(c => c.Username == recipient.UserName))
            {
                msg.DateRead = DateTime.UtcNow; // different timezones bug
            }else
            {
                var conns = await _tracker.GetConnectionsForUser(recipient.UserName);
                if(conns != null) // recipient is online but not connected to the same group
                {
                    await _presenceHub.Clients.Clients(conns).SendAsync("NewMessageReceived",
                    new {username = sender.UserName, knownAs = sender.KnownAs});
                }
            }

            _messageRepository.AddMessage(msg);

            if(await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(gName).SendAsync("NewMessage", _mapper.Map<MessageDto>(msg));
            }
        }

        private async Task<Group> AddToGroup(string gName)
        {
            var g = await _messageRepository.GetMessageGroupAsync(gName);
            var conn = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if(g == null)
            {
                g = new Group(gName);
                _messageRepository.AddGroup(g);
            }
            g.Connections.Add(conn);
            if(await _messageRepository.SaveAllAsync()) return g;
            throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMsgGroup()
        {
            var g = await _messageRepository.GetGroupForConnectionAsync(Context.ConnectionId);
            var conn = g.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            _messageRepository.removeConn(conn);
            if(await _messageRepository.SaveAllAsync()) return g;

            throw new HubException("Failed to remove from group");
        }

        private string GetGroupName(string caller, string other) // each group will be identified by [username1, username2] holding a pair of users
        {
            var cmp = string.CompareOrdinal(caller, other) < 0;
            return cmp ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}