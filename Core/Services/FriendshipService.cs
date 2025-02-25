﻿using Core.DTOs;
using Core.Interfaces;
using Core.Interfaces.Authentication;
using Core.Response;
using Storage.Models;
using Storage.Models.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    public class FriendshipService : IFriendshipService
    {
        readonly IFriendshipRepository _friendshipRepository;
        readonly IUserRepository _userRepository;
        readonly ILoggedUserProvider _loggedUserProvider;

        public FriendshipService(IFriendshipRepository friendshipRepository, IUserRepository userRepository, ILoggedUserProvider loggedUserProvider)
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
            _loggedUserProvider = loggedUserProvider;
        }

        public async Task<ServiceResponse> GetFriends()
        {
            var friendships = await _friendshipRepository.GetFriendships(true);
            var currentId = _loggedUserProvider.GetUserId();
            List<Guid> ids = new();
            foreach (Friendship friendship in friendships)
            {
                ids.Add(friendship.ToFriend == currentId ? friendship.FromFriend : friendship.ToFriend);
            }
            List<FriendListElementDto> friends = new();
            var users = await _userRepository.GetUsers(ids);
            foreach (User user in users)
            {
                friends.Add(new FriendListElementDto() { Id = user.Id, Username = user.UserName });
            }

            return ServiceResponse<IEnumerable<FriendListElementDto>>.Success(friends, "Friends retrieved");
        }

        public async Task<ServiceResponse> GetInvitations()
        {
            var friendships = await _friendshipRepository.GetFriendships(false);
            var currentId = _loggedUserProvider.GetUserId();
            List<Guid> ids = new();
            List<FriendInvitationDto> invitations = new();
            foreach (Friendship friendship in friendships)
            {
                if (friendship.ToFriend == currentId)
                {
                    ids.Add(friendship.FromFriend);
                    invitations.Add(new FriendInvitationDto() { Id = friendship.Id, UserId = friendship.FromFriend, Notification = friendship.Notification });
                }
            }

            var users = await _userRepository.GetUsers(ids);
            foreach (User user in users)
            {
                invitations.Find(i => i.UserId == user.Id).Username = user.UserName;
            }

            return ServiceResponse<IEnumerable<FriendInvitationDto>>.Success(invitations, "Friendships invites retrieved");
        }

        public async Task<ServiceResponse> SendInvitation(Guid toId)
        {
            var create = await _friendshipRepository.CreateFriendship(toId);
            if (create)
                return ServiceResponse.Success("Send friend invitation");

            return ServiceResponse.Error("Invitation not send");
        }

        public async Task<ServiceResponse> AcceptFriend(int requestId)
        {
            var request = await _friendshipRepository.GetFriendship(requestId);
            await _friendshipRepository.AcceptFriendship(request);

            return ServiceResponse.Success("Friend accepted");
        }

        public async Task<ServiceResponse> DeleteFriend(Guid friendId)
        {
            var deleted = await _friendshipRepository.DeleteFriendshipByFriend(friendId);
            if (deleted)
                return ServiceResponse.Success("Friend deleted");
            return ServiceResponse.Error("Error deleting friend");
        }

        public async Task<ServiceResponse> DeleteInvitation(int requestId)
        {
            var deleted = await _friendshipRepository.DeleteFriendshipByInvitation(requestId);
            if (deleted)
                return ServiceResponse.Success("Invitation deleted");
            return ServiceResponse.Error("Error deleting friend invitation");
        }

        public async Task<ServiceResponse> ReadRequest(int requestId)
        {
            var friendship = await _friendshipRepository.GetFriendship(requestId);
            await _friendshipRepository.ReadFriendship(friendship);

            return ServiceResponse.Success("Notificated");
        }

        public async Task<bool> IsFriend(Guid friendId)
        {
            var friendships = await _friendshipRepository.GetFriendships(true);
            foreach (Friendship friendship in friendships)
            {
                if (friendship.ToFriend == friendId || friendship.FromFriend == friendId)
                    return true;
            }
            return false;
        }
    }
}
