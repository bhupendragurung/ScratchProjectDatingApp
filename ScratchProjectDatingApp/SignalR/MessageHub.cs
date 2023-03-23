﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ScratchProjectDatingApp.Data;
using ScratchProjectDatingApp.DTOs;
using ScratchProjectDatingApp.Entity;
using ScratchProjectDatingApp.Extensions;
using ScratchProjectDatingApp.Interfaces;

namespace ScratchProjectDatingApp.SignalR
{
    [Authorize]
    public class MessageHub:Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;

        public MessageHub(IMessageRepository messageRepository,IUserRepository userRepository,IMapper mapper
            ,IHubContext<PresenceHub> presenceHub)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _presenceHub = presenceHub;
        }
        public override async Task OnConnectedAsync()
        {
           var httpContext=Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupaName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group= await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
            var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }
        private string GetGroupaName(string caller,string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();
            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                throw new HubException("You cannot send messages to yourself");
            }
            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null)  throw new HubException("Not Found User");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupaName(sender.UserName, recipient.UserName);
            var group = await _messageRepository.GetMessageGroup(groupName);
            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if(connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new
                    {
                        username = sender.UserName,
                        knownAs = sender.KnownAs
                    });
                }
            }
            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllAsync())
            {
              
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
             
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
          var group=  await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup");
                
            await base.OnDisconnectedAsync(exception);
        }
        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());
            if(group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);
             if( await _messageRepository.SaveAllAsync()) return group;
            throw new Exception("Failed to add group");

        }
        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection=group.Connections.FirstOrDefault(x=>x.ConnectionId==Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
          if( await _messageRepository.SaveAllAsync()) return group;

            throw new Exception("Failed to remove from group");
        }
    }
}
