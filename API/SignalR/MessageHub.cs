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
        public MessageHub(IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync() 
        {
            var httpCtx = Context.GetHttpContext();
            var otherUser = httpCtx.Request.Query["user"].ToString();
            
            var gName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, gName); 

            var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Group(gName).SendAsync("ReceiveMessageThread", messages); // what if user already have the message thread ? better way ?
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await base.OnDisconnectedAsync(ex);
        }

        private string GetGroupName(string caller, string other) // each group will be identified by [username1, username2] holding a pair of users
        {
            var cmp = string.CompareOrdinal(caller, other) < 0;
            return cmp ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}