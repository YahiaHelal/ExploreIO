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
        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
        {
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
            await AddToGroup(gName);

            var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Group(gName).SendAsync("ReceiveMessageThread", messages); // what if user already have the message thread ? better way ?
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await RemoveFromMsgGroup();
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
            }

            _messageRepository.AddMessage(msg);

            if(await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(gName).SendAsync("NewMessage", _mapper.Map<MessageDto>(msg));
            }
        }

        private async Task<bool> AddToGroup(string gName)
        {
            var g = await _messageRepository.GetMessageGroupAsync(gName);
            var conn = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if(g == null)
            {
                g = new Group(gName);
                _messageRepository.AddGroup(g);
            }
            g.Connections.Add(conn);
            return await _messageRepository.SaveAllAsync();
        }

        private async Task RemoveFromMsgGroup()
        {
            var conn = await _messageRepository.GetConnectionAsync(Context.ConnectionId);
            _messageRepository.removeConn(conn);
            await _messageRepository.SaveAllAsync();
        }

        private string GetGroupName(string caller, string other) // each group will be identified by [username1, username2] holding a pair of users
        {
            var cmp = string.CompareOrdinal(caller, other) < 0;
            return cmp ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}